using BusinessObjects;
using BusinessObjects.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class SalesOrderViewModel
    {
        public class SaleItem
        {
            public int Id { get; set; }
            [Range(1, 100000, ErrorMessage = "Invalid value for quantity")]
            public decimal Qty { get; set; }
            [Range(1, 100000, ErrorMessage = "Invalid value for Length")]
            public decimal Length { get; set; }
            [Range(0, 1000000000, ErrorMessage = "Invalid value for price")]
            public decimal Price { get; set; }
        }


        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Display(Name = "Client")]
        [Required]
        public int ClientId { get; set; }

        [Display(Name = "Sales Rep")]
        public int? SalesRepId { get; set; }

        [Required]
        public string Description { get; set; }

        public List<SaleItem> Items { get; set; }

        public SalesOrderViewModel()
        {
            Items = new List<SaleItem>();
        }


        public Invoice ToInvoice(IEnumerable<ProductProfile> profiles)
        {
            var invoice = new Invoice
            {
                Status = Invoice.InvoiceStatus.Order,
                TransType = Invoice.TansactionTypes.Sales
            };

            invoice.ClientId = this.ClientId;
            invoice.Description = this.Description;
            invoice.SalesRepId = this.SalesRepId;
            invoice.Items = this.Items.Select(i => new Item
            {
                Qty = i.Qty,
                Price = i.Price,
                ProfileId = i.Id,
                Profile = profiles.First(p => p.Id == i.Id),
                 Length = i.Length
            }
                ).ToList();

            invoice.Time = this.Time;

            return invoice;
        }
    }
}