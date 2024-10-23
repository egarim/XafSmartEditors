// Copyright (c) Microsoft. All rights reserved.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using DevExpress.Xpo;
using DocumentFormat.OpenXml.Spreadsheet;
using XafSmartEditors.SemanticKernel.Memory;

namespace Microsoft.SemanticKernel.Connectors.Xpo;

internal sealed class XpoDatabase
{
    public XpoDatabase() { }

    public Task CreateTableAsync(IDataLayer conn, CancellationToken cancellationToken = default)
    {
        IDataLayer dl = conn;
        using (Session session = new(dl))
        {
            System.Reflection.Assembly[] assemblies = new System.Reflection.Assembly[] {
                    typeof(XpoDatabaseEntry).Assembly };
            session.UpdateSchema(assemblies);
            session.CreateObjectTypeRecords(assemblies);
        }
        return Task.CompletedTask;
    }

    public async Task CreateCollectionAsync(IXpoEntryManager xpoEntryManager, string collectionName, CancellationToken cancellationToken = default)
    {
        if (await this.DoesCollectionExistsAsync(xpoEntryManager, collectionName, cancellationToken).ConfigureAwait(false))
        {
            // Collection already exists
            return;
        }


        IXpoMemoryEntry entry = xpoEntryManager.CreateObject();
        
        entry.Collection = collectionName;
        //await unitOfWork.CommitChangesAsync(cancellationToken).ConfigureAwait(false);
        xpoEntryManager.Commit();
       


    }

    public async Task UpdateAsync(IXpoEntryManager xpoEntryManager,
        string collection, string key, string? metadata, string? embedding, string? timestamp, CancellationToken cancellationToken = default)
    {
       
        var entry = xpoEntryManager.GetQuery().FirstOrDefault(x => x.Collection == collection && x.Key == key);
        if (entry != null)
        {
            entry.MetadataString = metadata ?? string.Empty;
            entry.EmbeddingString = embedding ?? string.Empty;
            entry.Timestamp = timestamp ?? string.Empty;
            xpoEntryManager.Commit();
        }
    }

    public async Task InsertOrIgnoreAsync(IXpoEntryManager xpoEntryManager,
        string collection, string key, string? metadata, string? embedding, string? timestamp, CancellationToken cancellationToken = default)
    {
       
        if (xpoEntryManager.GetQuery().FirstOrDefault(x => x.Collection == collection && x.Key == key) == null)
        {
            IXpoMemoryEntry entry = xpoEntryManager.CreateObject();
            entry.Collection = collection;
            entry.Key = key;
            entry.MetadataString = metadata ?? string.Empty;
            entry.EmbeddingString = embedding ?? string.Empty;
            entry.Timestamp = timestamp ?? string.Empty;
            //await unitOfWork.CommitChangesAsync(cancellationToken).ConfigureAwait(false);
            xpoEntryManager.Commit();
        }
    }

    public async Task<bool> DoesCollectionExistsAsync(IXpoEntryManager xpoEntryManager,
        string collectionName,
        CancellationToken cancellationToken = default)
    {
        var collections = await this.GetCollectionsAsync(xpoEntryManager, cancellationToken).ToListAsync(cancellationToken).ConfigureAwait(false);
        return collections.Contains(collectionName);
    }

    public async IAsyncEnumerable<string> GetCollectionsAsync(IXpoEntryManager xpoEntryManager,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
      

        var Collections = xpoEntryManager.GetQuery().Select(x => x.Collection).Distinct().ToList();
        foreach (string collection in Collections)
        {
            yield return collection;
        }
    }

    public async IAsyncEnumerable<IXpoMemoryEntry> ReadAllAsync(IXpoEntryManager xpoEntryManager,
        string collectionName,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        
        var entries = xpoEntryManager.GetQuery().Where(x => x.Collection == collectionName).ToList();
        foreach (var entry in entries)
        {
            yield return entry;
        }
    }

    public async Task<IXpoMemoryEntry?> ReadAsync(IDataLayer conn,
        string collectionName,
        string key,
        CancellationToken cancellationToken = default)
    {
        UnitOfWork unitOfWork = new(conn);
        var entry = unitOfWork.Query<XpoDatabaseEntry>().FirstOrDefault(x => x.Collection == collectionName && x.Key == key);
        if (entry != null)
        {
            return entry;
        }
        return null;
    }

    public Task DeleteCollectionAsync(IXpoEntryManager xpoEntryManager, string collectionName, CancellationToken cancellationToken = default)
    {
       
        var entries = xpoEntryManager.GetQuery().Where(x => x.Collection == collectionName).ToList();
        foreach (XpoDatabaseEntry entry in entries)
        {
            xpoEntryManager.Delete(entry);
        }
        xpoEntryManager.Commit();
        return Task.CompletedTask;
    }

    public Task DeleteAsync(IXpoEntryManager xpoEntryManager, string collectionName, string key, CancellationToken cancellationToken = default)
    {
        
        var entry = xpoEntryManager.GetQuery().FirstOrDefault(x => x.Collection == collectionName && x.Key == key);
        if (entry != null)
        {
            xpoEntryManager.Delete(entry);
            xpoEntryManager.Commit();
        }
        return Task.CompletedTask;
    }

    public Task DeleteEmptyAsync(IXpoEntryManager xpoEntryManager, string collectionName, CancellationToken cancellationToken = default)
    {
        
        var entries = xpoEntryManager.GetQuery().Where(x => x.Collection == collectionName && x.Key == null).ToList();

        if (entries != null)
        {
            xpoEntryManager.Delete(entries);
            xpoEntryManager.Commit();
        }
        return Task.CompletedTask;
    }
}
