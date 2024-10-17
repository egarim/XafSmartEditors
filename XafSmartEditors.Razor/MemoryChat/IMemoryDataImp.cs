using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using XafSmartEditors.Razor.RagChat;
#pragma warning disable IDE0039
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0050
namespace XafSmartEditors.Razor.MemoryChat
{
    [DomainComponent]
    public class IMemoryDataImp : IMemoryData, IXafEntityObject/*, IObjectSpaceLink*/, INotifyPropertyChanged
    {
        //private IObjectSpace objectSpace;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        public IMemoryDataImp()
        {
            Oid = Guid.NewGuid();
        }

        [DevExpress.ExpressApp.Data.Key]
        [Browsable(false)]  // Hide the entity identifier from UI.
        public Guid Oid { get; set; }


        double minimumRelevanceScore;
        string collectionName;
        IChatCompletionService chatCompletionService;
        SemanticTextMemory semanticTextMemory;
        string prompt;
        string fileName;

        public SemanticTextMemory SemanticTextMemory
        {
            get => semanticTextMemory;
            set
            {
                if (semanticTextMemory == value)
                    return;
                semanticTextMemory = value;
                OnPropertyChanged();
            }
        }

        public IChatCompletionService ChatCompletionService
        {
            get => chatCompletionService;
            set
            {
                if (chatCompletionService == value)
                    return;
                chatCompletionService = value;
                OnPropertyChanged();
            }
        }



        public string Prompt
        {
            get => prompt;
            set
            {
                if (prompt == value)
                    return;
                prompt = value;
                OnPropertyChanged();
            }
        }


        public string CollectionName
        {
            get => collectionName;
            set
            {
                if (collectionName == value)
                    return;
                collectionName = value;
                OnPropertyChanged();
            }
        }

        
        public double MinimumRelevanceScore
        {
            get => minimumRelevanceScore;
            set
            {
                if (minimumRelevanceScore == value)
                    return;
                minimumRelevanceScore = value;
                OnPropertyChanged();
            }
        }
        


        #region IXafEntityObject members (see https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.IXafEntityObject)
        void IXafEntityObject.OnCreated()
        {
            // Place the entity initialization code here.
            // You can initialize reference properties using Object Space methods; e.g.:
            // this.Address = objectSpace.CreateObject<Address>();
        }
        void IXafEntityObject.OnLoaded()
        {
            // Place the code that is executed each time the entity is loaded here.
        }
        void IXafEntityObject.OnSaving()
        {
            // Place the code that is executed each time the entity is saved here.
        }
        #endregion

        #region IObjectSpaceLink members (see https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.IObjectSpaceLink)
        // If you implement this interface, handle the NonPersistentObjectSpace.ObjectGetting event and find or create a copy of the source object in the current Object Space.
        // Use the Object Space to access other entities (see https://docs.devexpress.com/eXpressAppFramework/113707/data-manipulation-and-business-logic/object-space).
        //IObjectSpace IObjectSpaceLink.ObjectSpace {
        //    get { return objectSpace; }
        //    set { objectSpace = value; }
        //}
        #endregion

        #region INotifyPropertyChanged members (see https://learn.microsoft.com/en-us/dotnet/api/system.componentmodel.inotifypropertychanged?view=net-8.0&redirectedfrom=MSDN)
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}