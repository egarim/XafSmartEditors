using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.DC;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;
using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace XafSmartEditors.Module.BusinessObjects
{
    [DefaultClassOptions]
    [DefaultProperty(nameof(DataTypeName))]
    [Appearance("Disable while processing", TargetItems = "*", Criteria = "IsProcessing", Enabled = false)]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://docs.devexpress.com/eXpressAppFramework/112701/business-model-design-orm/data-annotations-in-data-model).
    public class CriteriaObjectTest : BaseObject
    { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://docs.devexpress.com/eXpressAppFramework/113146/business-model-design-orm/business-model-design-with-xpo/base-persistent-classes).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public CriteriaObjectTest(Session session)
            : base(session)
        {
        }
        [Browsable(false)]
        public bool IsProcessing
        {
            get => isProcessing;
            set
            {
                if (isProcessing == value)
                    return;
                isProcessing = value;
                RaisePropertyChangedEvent(nameof(IsProcessing));
            }
        }
        
        public override void AfterConstruction()
        {
            base.AfterConstruction();
            // Place your initialization code here (https://docs.devexpress.com/eXpressAppFramework/112834/getting-started/in-depth-tutorial-winforms-webforms/business-model-design/initialize-a-property-after-creating-an-object-xpo?v=22.1).
        }
        bool isProcessing;
        string validationResult;
        bool isValid;
        string criteriaDescription;
        string generatedCriteria;
        string nql;
        private string dataTypeName;
        [Browsable(false)]
        public string DataTypeName
        {
            get { return dataTypeName; }
            set
            {
                Type type = XafTypesInfo.Instance.FindTypeInfo(value) == null ? null :
                    XafTypesInfo.Instance.FindTypeInfo(value).Type;
                if (dataType != type)
                {
                    dataType = type;
                }
                if (!IsLoading && value != dataTypeName)
                {
                    Criteria = String.Empty;

                }
                SetPropertyValue<string>(nameof(DataTypeName), ref dataTypeName, value);
            }
        }

        private Type dataType;
        [TypeConverter(typeof(LocalizedClassInfoTypeConverter))]
        [ImmediatePostData, NonPersistent]
        public Type DataType
        {
            get { return dataType; }
            set
            {
                if (dataType != value)
                {
                    dataType = value;
                    DataTypeName = (value == null) ? null : value.FullName;
                }
            }
        }

        private string criteria;
        [CriteriaOptions("DataType")]
        [Size(SizeAttribute.Unlimited)]
        [ModelDefault("RowCount", "0")]
        [VisibleInListView(true)]
        [EditorAlias(EditorAliases.CriteriaPropertyEditor)]
        public string Criteria
        {
            get { return criteria; }
            set { SetPropertyValue<string>(nameof(Criteria), ref criteria, value); }
        }


        [Size(SizeAttribute.Unlimited)]
        public string Nql
        {
            get => nql;
            set => SetPropertyValue(nameof(Nql), ref nql, value);
        }

        [Size(SizeAttribute.Unlimited)]
        public string GeneratedCriteria
        {
            get => generatedCriteria;
            set => SetPropertyValue(nameof(GeneratedCriteria), ref generatedCriteria, value);
        }

        [Size(SizeAttribute.Unlimited)]
        public string CriteriaDescription
        {
            get => criteriaDescription;
            set => SetPropertyValue(nameof(CriteriaDescription), ref criteriaDescription, value);
        }

        public bool IsValid
        {
            get => isValid;
            set => SetPropertyValue(nameof(IsValid), ref isValid, value);
        }
        
        [Size(SizeAttribute.Unlimited)]
        public string ValidationResult
        {
            get => validationResult;
            set => SetPropertyValue(nameof(ValidationResult), ref validationResult, value);
        }
    }
}