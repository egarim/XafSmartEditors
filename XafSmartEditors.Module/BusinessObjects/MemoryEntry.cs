using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using Microsoft.SemanticKernel.Connectors.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XafSmartEditors.Module.BusinessObjects
{
   
    public class MemoryEntry : BaseObject, IXpoMemoryEntry
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://docs.devexpress.com/eXpressAppFramework/113146/business-model-design-orm/business-model-design-with-xpo/base-persistent-classes).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public MemoryEntry(Session session)
            : base(session)
        {
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://docs.devexpress.com/eXpressAppFramework/112834/getting-started/in-depth-tutorial-winforms-webforms/business-model-design/initialize-a-property-after-creating-an-object-xpo?v=22.1).
        }

        MemoryChat memoryChat;
        string timestamp;
        string metadataString;
        string key;
        string embeddingString;
        string collection;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Collection
        {
            get => collection;
            set => SetPropertyValue(nameof(Collection), ref collection, value);
        }

        [Size(SizeAttribute.Unlimited)]
        public string EmbeddingString
        {
            get => embeddingString;
            set => SetPropertyValue(nameof(EmbeddingString), ref embeddingString, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Key
        {
            get => key;
            set => SetPropertyValue(nameof(Key), ref key, value);
        }

        [Size(SizeAttribute.Unlimited)]
        public string MetadataString
        {
            get => metadataString;
            set => SetPropertyValue(nameof(MetadataString), ref metadataString, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Timestamp
        {
            get => timestamp;
            set => SetPropertyValue(nameof(Timestamp), ref timestamp, value);
        }
        
        [Association("MemoryChat-MemoryEntrys")]
        public MemoryChat MemoryChat
        {
            get => memoryChat;
            set => SetPropertyValue(nameof(MemoryChat), ref memoryChat, value);
        }
    }
}