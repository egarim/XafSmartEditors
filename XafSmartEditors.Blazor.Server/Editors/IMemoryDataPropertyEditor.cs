namespace XafSmartEditors.Blazor.Server.Editors
{


    using DevExpress.ExpressApp;

    using DevExpress.ExpressApp.Blazor.Components.Models;
    using DevExpress.ExpressApp.Blazor.Editors;
    using DevExpress.ExpressApp.Editors;
    using DevExpress.ExpressApp.Model;

    using Microsoft.AspNetCore.Components;
    using XafSmartEditors.Razor.MemoryChat;

    [PropertyEditor(typeof(IMemoryData), true)]
    public class IMemoryDataPropertyEditor : BlazorPropertyEditorBase, IComplexViewItem
    {
        public IMemoryDataPropertyEditor(Type objectType, IModelMemberViewItem model) : base(objectType, model)
        {
        }

        IObjectSpace _objectSpace;
        XafApplication _application;

        public void Setup(IObjectSpace objectSpace, XafApplication application)
        {
            _objectSpace = objectSpace;
            _application = application;
        }


        public override MemoryDataComponentModel ComponentModel => (MemoryDataComponentModel)base.ComponentModel;

        protected override IComponentModel CreateComponentModel()
        {
            var model = new MemoryDataComponentModel();

            model.ValueChanged = EventCallback.Factory
                .Create<IMemoryData>(
                    this,
                    value =>
                    {
                        model.Value = value;
                        OnControlValueChanged();
                        WriteValue();
                    });
            return model;
        }

        protected override void ReadValueCore()
        {
            base.ReadValueCore();
            ComponentModel.Value = (IMemoryData)PropertyValue;
        }

        protected override object GetControlValueCore() => ComponentModel.Value;

        protected override void ApplyReadOnly()
        {
            base.ApplyReadOnly();
            ComponentModel?.SetAttribute("readonly", !AllowEdit);
        }
    }

}
