using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XafSmartEditors.Module.BusinessObjects
{
    using DevExpress.ExpressApp.ConditionalAppearance;
    using DevExpress.Persistent.Base;
    using DevExpress.Persistent.BaseImpl;
    using DevExpress.Persistent.Validation;
    using DevExpress.Xpo;
    using System;

    [DefaultClassOptions]
    public class CustomerForDiagnostics : BaseObject
    {
        public CustomerForDiagnostics(Session session) : base(session) { }

        private string customerID;
        [RuleRequiredField("RuleRequiredField for Customer.CustomerID", DefaultContexts.Save, "Customer ID cannot be empty")]
        [Appearance("CustomerIDAppearance", Enabled = false)]
        public string CustomerID
        {
            get => customerID;
            set => SetPropertyValue(nameof(CustomerID), ref customerID, value);
        }

        private string name;
        [RuleRequiredField("RuleRequiredField for Customer.Name", DefaultContexts.Save, "Name cannot be empty")]
        [Appearance("NameAppearance", Criteria = "Len(Name) < 5", BackColor = "Red", FontColor = "White", FontStyle = DevExpress.Drawing.DXFontStyle.Bold)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        private string street;
        [RuleRequiredField("RuleRequiredField for Customer.Street", DefaultContexts.Save, "Street cannot be empty")]
        [Appearance("StreetAppearance", Criteria = "Len(Street) < 3", BackColor = "Yellow", FontColor = "Black")]
        public string Street
        {
            get => street;
            set => SetPropertyValue(nameof(Street), ref street, value);
        }

        private string city;
        [RuleRequiredField("RuleRequiredField for Customer.City", DefaultContexts.Save, "City cannot be empty")]
        [Appearance("CityAppearance", Criteria = "Len(City) < 2", BackColor = "Yellow", FontColor = "Black")]
        public string City
        {
            get => city;
            set => SetPropertyValue(nameof(City), ref city, value);
        }

        private string state;
        [RuleRequiredField("RuleRequiredField for Customer.State", DefaultContexts.Save, "State cannot be empty")]
        [Appearance("StateAppearance", Criteria = "Len(State) < 2", BackColor = "Yellow", FontColor = "Black")]
        public string State
        {
            get => state;
            set => SetPropertyValue(nameof(State), ref state, value);
        }

        private string zipcode;
        [RuleRequiredField("RuleRequiredField for Customer.Zipcode", DefaultContexts.Save, "Zipcode cannot be empty")]
        [Appearance("ZipcodeAppearance", Criteria = "Len(Zipcode) < 5", BackColor = "Yellow", FontColor = "Black")]
        public string Zipcode
        {
            get => zipcode;
            set => SetPropertyValue(nameof(Zipcode), ref zipcode, value);
        }

        private string country;
        [RuleRequiredField("RuleRequiredField for Customer.Country", DefaultContexts.Save, "Country cannot be empty")]
        [Appearance("CountryAppearance", Criteria = "Len(Country) < 3", BackColor = "Yellow", FontColor = "Black")]
        public string Country
        {
            get => country;
            set => SetPropertyValue(nameof(Country), ref country, value);
        }
    }

}
