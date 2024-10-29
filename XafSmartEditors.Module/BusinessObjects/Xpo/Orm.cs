using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo;


namespace XafSmartEditors.Module.BusinessObjects.Xpo
{


    public class Customer : XPObject
    {
        public Customer(Session session) : base(session) { }




        string name;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Association("CustomerCategory-Customers")]
        public Xpo.CustomerCategory Category { get; set; }

        [Association("Address-Customers")]
        public Xpo.Address Address { get; set; }
    }
    public class Address : XPObject
    {
        public Address(Session session) : base(session) { }




        string country;
        string zipcode;
        string state;
        string city;
        string street;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Street
        {
            get => street;
            set => SetPropertyValue(nameof(Street), ref street, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string City
        {
            get => city;
            set => SetPropertyValue(nameof(City), ref city, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string State
        {
            get => state;
            set => SetPropertyValue(nameof(State), ref state, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Zipcode
        {
            get => zipcode;
            set => SetPropertyValue(nameof(Zipcode), ref zipcode, value);
        }
        
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Country
        {
            get => country;
            set => SetPropertyValue(nameof(Country), ref country, value);
        }

        [Association("Address-Customers")]
        public XPCollection<Xpo.Customer> Customers => GetCollection<Xpo.Customer>();
    }

    public class InvoiceHeader : XPObject
    {
        public InvoiceHeader(Session session) : base(session) { }



        //[Association("Customer-InvoiceHeaders")]
        
        public Customer Customer
        {
            get => customer;
            set => SetPropertyValue(nameof(Customer), ref customer, value);
        }


        public DateTime InvoiceDate
        {
            get => invoiceDate;
            set => SetPropertyValue(nameof(InvoiceDate), ref invoiceDate, value);
        }


        public DateTime DueDate
        {
            get => dueDate;
            set => SetPropertyValue(nameof(DueDate), ref dueDate, value);
        }

        Customer customer;
    DateTime invoiceDate;
      DateTime dueDate;
        decimal totalAmount;

        public decimal TotalAmount
        {
            get => totalAmount;
            set => SetPropertyValue(nameof(TotalAmount), ref totalAmount, value);
        }

        [Association("InvoiceHeader-InvoiceDetails")]
        public XPCollection<Xpo.InvoiceDetails> InvoiceDetails => GetCollection<Xpo.InvoiceDetails>();
    }

    public class InvoiceDetails : XPObject
    {
        public InvoiceDetails(Session session) : base(session) { }



        InvoiceHeader invoiceHeader;
        [Association("InvoiceHeader-InvoiceDetails")]
        
        public InvoiceHeader InvoiceHeader
        {
            get => invoiceHeader;
            set => SetPropertyValue(nameof(InvoiceHeader), ref invoiceHeader, value);
        }

        Product product;
        [Association("Product-InvoiceDetails")]
        public Product Product
        {
            get => product;
            set => SetPropertyValue(nameof(Product), ref product, value);
        }

        decimal totalPrice;
        decimal unitPrice;
        int quantity;

        public int Quantity
        {
            get => quantity;
            set => SetPropertyValue(nameof(Quantity), ref quantity, value);
        }

        public decimal UnitPrice
        {
            get => unitPrice;
            set => SetPropertyValue(nameof(UnitPrice), ref unitPrice, value);
        }
        
        public decimal TotalPrice
        {
            get => totalPrice;
            set => SetPropertyValue(nameof(TotalPrice), ref totalPrice, value);
        }
    }
    public class Product : XPObject
    {
        public Product(Session session) : base(session) { }




        string name;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Association("ProductCategory-Products")]
        public Xpo.ProductCategory Category { get; set; }

        [Association("PriceList-Products")]
        public Xpo.PriceList PriceList { get; set; }

        [Association("Product-InvoiceDetails")]
        public XPCollection<Xpo.InvoiceDetails> InvoiceDetails => GetCollection<Xpo.InvoiceDetails>();
    }
    public class CustomerCategory : XPObject
    {
        public CustomerCategory(Session session) : base(session) { }




        string categoryName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CategoryName
        {
            get => categoryName;
            set => SetPropertyValue(nameof(CategoryName), ref categoryName, value);
        }

        [Association("CustomerCategory-Customers")]
        public XPCollection<Xpo.Customer> Customers => GetCollection<Xpo.Customer>();
    }
    public class ProductCategory : XPObject
    {
        public ProductCategory(Session session) : base(session) { }




        string categoryName;

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string CategoryName
        {
            get => categoryName;
            set => SetPropertyValue(nameof(CategoryName), ref categoryName, value);
        }

        [Association("ProductCategory-Products")]
        public XPCollection<Xpo.Product> Products => GetCollection<Xpo.Product>();
    }
    public class PriceList : XPObject
    {
        public PriceList(Session session) : base(session) { }

      

        [Association("PriceList-Products")]
        public XPCollection<Xpo.Product> Products => GetCollection<Xpo.Product>();


        DateTime effectiveDate;
        double price;

        public double Price
        {
            get => price;
            set => SetPropertyValue(nameof(Price), ref price, value);
        }
        
        public DateTime EffectiveDate
        {
            get => effectiveDate;
            set => SetPropertyValue(nameof(EffectiveDate), ref effectiveDate, value);
        }
    }
}