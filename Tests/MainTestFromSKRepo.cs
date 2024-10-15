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
    public class MainTestFromSKRepo
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

            XpoTypesInfoHelper.GetXpoTypeInfoSource();
            XafTypesInfo.Instance.RegisterEntity(typeof(XpoDatabaseEntry));
            XPObjectSpaceProvider osProvider = new XPObjectSpaceProvider(cnx, null);
            osProvider.SchemaUpdateMode= SchemaUpdateMode.DatabaseAndSchema;
           
            IObjectSpace objectSpace = osProvider.CreateUpdatingObjectSpace(true);//osProvider.CreateObjectSpace();


            XafEntryManager xafEntryManager=new XafEntryManager(objectSpace);

            //cnx = "Integrated Security=SSPI;Pooling=false;Data Source=(localdb)\\mssqllocaldb;Initial Catalog=XpoKernelMemory";
            XpoMemoryStore store = await XpoMemoryStore.ConnectAsync(xafEntryManager);


            await RunWithStoreAsync(store);
        }

        private async Task RunWithStoreAsync(IMemoryStore memoryStore)
        {
            var EmbeddingModelId = "text-embedding-3-small";
            var ChatModelId = "gpt-4o";


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

            // Retrieve a memory
            Debug.WriteLine("== PART 1b: Retrieving Memories through the ISemanticTextMemory object ==");
            MemoryQueryResult? lookup = await textMemory.GetAsync(MemoryCollectionName, "info1");
            Debug.WriteLine("Memory with key 'info1':" + lookup?.Metadata.Text ?? "ERROR: memory not found");
            Debug.WriteLine("");

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // PART 2: Create TextMemoryPlugin, store and retrieve memories through the Kernel.
            //
            // This enables prompt functions and the AI (via Planners) to access memories
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            Debug.WriteLine("== PART 2a: Saving Memories through the Kernel with TextMemoryPlugin and the 'Save' function ==");

            // Import the TextMemoryPlugin into the Kernel for other functions
            var memoryPlugin = kernel.ImportPluginFromObject(new TextMemoryPlugin(textMemory));

            // Save a memory with the Kernel
            Debug.WriteLine("Saving memory with key 'info5': \"My family is from New York\"");
            await kernel.InvokeAsync(memoryPlugin["Save"], new()
            {
                [TextMemoryPlugin.InputParam] = "My family is from New York",
                [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
                [TextMemoryPlugin.KeyParam] = "info5",
            });

            // Retrieve a specific memory with the Kernel
            Debug.WriteLine("== PART 2b: Retrieving Memories through the Kernel with TextMemoryPlugin and the 'Retrieve' function ==");
            var result = await kernel.InvokeAsync(memoryPlugin["Retrieve"], new KernelArguments()
            {
                [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
                [TextMemoryPlugin.KeyParam] = "info5"
            });

            Debug.WriteLine("Memory with key 'info5':" + result.GetValue<string>() ?? "ERROR: memory not found");
            Debug.WriteLine("");

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // PART 3: Recall similar ideas with semantic search
            //
            // Uses AI Embeddings for fuzzy lookup of memories based on intent, rather than a specific key.
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            Debug.WriteLine("== PART 3: Recall (similarity search) with AI Embeddings ==");

            Debug.WriteLine("== PART 3a: Recall (similarity search) with ISemanticTextMemory ==");
            Debug.WriteLine("Ask: where did I grow up?");

            IAsyncEnumerable<MemoryQueryResult> answers = textMemory.SearchAsync(
                            collection: MemoryCollectionName,
                            query: "where did I grow up?",
                            limit: 2,
                            minRelevanceScore: 0.20,
                            //minRelevanceScore: 0.79, //HACK depending on the model you are using you might need to change this parameter to a lower value I got good results with 0.20
                            withEmbeddings: true);

            await foreach (var answer in answers)
            {
                Debug.WriteLine($"Answer: {answer.Metadata.Text}");
            }

            Debug.WriteLine("== PART 3b: Recall (similarity search) with Kernel and TextMemoryPlugin 'Recall' function ==");
            Debug.WriteLine("Ask: where do I live?");

            result = await kernel.InvokeAsync(memoryPlugin["Recall"], new()
            {
                [TextMemoryPlugin.InputParam] = "Ask: where do I live?",
                [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
                [TextMemoryPlugin.LimitParam] = "2",
                //[TextMemoryPlugin.RelevanceParam] = "0.79", HACK depending on the model you are using you might need to change this parameter to a lower value I got good results with 0.20
            });

            Debug.WriteLine($"Answer: {result.GetValue<string>()}");
            Debug.WriteLine("");

            /*
            Output:

                Ask: where did I grow up?
                Answer:
                    ["My family is from New York","I\u0027ve been living in Seattle since 2005"]

                Ask: where do I live?
                Answer:
                    ["I\u0027ve been living in Seattle since 2005","My family is from New York"]
            */

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // PART 4: TextMemoryPlugin Recall in a Prompt Function
            //
            // Looks up related memories when rendering a prompt template, then sends the rendered prompt to
            // the text generation model to answer a natural language query.
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            Debug.WriteLine("== PART 4: Using TextMemoryPlugin 'Recall' function in a Prompt Function ==");

            // Build a prompt function that uses memory to find facts
            const string RecallFunctionDefinition = @"
            Consider only the facts below when answering questions:

            BEGIN FACTS
            About me: {{recall 'where did I grow up?'}}
            About me: {{recall 'where do I live now?'}}
            END FACTS

            Question: {{$input}}

            Answer:
            ";

            var aboutMeOracle = kernel.CreateFunctionFromPrompt(RecallFunctionDefinition, new OpenAIPromptExecutionSettings() { MaxTokens = 100 });

            result = await kernel.InvokeAsync(aboutMeOracle, new()
            {
                [TextMemoryPlugin.InputParam] = "Do I live in the same town where I grew up?",
                [TextMemoryPlugin.CollectionParam] = MemoryCollectionName,
                [TextMemoryPlugin.LimitParam] = "2",
                [TextMemoryPlugin.RelevanceParam] = "0.79",
            });

            Debug.WriteLine("Ask: Do I live in the same town where I grew up?");
            Debug.WriteLine($"Answer: {result.GetValue<string>()}");

            /*
            Approximate Output:
                Answer: No, I do not live in the same town where I grew up since my family is from New York and I have been living in Seattle since 2005.
            */

            /////////////////////////////////////////////////////////////////////////////////////////////////////
            // PART 5: Cleanup, deleting database collection
            //
            /////////////////////////////////////////////////////////////////////////////////////////////////////

            Debug.WriteLine("== PART 5: Cleanup, deleting database collection ==");

            Debug.WriteLine("Printing Collections in DB...");
            var collections = memoryStore.GetCollectionsAsync();
            await foreach (var collection in collections)
            {
                Debug.WriteLine(collection);
            }
            Debug.WriteLine("");

            Debug.WriteLine($"Removing Collection {MemoryCollectionName}");
            await memoryStore.DeleteCollectionAsync(MemoryCollectionName);
            Debug.WriteLine("");

            Debug.WriteLine($"Printing Collections in DB (after removing {MemoryCollectionName})...");
            collections = memoryStore.GetCollectionsAsync();
            await foreach (var collection in collections)
            {
                Debug.WriteLine(collection);
            }
        }
    }
}