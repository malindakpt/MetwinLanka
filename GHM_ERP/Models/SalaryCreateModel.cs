using BusinessObjects;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{

    public class SalaryExpenseItem
    {
        [Required]
        public int AccountId { get; set; }

        [Range(0, 1e15, ErrorMessage = "Amount must be positive")]
        public decimal? Amount { get; set; }

        public string Description { get; set; }
    }


    public class SalaryCreateModel
    {
        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }
        
        [Required]
        [StringLength(150)]
        public string Description { get; set; }

        [Required]
        [Range(1, 1e15, ErrorMessage = "Amount must be positive")]
        public decimal TotalAmount { get; set; }

        //public string Reference { get; set; }


        public List<SalaryExpenseItem> Items { get; set; }

        public SalaryCreateModel()
        {
            Items = new List<SalaryExpenseItem>();
        }


        public bool IsValid(out string errMsg)
        {
            errMsg = "";
            var validItems = Items.Where(item => item.Amount.HasValue);
            if(TotalAmount != validItems.Sum(item => item.Amount))
            {
                errMsg = "Totals doens't match";
                return false;
            }
            return true;
        }


        public Invoice ToInvoice()
        {
            Invoice invoice = new Invoice(Invoice.TansactionTypes.SalaryTransfer)
            {
                Description = Description,
                Status = Invoice.InvoiceStatus.Approved,
                Time = Time
            };
            var validItems = Items.Where(item => item.Amount.HasValue);

            PaymentInstance  paymentInstance = new PaymentInstance{ Time =DateTime.Now};
            foreach (var item in validItems)
            {
                var paymentReceipt = new PaymentReceipt
                {
                    Amount = item.Amount.Value,
                    Side = PaymentReceipt.Sides.Other,
                    Description = item.Description,
                    PayedInstance = paymentInstance,
                    PaymentMethod = PaymentReceipt.PaymentType.Transfer,
                    Status = PaymentReceipt.PaymentStatus.Accepted,
                    Time = Time,
                    TransferToId = item.AccountId
                };

                invoice.PaymentReceipts.Add(paymentReceipt);
            }

            return invoice;
        }
    }
}