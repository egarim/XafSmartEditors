using DevExpress.ExpressApp.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafSmartEditors.Razor.RagChat
{
    public interface IRagData
    {
        Stream FileContent { get; set; }
        string Prompt { get; set; }
        string FileName { get; set; }

    }
}
