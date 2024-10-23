// Copyright (c) Microsoft. All rights reserved.

using System;

namespace Microsoft.SemanticKernel.Connectors.Xpo
{
    public interface IXpoMemoryEntry
    {
        string Collection { get; set; }
        string EmbeddingString { get; set; }
        string Key { get; set; }
        string MetadataString { get; set; }
   
        DateTime Timestamp { get; set; }
    }
}
