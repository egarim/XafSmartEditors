using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Blazor;
using Microsoft.JSInterop;
using XafSmartEditors.Module.Controllers;

namespace XafSmartEditors.Blazor.Server.Controllers
{
    public class NqlControllerBlazor : NqlController
    {
        IJSRuntime jsRuntime;
        IServiceProvider serviceProvider;
        ILoadingIndicatorProvider loading;
        protected override void OnActivated()
        {
            base.OnActivated();
            IServiceProvider serviceProvider = ((BlazorApplication)Application).ServiceProvider;
            jsRuntime = (IJSRuntime)serviceProvider.GetService(typeof(IJSRuntime));
            loading = serviceProvider.GetService(typeof(ILoadingIndicatorProvider)) as ILoadingIndicatorProvider;


        }
        protected override async void action_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //HACK show loading indicator
            loading.Hold("Loading");
            //Execute the base action on the agnostic module
            base.action_Execute(sender, e);
        }
        protected override void ProcessingDone(Dictionary<int, object> results)
        {
            //HACK this is in the U.I thread, so we can interact with the U.I and the view and object space of this controller
            base.ProcessingDone(results);
            //HACK hide loading indicator
            loading.Release("Loading");
        }
    }
}
