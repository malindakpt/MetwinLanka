using BusinessObjects;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class PushCapitalModel
    {
        public enum PaymentMethod
        {
            Cash, Cheque
        }

        [Required]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Payment Method")]
        public PaymentMethod Method { get; set; }

        [Required]
        [Range(1, 1e15, ErrorMessage = "Amount must be positive")]
        public decimal Amount { get; set; }


        [StringLength(50)]
        [Display(Name = "Reference")]
        public string ReferenceNo { get; set; }

        /* Cheque Attributes */
        [Display(Name = "Bank")]
        public string ChequeBank { get; set; }

        [Display(Name = "Cheque No.")]
        public string ChequeNumber { get; set; }

        [Display(Name = "Settle Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ChequeSettleDate { get; set; }


        public virtual Invoice ToInvoice()
        {
            var invoice = new Invoice
            {
                Status = Invoice.InvoiceStatus.Approved,
                TransType = Invoice.TansactionTypes.CapitalDeposite
            };

            invoice.Description = this.Description;

            invoice.Time = this.Time;
          
            Cheque cheque = null;
            if (Method == PaymentMethod.Cheque)
            {
                cheque = new Cheque
                {
                    Bank =  ChequeBank,
                    ChequeNo = ChequeNumber,
                    SettleDate = ChequeSettleDate,
                    ChequeSide = Cheque.Side.Received,
                    HandedOverDate = Time,
                    Status = Cheque.ChequeStatus.Pending,
                    Amount = this.Amount
                };
            }

            var payment = new PaymentReceipt
            {
                Amount = this.Amount,
                Description = this.Description,
                PaymentMethod = PaymentReceipt.PaymentType.Cash,
                Side = PaymentReceipt.Sides.Received,
                Time = invoice.Time,
                ChequeRef = cheque,
                 RefNo = ReferenceNo,
                PayedInstance = new PaymentInstance { Time = DateTime.Now }
            };
            if(Method == PaymentMethod.Cheque)
            {
                payment.PaymentMethod = PaymentReceipt.PaymentType.Cheque;
            }

            invoice.PaymentReceipts.Add(payment);

            return invoice;
        }
    }


    public class PopCapitalModel : PushCapitalModel
    {
        public int BankAccountId { get; set; }

        public override Invoice ToInvoice()
        {
            var invoice = new Invoice
            {
                Status = Invoice.InvoiceStatus.Approved,
                TransType = Invoice.TansactionTypes.CapitalWithdraw
            };

            invoice.Description = this.Description;

            invoice.Time = this.Time;

            Cheque cheque = null;
            if (Method == PaymentMethod.Cheque)
            {
                cheque = new Cheque
                {
                    Bank = "<Bank Account " +BankAccountId + ">",
                    ChequeNo = ChequeNumber,
                    SettleDate = ChequeSettleDate,
                    ChequeSide = Cheque.Side.Paid,
                    HandedOverDate = Time,
                    Status = Cheque.ChequeStatus.Pending,
                    Amount = this.Amount
                };
            }

            var payment = new PaymentReceipt
            {
                Amount = this.Amount,
                Description = this.Description,
                PaymentMethod = PaymentReceipt.PaymentType.Cash,
                Side = PaymentReceipt.Sides.Sent,
                Time = invoice.Time,
                ChequeRef = cheque,
                RefNo = ReferenceNo,
                PayedInstance = new PaymentInstance { Time = DateTime.Now },
                 BankAccountId = BankAccountId
            };
            if (Method == PaymentMethod.Cheque)
            {
                payment.PaymentMethod = PaymentReceipt.PaymentType.Cheque;
            }

            invoice.PaymentReceipts.Add(payment);

            return invoice;
        }
    }

}