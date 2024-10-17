using DevExpress.ExpressApp.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#pragma warning disable IDE0039
#pragma warning disable SKEXP0010
#pragma warning disable SKEXP0001
#pragma warning disable SKEXP0050
namespace XafSmartEditors.Razor.MemoryChat
{
    public interface IMemoryData
    {
        IChatCompletionService ChatCompletionService { get; set; }
        SemanticTextMemory SemanticTextMemory { get; set; }
        string CollectionName { get; set; }
        string Prompt { get; set; }
        double MinimumRelevanceScore { get; set; }


    }
}
