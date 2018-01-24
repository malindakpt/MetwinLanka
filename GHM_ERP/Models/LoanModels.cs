using BusinessObjects;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class LoanCreateModel
    {
        [Required]
        [Display(Name="Loan Name")]
        public string Name { get; set; }
        [Required]
        [Range(1,1e15,ErrorMessage ="Invalid Amount")]
        public decimal Amount { get; set; }

        [Required]
        public string Description { get; set; }

        public string Reference { get; set; }

        [Required]
        [Display(Name="Date")]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required]
        [Display(Name="Transferred To")]
        public int ToAccountId { get; set; }

        public Invoice ToInvoice(int loanAccountId)
        {
            Invoice invoice = new Invoice(Invoice.TansactionTypes.LoanCreation)
            {
                Description = Description,
                Status = Invoice.InvoiceStatus.Approved,
                Time = Time,
                RelatedAccountId = loanAccountId
            };

            var paymentReceipt = new PaymentReceipt
            {
                Amount = Amount,
                PayedInstance = new PaymentInstance(),
                RefNo = Reference,
                Description = Description,
                PaymentMethod = PaymentReceipt.PaymentType.Transfer,
                Side = PaymentReceipt.Sides.Other,
                Status = PaymentReceipt.PaymentStatus.Accepted,
                Time = Time,
                TransferFromId = loanAccountId,
                TransferToId = ToAccountId
            };

            invoice.PaymentReceipts.Add(paymentReceipt);

            return invoice;
        }

    }
}