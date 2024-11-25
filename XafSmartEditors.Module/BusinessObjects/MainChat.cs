using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafSmartEditors.Module.BusinessObjects
{
    using DevExpress.Data.Filtering;
    using DevExpress.ExpressApp;
    using DevExpress.ExpressApp.DC;
    using DevExpress.ExpressApp.Model;
    using DevExpress.Persistent.Base;
    using DevExpress.Persistent.BaseImpl;
    using DevExpress.Persistent.Validation;
    using DevExpress.Xpo;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;

    namespace XafSmartEditors.Module.BusinessObjects
    {

        [DefaultClassOptions]
        [RuleObjectExists("AnotherSingletonExists", DefaultContexts.Save, "True", InvertResult = true,
        CustomMessageTemplate = "Another Singleton already exists.")]
        [RuleCriteria("CannotDeleteSingleton", DefaultContexts.Delete, "False",
        CustomMessageTemplate = "Cannot delete Singleton.")]
        public class MainChat : BaseObject
        {
            public MainChat(Session session) : base(session)
            { }


            public override void AfterConstruction()
            {
                base.AfterConstruction();

            }

            string name;

            [Size(SizeAttribute.DefaultStringMappingFieldSize)]
            public string Name
            {
                get => name;
                set => SetPropertyValue(nameof(Name), ref name, value);
            }
        }
   
    }
}
