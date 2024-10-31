using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Validation.DiagnosticViews;
using DevExpress.Persistent.Validation;
using DevExpress.XtraReports.Design;
using DocumentFormat.OpenXml.Office2021.DocumentTasks;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel;
using NqlDotNet;
using OpenAI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XafSmartEditors.Module.BusinessObjects;
using XafSmartEditors.Module.BusinessObjects.Xpo;
using XafSmartEditors.Razor.NqlDotNet;
using DevExpress.ExpressApp.Utils;
using DevExpress.ExpressApp.Validation;

namespace XafSmartEditors.Module.Controllers
{
    [DomainComponent]
    public class MyDialog
    {
        public MyDialog(string message)
        {
            this.Message = message;
        }
        [ModelDefault("RowCount", "10")]
        public string Message { get; private set; }
    }
    public class XafViewDescriptionController : ViewController
    {
        SimpleAction Describe;
    
        IChatCompletionService chatService;
        public XafViewDescriptionController() : base()
        {
            Describe  = new SimpleAction(this, "Diagnostics info in natural language", "View");
            Describe.Execute += Describe_Execute;
            


        }
        protected virtual void Describe_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            //HACK model interfaces https://docs.devexpress.com/eXpressAppFramework/403535/ui-construction/application-model-ui-settings-storage/how-application-model-works/application-model-interfaces
            var RulesDiagnosticInfo = Frame.GetController<ShowRulesController>().GetDiagnosticInfoObjectString();
            var ValidationService = DevExpress.Persistent.Validation.Validator.GetService(this.Application.ServiceProvider);
            List<IRule> rules = ValidationService.RegisteredRules.ToList();



           var ValidationModelNode=  this.View.Model.Application as DevExpress.ExpressApp.Validation.IModelApplicationValidation;

            for (int i = 0; i < ValidationModelNode.Validation.Rules.NodeCount; i++)
            {
                var node = ValidationModelNode.Validation.Rules.GetNode(i);

                var TargetType= node.GetValue<Type>("TargetType");

                if(this.View.CurrentObject.GetType() == TargetType)
                {
                    Debug.WriteLine("TargetType Matched");
                    var Diff = UserDifferencesHelper.GetUserDifferences(node);
                    foreach (var item in Diff)
                    {

                        Debug.WriteLine(item.Value);
                    }
                    for (int n = 0; i < node.NodeCount; n++)
                    {
                        var Diff2 = UserDifferencesHelper.GetUserDifferences(node.GetNode(i));
                        foreach (var item in Diff2)
                        {

                            Debug.WriteLine(item.Value);
                        }
                    }
                }
               


            }

          
            //ValidationModelNode.Validation.Rules.ToList().ForEach(x =>
            //{
            //    Debug.WriteLine(x.GetType());
            //    Debug.WriteLine(x.ToString());
            //});
            //rules.ForEach(x =>
            //{
            //    Debug.WriteLine(x.Id);
            //    Debug.WriteLine(x.ToString());
            //});


            var tasks = new List<Func<Task<object>>>
            {
                async () => {

                    var clientOpenAi = new OpenAIClient(new System.ClientModel.ApiKeyCredential(Environment.GetEnvironmentVariable("OpenAiTestKey")));
                    var KernelBuilder = Kernel.CreateBuilder();
                    KernelBuilder.AddOpenAIChatCompletion("gpt-4o-mini", clientOpenAi);
                    var sk = KernelBuilder.Build();
                    chatService = sk.GetRequiredService<IChatCompletionService>();
                    var Nld= await chatService.GetChatMessageContentAsync("Describe the content the following xml in natural language, describe the data not the xml structure, describe the validation rules that apply"+ RulesDiagnosticInfo); 
                    string Answer =  Nld.ToString() +Environment.NewLine+$"Original XML"+Environment.NewLine+RulesDiagnosticInfo; 
                    return Answer;
                },

            };
            Action<int, string, object> onProgressChanged = (progress, status, result) =>
            {
                Debug.WriteLine($"{status} - Result: {result}");
            };
            var worker = new AsyncBackgroundWorker<object>(
              tasks,
              onProgressChanged,
              result => ProcessingDone(result)
          );
            worker.Start();
        }


        void ProcessingDone(Dictionary<int, object> results)
        {
            var Message= results[0].ToString();
            IObjectSpace objectSpace = Application.CreateObjectSpace(typeof(MyDialog));
            MyDialog myDialogObject = new MyDialog(Message);
            DetailView dialogView = Application.CreateDetailView(objectSpace, myDialogObject);
            Application.ShowViewStrategy.ShowViewInPopupWindow(dialogView,
                () => Application.ShowViewStrategy.ShowMessage("Done."),
                () => Application.ShowViewStrategy.ShowMessage("Cancelled."),
                null, null, this.Frame
            );

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
