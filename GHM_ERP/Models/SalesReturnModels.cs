using BusinessObjects;
using BusinessObjects.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class ReturnItem
    {
        [Required]
        public int ItemId { get; set; }
        [Required]
        [Range(0,1e15,ErrorMessage="Must be non-negative")]
        public decimal ReturnQty { get; set; }
    }
    public class SalesReturnModel
    {
        [Required]
        public int InvoiceId { get; set; }

        [Required]
        public List<ReturnItem> ReturnedItems { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Description { get; set; }


        public SalesReturnModel()
        {
            ReturnedItems = new List<ReturnItem>();
        }


        //=========== Helper methods =====

        public bool IsValid()
        {
            if(ReturnedItems.Any(it => it.ItemId <= 0) || ReturnedItems.Any(it => it.ReturnQty < 0))
            {
                return false;
            }
            if(ReturnedItems.All(it => it.ReturnQty  == 0))
            {
                return false;
            }
            return true;
        }

        public Invoice ToInvoice(Invoice originalInvoice, IEnumerable<ProductProfile> profiles)
        {
            var invoice = new Invoice(Invoice.TansactionTypes.SalesReturn)
            {
                ClientId = originalInvoice.ClientId,
                CreationTime = DateTime.Now,
                Status = Invoice.InvoiceStatus.Approved,
                Time = this.Date,
                RelatedInvoiceId = this.InvoiceId,
                Description = Description
            };

            foreach (var item in ReturnedItems)
            {
                var originalItem = originalInvoice.Items.FirstOrDefault(it => it.Id == item.ItemId);
                if(item.ReturnQty > 0)
                {
                    var price = item.ReturnQty * originalItem.UnitPrice * originalItem.Length;
                    var newItem = new Item
                    {
                        Length = originalItem.Length,
                        ProfileId = originalItem.ProfileId,
                        Profile = profiles.FirstOrDefault(prof => prof.Id == originalItem.ProfileId),
                        SourceRefId = originalItem.SourceRefId,
                        Price = price,
                        Qty = item.ReturnQty
                    };

                    invoice.Items.Add(newItem);

                    if(originalItem.Qty < item.ReturnQty)
                    {
                        throw new Exception("Returned Qty is larger than purchase qty");
                    }
                }
            }

            invoice.PaymentReceipts.Add(new PaymentReceipt
            {
                Amount = invoice.Items.Sum(i => i.Price),
                PayedInstance = new BusinessObjects.Invoicing.PaymentInstance { Time = DateTime.Now },
                PaymentMethod = PaymentReceipt.PaymentType.Credit,
                Side = PaymentReceipt.Sides.Sent,
                Status = PaymentReceipt.PaymentStatus.Accepted,
                Time = this.Date
            });

            return invoice;
        }
    }
}