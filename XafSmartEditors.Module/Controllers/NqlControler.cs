﻿using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;
using MongoDB.Bson.IO;
using NqlDotNet;
using NqlDotNet.DevExpress;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XafSmartEditors.Module.BusinessObjects;
using XafSmartEditors.Razor.NqlDotNet;

namespace XafSmartEditors.Module.Controllers
{
    public class NqlControler : ViewController
    {
        SimpleAction action;
        public NqlControler() : base()
        {
            // Target required Views (use the TargetXXX properties) and create their Actions.
            action = new SimpleAction(this, "Generate Criteria", "View");
            action.Execute += action_Execute;
            
        }
        void ProcessingDone(Dictionary<int,object> results)
        {
            CriteriaResult criteriaResult = (results[0] as CriteriaResult);
            var Cot = this.View.CurrentObject as BusinessObjects.CriteriaObjectTest;
            Cot.DataTypeName = "XafSmartEditors.Module.BusinessObjects.Xpo" + "." + criteriaResult.RootEntity;



            //Cot.Criteria = Criteria.Criteria;
           
            Cot.GeneratedCriteria = criteriaResult.Criteria;
            var Parsed = CriteriaOperator.Parse(Cot.GeneratedCriteria);
            Cot.Criteria = Parsed.ToString();
            //Cot.CriteriaDescription = await nqlService.CriteriaToNl(Criteria.Criteria, Schema, Doc);


            var CriteriaObjectTestInstance = XafTypesInfo.Instance.FindTypeInfo(typeof(CriteriaObjectTest));
            var criteriaMember = CriteriaObjectTestInstance.FindMember(nameof(CriteriaObjectTest.Criteria));
            var helper = new CriteriaPropertyEditorHelper(criteriaMember);
            Type type = XafTypesInfo.Instance.FindTypeInfo(Cot.DataTypeName).Type;
            var Os = this.Application.CreateObjectSpace<CriteriaObjectTest>();
            var Instance = Os.CreateObject(type);
            string validationResult = "";
            Cot.IsValid = helper.ValidateCriteria(Instance, Cot.Criteria, out validationResult);
            Cot.ValidationResult = validationResult;
            Cot.IsProcessing= false;
            //Thread.Sleep(2000);
            this.View.ObjectSpace.CommitChanges();
        }

        private async void action_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var Cot= this.View.CurrentObject as BusinessObjects.CriteriaObjectTest;
            string Doc = EmbeddedResourceHelper.ReadEmbeddedResource("DevExpressCriteriaSyntax.txt", "XafSmartEditors.Module.Data", this.GetType());
            Cot.IsProcessing = true;
            this.ObjectSpace.CommitChanges();

            var os=this.View.ObjectSpace as XPObjectSpace;
            var props = XpoUtilities.GetEntityProperties(this.GetType().Assembly,os.Session);
            string Schema = System.Text.Json.JsonSerializer.Serialize(props);
            string Nlq = Cot.Nql;

            DevExNqlService nqlService = new DevExNqlService();
            //var Criteria = await nqlService.NlToCriteria(Nlq, Schema, Doc);
            //var CriteriaTask = await nqlService.NlToCriteria(Nlq, Schema, Doc);


            var tasks = new List<Func<Task<CriteriaResult>>>
            {
                async () => { return await nqlService.NlToCriteria(Nlq, Schema, Doc); },
              
            };




            Action<int, string, CriteriaResult> onProgressChanged = (progress, status, result) => Console.WriteLine($"{status} - Result: {result}");
            var worker = new AsyncBackgroundWorker<CriteriaResult>(
                tasks,
                null,
                result => ProcessingDone(result)
            );

            worker.Start();


            // Execute your business logic (https://docs.devexpress.com/eXpressAppFramework/112737/).
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
