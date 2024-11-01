using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Blazor;
using Microsoft.JSInterop;
using XafSmartEditors.Module.Controllers;
using Toolbelt.Blazor.SpeechRecognition;
using XafSmartEditors.Module.BusinessObjects;

namespace XafSmartEditors.Blazor.Server.Controllers
{
    public class NqlControllerBlazor : NqlController
    {
        SimpleAction StartSpeechRecognition;
        SpeechRecognition SpeechRecognition;

        IJSRuntime jsRuntime;
        IServiceProvider serviceProvider;
        ILoadingIndicatorProvider loading;
        public NqlControllerBlazor()
        {
            StartSpeechRecognition = new SimpleAction(this, nameof(StartSpeechRecognition), "View");
            StartSpeechRecognition.Execute += StartSpeechRecognition_Execute;
            StartSpeechRecognition.Caption = "Start Speech Recognition";
        }
        protected override void OnActivated()
        {
            base.OnActivated();
            IServiceProvider serviceProvider = ((BlazorApplication)Application).ServiceProvider;
            jsRuntime = (IJSRuntime)serviceProvider.GetService(typeof(IJSRuntime));
            loading = serviceProvider.GetService(typeof(ILoadingIndicatorProvider)) as ILoadingIndicatorProvider;
            this.SpeechRecognition = serviceProvider.GetRequiredService<SpeechRecognition>();
            this.SpeechRecognition.Result += OnSpeechRecognized;
            this.SpeechRecognition.Lang = "en-US";
            this.SpeechRecognition.InterimResults = true;
            this.SpeechRecognition.Continuous = true;
            this.SpeechRecognition.Result += OnSpeechRecognized;
            this.SpeechRecognition.End += OnSpeechEnded;


        }
        private async void StartSpeechRecognition_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            if (!this.Listening)
            {
                await this.SpeechRecognition.StartAsync();
                this.Listening = true;
                this.StartSpeechRecognition.Caption = "Stop Speech Recognition";
            }
            else
            {

                await this.SpeechRecognition.StopAsync();
                this.Listening = false;
                this.StartSpeechRecognition.Caption = "Start Speech Recognition";
            }
        }
        void OnSpeechEnded(object sender, EventArgs e)
        {

            this.generateCriteria.DoExecute();
        }
        protected override void OnDeactivated()
        {
            base.OnDeactivated();
            this.SpeechRecognition.Result -= OnSpeechRecognized;
        }
        private SpeechRecognitionResult[] _results = Array.Empty<SpeechRecognitionResult>();
        public bool Listening { get; set; }
        private  void OnSpeechRecognized(object sender, SpeechRecognitionEventArgs args)
        {
            var Cot = this.View.CurrentObject as CriteriaObjectTest;
            _results = args.Results.Skip(args.ResultIndex).ToArray();

            foreach (var item in _results)
            {
                Cot.Nql = item.Items[0].Transcript;
            }
          
          

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
