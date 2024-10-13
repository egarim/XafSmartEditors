﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics.Tensors;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Xpo;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Text;
using XafSmartEditors.SemanticKernel;

namespace Microsoft.SemanticKernel.Connectors.Xpo;
#pragma warning disable SKEXP0001
/// <summary>
/// An implementation of <see cref="IMemoryStore"/> backed by a XPO database.
/// </summary>
/// <remarks>The data is saved to a database, specified in the constructor.
/// The data persists between subsequent instances.
/// </remarks>
public class XpoMemoryStore : IMemoryStore, IDisposable
{
    /// <summary>
    /// Connect a XPO database
    /// </summary>
    /// <param name="cnx">The XPO connection string. If database does not exist, it will be created.</param>
    /// <param name="cancellationToken">The <see cref="CancellationToken"/> to monitor for cancellation requests. The default is <see cref="CancellationToken.None"/>.</param>
    public static async Task<XpoMemoryStore> ConnectAsync(string cnx,
        CancellationToken cancellationToken = default)
    {
        var memoryStore = new XpoMemoryStore(cnx);
        await memoryStore._dbConnector.CreateTableAsync(memoryStore._dataLayer, cancellationToken).ConfigureAwait(false);
        return memoryStore;
    }

    /// <inheritdoc/>
    public async Task CreateCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await this._dbConnector.CreateCollectionAsync(this._dataLayer, collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<bool> DoesCollectionExistAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        return await this._dbConnector.DoesCollectionExistsAsync(this._dataLayer, collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> GetCollectionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var collection in this._dbConnector.GetCollectionsAsync(this._dataLayer, cancellationToken).ConfigureAwait(false))
        {
            yield return collection;
        }
    }

