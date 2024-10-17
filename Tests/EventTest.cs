using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Connectors.Xpo;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel;
using System.Diagnostics;
using DevExpress.ExpressApp.Xpo;
using DevExpress.ExpressApp;
using DocumentFormat.OpenXml.Bibliography;
using XafSmartEditors.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Plugins.Memory;
#pragma warning disable IDE0039
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0050
namespace Tests
{
    public class EventTest
    {
        [SetUp]
        public void Setup()
        {
        }

    

        private const string MemoryCollectionName = "aboutMe";

        [Test]
        public async Task RunAsync()
        {

            // Xpo Memory Store - using InMemoryDataStore, an in-memory store that is not persisted

            string cnx = DevExpress.Xpo.DB.InMemoryDataStore.GetConnectionStringInMemory(true);
            cnx = "Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=XafSmartEditorSk";
            XpoTypesInfoHelper.GetXpoTypeInfoSource();
            XafTypesInfo.Instance.RegisterEntity(typeof(XpoDatabaseEntry));
            XPObjectSpaceProvider osProvider = new XPObjectSpaceProvider(cnx, null);
            osProvider.SchemaUpdateMode= SchemaUpdateMode.DatabaseAndSchema;
           
            IObjectSpace objectSpace = osProvider.CreateUpdatingObjectSpace(true);//osProvider.CreateObjectSpace();


            XafEntryManager xafEntryManager=new XafEntryManager(objectSpace);

           
            XpoMemoryStore store = await XpoMemoryStore.ConnectAsync(xafEntryManager);


            await RunWithStoreAsync(store, xafEntryManager);
        }

        private async Task RunWithStoreAsync(IMemoryStore memoryStore, XafEntryManager xafEntryManager)
        {
            var EmbeddingModelId = "text-embedding-3-small";
            var ChatModelId = "gpt-4o-mini";
            int CountObjectCreated = 0;

            xafEntryManager.ObjectCreatedEvent += XafEntryManager_ObjectCreatedEvent;
            void XafEntryManager_ObjectCreatedEvent(object? sender, EventArgs e)
            {
                CountObjectCreated++;
            }
#pragma warning disable IDE0039
            var GetKey = () => Environment.GetEnvironmentVariable("OpenAiTestKey", EnvironmentVariableTarget.Machine);
            var kernel = Kernel.CreateBuilder()
               .AddOpenAIChatCompletion(ChatModelId, GetKey.Invoke())
               .AddOpenAITextEmbeddingGeneration(EmbeddingModelId, GetKey.Invoke())
               .Build();

            // Create an embedding generator to use for semantic memory.
            var embeddingGenerator = new OpenAITextEmbeddingGenerationService(EmbeddingModelId, GetKey.Invoke());

            // The combination of the text embedding generator and the memory store makes up the 'SemanticTextMemory' object used to
            // store and retrieve memories.
            SemanticTextMemory textMemory = new(memoryStore, embeddingGenerator);

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // PART 1: Store and retrieve memories using the ISemanticTextMemory (textMemory) object.
            //
            // This is a simple way to store memories from a code perspective, without using the Kernel.
            /////////////////////////////////////////////////////////////////////////////////////////////////////
            Debug.WriteLine("== PART 1a: Saving Memories through the ISemanticTextMemory object ==");

            Debug.WriteLine("Saving memory with key 'info1': \"My name is Andrea\"");
            await textMemory.SaveInformationAsync(MemoryCollectionName, id: "info1", text: "My name is Andrea");

            Debug.WriteLine("Saving memory with key 'info2': \"I work as a tourist operator\"");
            await textMemory.SaveInformationAsync(MemoryCollectionName, id: "info2", text: "I work as a tourist operator");

            Debug.WriteLine("Saving memory with key 'info3': \"I've been living in Seattle since 2005\"");
            await textMemory.SaveInformationAsync(MemoryCollectionName, id: "info3", text: "I've been living in Seattle since 2005");

            Debug.WriteLine("Saving memory with key 'info4': \"I visited France and Italy five times since 2015\"");
            await textMemory.SaveInformationAsync(MemoryCollectionName, id: "info4", text: "I visited France and Italy five times since 2015");

            Assert.AreEqual(4, CountObjectCreated);

            
        }

    
    }
}