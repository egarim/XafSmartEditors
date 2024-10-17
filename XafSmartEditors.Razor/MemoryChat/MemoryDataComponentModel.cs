using DevExpress.ExpressApp.Blazor.Components.Models;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;

namespace XafSmartEditors.Razor.MemoryChat
{
    public class MemoryDataComponentModel : ComponentModelBase
    {
        public IMemoryData Value
        {
            get => GetPropertyValue<IMemoryData>();
            set => SetPropertyValue(value);
        }

        public EventCallback<IMemoryData> ValueChanged
        {
            get => GetPropertyValue<EventCallback<IMemoryData>>();
            set => SetPropertyValue(value);
        }


        public override Type ComponentType => typeof(MemoryChatComponent);
    }
}
