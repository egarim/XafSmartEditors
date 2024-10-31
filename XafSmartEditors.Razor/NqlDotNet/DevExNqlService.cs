using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using OpenAI;
using System.Diagnostics;
using System.Text;
using MongoDB.Bson.IO;
using MongoDB.Bson;
using System.Text.Json;
namespace NqlDotNet.DevExpress
{
#pragma warning disable SKEXP0010
    public class DevExNqlService : INqlService
    {
        IChatCompletionService? chatService;
        Kernel? sk;
        const string ResultFormat = "{\"RootEntity\": null, \"Criteria\": null}";
        public DevExNqlService()
        {
            var clientOpenAi = new OpenAIClient(new System.ClientModel.ApiKeyCredential(Environment.GetEnvironmentVariable("OpenAiTestKey")));
            var KernelBuilder = Kernel.CreateBuilder();
            KernelBuilder.AddOpenAIChatCompletion("gpt-4o-mini", clientOpenAi);
            sk = KernelBuilder.Build();
            chatService = sk.GetRequiredService<IChatCompletionService>();
        }
        public async Task<CriteriaResult> NlToCriteria(string Nlq, string Schema, string Doc)
        {
      
             KernelArguments arguments = new(new OpenAIPromptExecutionSettings { MaxTokens = 500, Temperature = 0.5, ResponseFormat="json_object", ChatSystemPrompt =$"Given the DevExpress Criteria Language syntax documentation {Doc} and JSON schema of classes {Schema},only use functions from the documentation and not use properties that are not in the schema,make sure that the properties exist in order to navigate within the property tree, if there is not relationship or collection to navigate from,try with a different entity as entry point" });
            //Given the DevExpress Criteria Language syntax documentation and the.
            FunctionResult value = await sk.InvokePromptAsync($"transform a natural language query #{Nlq}# into a criteria string suitable for querying data,return only the criteria without any extra text or explanation, the result should be a json with this structure {ResultFormat}", arguments);
            //FunctionResult value = await sk.InvokePromptAsync($"Given the DevExpress Criteria Language syntax documentation {Doc} and JSON schema of classes {Schema}, transform a natural language query #{Nlq}# into a criteria string suitable for querying data,return only the criteria without any extra text or explanation, the result should be a json with this structure {ResultFormat},only use functions from the documentation and not use properties that are not in the schema", arguments);
            Debug.WriteLine(value);
            CriteriaResult result = JsonSerializer.Deserialize<CriteriaResult>(value.ToString());
            return result;
        }
        public async Task<CriteriaResult> CriteriaToNl(string Criteria, string Schema, string Doc)
        {
           
            KernelArguments arguments = new(new OpenAIPromptExecutionSettings { MaxTokens = 500, Temperature = 0.5 });
            //Given the DevExpress Criteria Language syntax documentation and the.
            FunctionResult value = await sk.InvokePromptAsync($"Given the DevExpress Criteria Language syntax documentation {Doc} and JSON schema of classes {Schema}, transform the #{Criteria}# into a natural language,return the answer and explanation how you did it", arguments);
            Debug.WriteLine(value);
            CriteriaResult criteriaResult =new CriteriaResult();
            criteriaResult.CriteriaDescription = value.ToString();
            criteriaResult.Criteria = Criteria;
            return criteriaResult;
            
        }
    }
}
