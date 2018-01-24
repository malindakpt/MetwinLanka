using BusinessObjects;
using BusinessObjects.Categories;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class ClientViewModel
    {
        [Display(Name ="Client")]
        public int ClientID { get; set; }
        public string Name { get; set; }
        public string Location{ get; set; }
        public decimal Credits { get; set; }

    }


    public class PurchaseViewModel
    {
        public class PurchaseItem
        {
            public int Id { get; set; }
            [Range(1, 100000, ErrorMessage = "Invalid value for quantity")]
            public decimal Qty { get; set; }
            [Range(0,1000000000,ErrorMessage="Invalid value for price")]
            public decimal Price { get; set; }
            [MaxLength(100)]
            public string Description { get; set; }
        }


        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Display(Name="Client")]
        [Required]
        public int ClientId { get; set; }
        [Required]
        public string Description { get; set; }

        public List<PurchaseItem> Items { get; set; }

        

        public PurchaseViewModel()
        {
            Items = new List<PurchaseItem>();
            
        }

        public bool IsValid(out string errorMsg)
        {
            errorMsg = "";
            if(ClientId <= 0)
            {
                errorMsg = "Invalid supplier. Please select a valid supplier.";
                return false;
            }

            return true;
        }


        public Invoice ToInvoice(IEnumerable<RawMaterialProfile> rawMaterialProfiles)
        {
            var invoice = new Invoice
            {
                Status = Invoice.InvoiceStatus.Approved,
                TransType = Invoice.TansactionTypes.Purchases
            };

            invoice.ClientId = this.ClientId;
            invoice.Description = this.Description;
            invoice.Items = this.Items.Select(i => new Item
            {
                Qty = i.Qty,
                Price = i.Price,
                 ProfileId = i.Id,
                 Profile = rawMaterialProfiles.First(rm => rm.Id == i.Id),
                 Description = i.Description
            }
                ).ToList();

            invoice.Time = this.Time;
            var total = invoice.Items.Sum(i => i.Price);
            var payment = new PaymentReceipt
            {
                Amount = total,
                Description = "Credit",
                PaymentMethod = PaymentReceipt.PaymentType.Credit,
                Side = PaymentReceipt.Sides.Sent,
                Time = invoice.Time,
                PayedInstance = new PaymentInstance {  Time = DateTime.Now}
                 
            };
            invoice.PaymentReceipts.Add(payment);


            return invoice;
        }
    }




    public class SaleViewModel
    {
        public class SaleItem
        {
            public int Id { get; set; }
            [Range(1, 100000, ErrorMessage = "Invalid value for length")]
            public decimal Length { get; set; }
            [Range(1, 100000, ErrorMessage = "Invalid value for quantity")]
            public decimal Qty { get; set; }
            [Range(0, 1000000000, ErrorMessage = "Invalid value for price")]
            public decimal Price { get; set; }
            public int? SourceInvoiceId {get;set;}
        }


        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString="{0:yyyy-MM-dd HH:mm}",ApplyFormatInEditMode=true)]
        public DateTime Time { get; set; }

        [Display(Name = "Client")]
        [Required]
        public int ClientId { get; set; }

        [Display(Name="Sales Rep")]
        public int? SalesRepId { get; set; }

        [Required]
        public string Description { get; set; }

        public List<SaleItem> Items { get; set; }

        public SaleViewModel()
        {
            Items = new List<SaleItem>();
        }
        public int? SalesOrderInvoiceId { get; set; }
        public bool IsValid(out string errorMsg)
        {
            errorMsg = "";
            if (ClientId <= 0)
            {
                errorMsg = "Invalid supplier. Please select a valid supplier.";
                return false;
            }

            return true;
        }


        public Invoice ToInvoice(IEnumerable<ProductProfile> profiles,decimal taxPercentage)
        {
            var invoice = new Invoice
            {
                Status = Invoice.InvoiceStatus.Approved,
                TransType = Invoice.TansactionTypes.Sales
            };

            invoice.ClientId = this.ClientId;
            invoice.Description = this.Description;
            invoice.SalesRepId = this.SalesRepId;
            invoice.Items = this.Items.Select(i => new Item
            {
                Qty = i.Qty,
                Length = i.Length,
                Price = i.Price,
                 SourceRefId = i.SourceInvoiceId,
                  ProfileId = i.Id,
                   Profile = profiles.First(p=> p.Id == i.Id)
            }
                ).ToList();

            invoice.Time = this.Time;

            var total = invoice.Items.Sum(i => i.Price);
            var salesAmount = total / (1 + taxPercentage);
            salesAmount = decimal.Round(salesAmount, 2);
            var taxAmount = total - salesAmount;
            var PayedInstance = new PaymentInstance { Time = DateTime.Now };
            var payment = new PaymentReceipt
            {
                Amount = salesAmount,
                Description = "Credit",
                PaymentMethod = PaymentReceipt.PaymentType.Credit,
                Side = PaymentReceipt.Sides.Received,
                Time = invoice.Time,
                PayedInstance = PayedInstance
            };
            var taxPayment = new PaymentReceipt
            {
                Amount = taxAmount,
                IsTaxReceipt = true,
                PaymentMethod = PaymentReceipt.PaymentType.Credit,
                Side = PaymentReceipt.Sides.Received,
                Time = invoice.Time,
                PayedInstance = PayedInstance
            };
            invoice.PaymentReceipts.Add(payment);
            invoice.PaymentReceipts.Add(taxPayment);

            if (SalesOrderInvoiceId.HasValue)
            {
                invoice.RelatedInvoiceId = SalesOrderInvoiceId;
            }
            return invoice;
        }
    }
}