    /// <inheritdoc/>
    public async Task DeleteCollectionAsync(string collectionName, CancellationToken cancellationToken = default)
    {
        await this._dbConnector.DeleteCollectionAsync(this._dataLayer, collectionName, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task<string> UpsertAsync(string collectionName, MemoryRecord record, CancellationToken cancellationToken = default)
    {
        return await this.InternalUpsertAsync(this._dataLayer, collectionName, record, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<string> UpsertBatchAsync(string collectionName, IEnumerable<MemoryRecord> records,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var record in records)
        {
            yield return await this.InternalUpsertAsync(this._dataLayer, collectionName, record, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <inheritdoc/>
    public async Task<MemoryRecord?> GetAsync(string collectionName, string key, bool withEmbedding = false, CancellationToken cancellationToken = default)
    {
        return await this.InternalGetAsync(this._dataLayer, collectionName, key, withEmbedding, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<MemoryRecord> GetBatchAsync(string collectionName, IEnumerable<string> keys, bool withEmbeddings = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        foreach (var key in keys)
        {
            var result = await this.InternalGetAsync(this._dataLayer, collectionName, key, withEmbeddings, cancellationToken).ConfigureAwait(false);
            if (result != null)
            {
                yield return result;
            }
            else
            {
                yield break;
            }
        }
    }

    /// <inheritdoc/>
    public async Task RemoveAsync(string collectionName, string key, CancellationToken cancellationToken = default)
    {
        await this._dbConnector.DeleteAsync(this._dataLayer, collectionName, key, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async Task RemoveBatchAsync(string collectionName, IEnumerable<string> keys, CancellationToken cancellationToken = default)
    {
        await Task.WhenAll(keys.Select(k => this._dbConnector.DeleteAsync(this._dataLayer, collectionName, k, cancellationToken))).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public async IAsyncEnumerable<(MemoryRecord, double)> GetNearestMatchesAsync(
        string collectionName,
        ReadOnlyMemory<float> embedding,
        int limit,
        double minRelevanceScore = 0,
        bool withEmbeddings = false,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (limit <= 0)
        {
            yield break;
        }

        var collectionMemories = new List<MemoryRecord>();
        List<(MemoryRecord Record, double Score)> embeddings = [];

        await foreach (var record in this.GetAllAsync(collectionName, cancellationToken).ConfigureAwait(false))
        {
            if (record != null)
            {
                double similarity = TensorPrimitives.CosineSimilarity(embedding.Span, record.Embedding.Span);
                if (similarity >= minRelevanceScore)
                {
                    var entry = withEmbeddings ? record : MemoryRecord.FromMetadata(record.Metadata, ReadOnlyMemory<float>.Empty, record.Key, record.Timestamp);
                    embeddings.Add(new(entry, similarity));
                }
            }
        }

        foreach (var item in embeddings.OrderByDescending(l => l.Score).Take(limit))
        {
            yield return (item.Record, item.Score);
        }
    }

    /// <inheritdoc/>
    public async Task<(MemoryRecord, double)?> GetNearestMatchAsync(string collectionName, ReadOnlyMemory<float> embedding, double minRelevanceScore = 0, bool withEmbedding = false,
        CancellationToken cancellationToken = default)
    {
        return await this.GetNearestMatchesAsync(
            collectionName: collectionName,
            embedding: embedding,
            limit: 1,
            minRelevanceScore: minRelevanceScore,
            withEmbeddings: withEmbedding,
            cancellationToken: cancellationToken).FirstOrDefaultAsync(cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        this.Dispose(true);
        GC.SuppressFinalize(this);
    }

    #region protected ================================================================================
    /// <summary>
    /// Disposes the resources used by the <see cref="XpoMemoryStore"/> instance.
    /// </summary>
    /// <param name="disposing">True to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!this._disposedValue)
        {
            if (disposing)
            {
                this._dataLayer.Dispose();
            }

            this._disposedValue = true;
        }
    }

    #endregion

    #region private ================================================================================

    internal readonly XpoDatabase _dbConnector;
    internal readonly IDataLayer _dataLayer;
    private bool _disposedValue;

    /// <summary>
    /// Constructor
    /// </summary>
    private XpoMemoryStore(string XpoConnectionString)
    {
        this._dbConnector = new XpoDatabase();
        this._dataLayer = XpoDefault.GetDataLayer(XpoConnectionString, DevExpress.Xpo.DB.AutoCreateOption.DatabaseAndSchema);
        this._disposedValue = false;
    }

    private static string? ToTimestampString(DateTimeOffset? timestamp)
    {
        return timestamp?.ToString("u", CultureInfo.InvariantCulture);
    }

    private static DateTimeOffset? ParseTimestamp(string? str)
    {
        if (!string.IsNullOrEmpty(str)
            && DateTimeOffset.TryParse(str, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset timestamp))
        {
            return timestamp;
        }

        return null;
    }

    private async IAsyncEnumerable<MemoryRecord> GetAllAsync(string collectionName, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // delete empty entry in the database if it exists (see CreateCollection)
        await this._dbConnector.DeleteEmptyAsync(this._dataLayer, collectionName, cancellationToken).ConfigureAwait(false);

        await foreach (DatabaseEntry dbEntry in this._dbConnector.ReadAllAsync(this._dataLayer, collectionName, cancellationToken).ConfigureAwait(false))
        {
            ReadOnlyMemory<float> vector = JsonSerializer.Deserialize<ReadOnlyMemory<float>>(dbEntry.EmbeddingString, CustomJsonOptionsCache.Default);

            var record = MemoryRecord.FromJsonMetadata(dbEntry.MetadataString, vector, dbEntry.Key, ParseTimestamp(dbEntry.Timestamp));

            yield return record;
        }
    }

    private async Task<string> InternalUpsertAsync(IDataLayer connection, string collectionName, MemoryRecord record, CancellationToken cancellationToken)
    {
        record.Key = record.Metadata.Id;

        // Update
        await this._dbConnector.UpdateAsync(
            conn: connection,
            collection: collectionName,
            key: record.Key,
            metadata: record.GetSerializedMetadata(),
            embedding: JsonSerializer.Serialize(record.Embedding, CustomJsonOptionsCache.Default),
            timestamp: ToTimestampString(record.Timestamp),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        // Insert if entry does not exists
        await this._dbConnector.InsertOrIgnoreAsync(
            conn: connection,
            collection: collectionName,
            key: record.Key,
            metadata: record.GetSerializedMetadata(),
            embedding: JsonSerializer.Serialize(record.Embedding, CustomJsonOptionsCache.Default),
            timestamp: ToTimestampString(record.Timestamp),
            cancellationToken: cancellationToken).ConfigureAwait(false);

        return record.Key;
    }

    private async Task<MemoryRecord?> InternalGetAsync(
        IDataLayer connection,
        string collectionName,
        string key, bool withEmbedding,
        CancellationToken cancellationToken)
    {
        UnitOfWork unitOfWork = new(connection);
        var entry = unitOfWork.Query<XpoDatabaseEntry>().FirstOrDefault(x => x.Collection == collectionName && x.Key == key);
        if (entry != null)
        {
            if (withEmbedding)
            {
                return MemoryRecord.FromJsonMetadata(
                    json: entry.MetadataString,
                    JsonSerializer.Deserialize<ReadOnlyMemory<float>>(entry.EmbeddingString, CustomJsonOptionsCache.Default),
                    entry.Key,
                    ParseTimestamp(entry.Timestamp));
            }

            return MemoryRecord.FromJsonMetadata(
                json: entry.MetadataString,
                ReadOnlyMemory<float>.Empty,
                entry.Key,
                ParseTimestamp(entry.Timestamp));
        }
        return null;
    }

    #endregion
}
