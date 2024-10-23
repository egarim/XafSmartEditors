// Copyright (c) Microsoft. All rights reserved.

using DevExpress.Persistent.Base;

using System;
using DevExpress.Xpo;
using DevExpress.ExpressApp;

namespace Microsoft.SemanticKernel.Connectors.Xpo;
[DefaultClassOptions]
public class XpMemoryCollection : XPCustomObject
{
    public XpMemoryCollection(Session session) : base(session)
    { }

    string collectionName;

    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    public string CollectionName
    {
        get => collectionName;
        set => SetPropertyValue(nameof(CollectionName), ref collectionName, value);
    }

}
public class XpoDatabaseEntry : XPCustomObject, IXpoMemoryEntry
{
    public XpoDatabaseEntry(Session session) : base(session)
    {
    }

    DateTime timestamp;
    private string _oid;
    private string _collection;

    private string _embeddingString;
    private string _metadataString;
    private string _key;



    //
    // Summary:
    //     Set this field to `true`` before profiling the application via XPO Profiler.
    public static bool IsXpoProfiling;

    [Persistent("Oid")]
    [Key(true)]
    [VisibleInListView(false)]
    [VisibleInDetailView(false)]
    [VisibleInLookupListView(false)]
    [MemberDesignTimeVisibility(false)]
    private Guid oid = Guid.Empty;

    [PersistentAlias("oid")]
    [VisibleInListView(false)]
    [VisibleInDetailView(false)]
    [VisibleInLookupListView(false)]
    public Guid Oid => oid;


    public override void AfterConstruction()
    {
        base.AfterConstruction();
        oid = XpoDefault.NewGuid();
    }





    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    public string Key
    {
        get => this._key;
        set => this.SetPropertyValue(nameof(this.Key), ref this._key, value);
    }

    [Size(SizeAttribute.Unlimited)]
    public string MetadataString
    {
        get => this._metadataString;
        set => this.SetPropertyValue(nameof(this.MetadataString), ref this._metadataString, value);
    }

    [Size(SizeAttribute.Unlimited)]
    public string EmbeddingString
    {
        get => this._embeddingString;
        set => this.SetPropertyValue(nameof(this.EmbeddingString), ref this._embeddingString, value);
    }

    
    public DateTime Timestamp
    {
        get => timestamp;
        set => SetPropertyValue(nameof(Timestamp), ref timestamp, value);
    }

    [Size(SizeAttribute.DefaultStringMappingFieldSize)]
    public string Collection
    {
        get => this._collection;
        set => this.SetPropertyValue(nameof(this.Collection), ref this._collection, value);
    }

}
