using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XafSmartEditors.Module.BusinessObjects;
using XafSmartEditors.Razor.RagChat;
using XafSmartEditors.Razor.MemoryChat;
using Microsoft.SemanticKernel.Connectors.Xpo;
using XafSmartEditors.SemanticKernel.Memory;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.Memory;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using OpenAI;
#pragma warning disable IDE0039
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0050

namespace XafSmartEditors.Module.Controllers
{
    public class OpenMemoryChatController : ViewController
    {
        PopupWindowShowAction Chat;
        const string ChatModelId = "gpt-4o-mini";
        const string DefaultPrompt = "You are an analytics assistant specialized in analyzing PDF files. Your role is to assist users by providing accurate answers to their questions about data contained within these files.\n \n### Tasks:\n- Perform various types of data analyses, including summaries, calculations, data filtering, and trend identification.\n- Clearly explain your analysis process to ensure users understand how you arrived at your answers.\n- Always provide precise and accurate information based on the Excel data.\n- If you cannot find an answer based on the provided data, explicitly state: \"The requested information cannot be found in the data provided.\"\n \n### Examples:\n1. **Summarization:**\n   - **User Question:** \"What is the average sales revenue for Q1?\"\n   - **Response:** \"The average sales revenue for Q1 is calculated as $45,000, based on the data in Sheet1, Column C.\"\n \n2. **Data Filtering:**\n   - **User Question:** \"Which products had sales over $10,000 in June?\"\n   - **Response:** \"The products with sales over $10,000 in June are listed in Sheet2, Column D, and they include Product A, Product B, and Product C.\"\n \n3. **Insufficient Data:**\n   - **User Question:** \"What is the market trend for Product Z over the past 5 years?\"\n   - **Response:** \"The requested information cannot be found in the data provided, as the dataset only includes data for the current year.\"\n \n### Additional Instructions:\n- Format your responses to clearly indicate which sheet and column the data was extracted from when necessary.\n- Avoid providing any answers if the data in the file is insufficient for a reliable response.\n- Ask clarifying questions if the user's query is ambiguous or lacks detail.\n \nRemember, your primary goal is to provide helpful, data-driven insights that directly answer the user's questions. Do not assume or infer information not present in the dataset.Always return response well formatted using markdown.";
        public OpenMemoryChatController() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            this.TargetObjectType = typeof(MemoryChat);

            Chat = new PopupWindowShowAction(this, "ChatWithMemoryAction", "View");
            Chat.Caption = "Chat";
            Chat.ImageName = "artificial_intelligence";
            Chat.Execute += Chat_Execute;
            Chat.CustomizePopupWindowParams += Chat_CustomizePopupWindowParams;
        }
        private void Chat_Execute(object sender, PopupWindowShowActionExecuteEventArgs e)
        {
            var selectedPopupWindowObjects = e.PopupWindowViewSelectedObjects;
            var selectedSourceViewObjects = e.SelectedObjects;
        }
        private void Chat_CustomizePopupWindowParams(object sender, CustomizePopupWindowParamsEventArgs e)
        {
            MemoryChat memoryChat = this.View.CurrentObject as MemoryChat;
            var os = this.Application.CreateObjectSpace(typeof(MemoryChatView));
            var ChatView = os.CreateObject<MemoryChatView>();


            XafEntryManager xafEntryManager = new XafEntryManager(this.View.ObjectSpace);


            XpoMemoryStore store = XpoMemoryStore.ConnectAsync(xafEntryManager).GetAwaiter().GetResult();
            SemanticTextMemory semanticTextMemory = GetSemanticTextMemory(store);


            var clientOpenAi = new OpenAIClient(new System.ClientModel.ApiKeyCredential(Environment.GetEnvironmentVariable("OpenAiTestKey")));
            var KernelBuilder = Kernel.CreateBuilder();
            KernelBuilder.AddOpenAIChatCompletion(ChatModelId, clientOpenAi);
            var sk = KernelBuilder.Build();
            var ChatService = sk.GetRequiredService<IChatCompletionService>();

            ChatView.Memory = os.CreateObject<IMemoryDataImp>();
            ChatView.Memory.SemanticTextMemory = semanticTextMemory;
            ChatView.Memory.ChatCompletionService = ChatService;


            DetailView detailView = this.Application.CreateDetailView(os, ChatView);

            detailView.Caption = $"Chat with memory collection | {memoryChat.Name}";


            e.DialogController.AcceptAction.Active["NothingToAccept"] = false;
            e.DialogController.CancelAction.Active["NothingToCancel"] = false;
            e.View = detailView;
        }

        private static SemanticTextMemory GetSemanticTextMemory(XpoMemoryStore store)
        {
            var EmbeddingModelId = "text-embedding-3-small";
         

            var GetKey = () => Environment.GetEnvironmentVariable("OpenAiTestKey", EnvironmentVariableTarget.Machine);
            var kernel = Kernel.CreateBuilder()
               .AddOpenAIChatCompletion(ChatModelId, GetKey.Invoke())
               .AddOpenAITextEmbeddingGeneration(EmbeddingModelId, GetKey.Invoke())
               .Build();

            // Create an embedding generator to use for semantic memory.
            var embeddingGenerator = new OpenAITextEmbeddingGenerationService(EmbeddingModelId, GetKey.Invoke());

            // The combination of the text embedding generator and the memory store makes up the 'SemanticTextMemory' object used to
            // store and retrieve memories.
            SemanticTextMemory textMemory = new(store, embeddingGenerator);
            return textMemory;
        }

        protected override void OnActivated()
        {
            base.OnActivated();
            // Perform various tasks depending on the target View.
        }
        protected override void OnDeactivated()
        {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
        protected override void OnViewControlsCreated()
        {
            base.OnViewControlsCreated();
            // Access and customize the target View control.
        }
    }
}
