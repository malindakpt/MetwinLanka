using BusinessObjects;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class TransferAccountModel
    {
        [Required]
        [Display(Name="From Account")]
        public int FromAccountId { get; set; }
        [Display(Name = "To Account")]
        [Required]
        public int ToAccountId { get; set; }
        [Required]
        public string Description { get; set; }

        public string Reference { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        [Required]
        [Range(0,1e15,ErrorMessage="Amount must be positive")]
        public decimal Amount { get; set; }

        public Invoice ToInvoice (Invoice.TansactionTypes transType)
        {
            var invoice = new Invoice(transType);
            invoice.Description = Description;
            invoice.Status = Invoice.InvoiceStatus.Approved;
            invoice.Time = Time;

            PaymentReceipt receipt = new PaymentReceipt
            {
                Amount = Amount,
                Description = Description,
                RefNo = Reference,
                PayedInstance = new PaymentInstance(),
                PaymentMethod = PaymentReceipt.PaymentType.Transfer,
                Status = PaymentReceipt.PaymentStatus.Accepted,
                Time = Time,
                TransferFromId = FromAccountId,
                TransferToId = ToAccountId

            };

            invoice.PaymentReceipts.Add(receipt);

            return invoice;
        }


        public bool IsValid(out string errorMsg)
        {
            errorMsg = "";
            if(FromAccountId == ToAccountId)
            {
                errorMsg = "From and To accounts must be different";
                return false;
            }

            return true;
        }

    }

    public class MassTransferItem
    {
        [Required]
        public int AccountId { get; set; }

        [Range(0,1e12)]
        public decimal? CreditAmount { get; set; }

        [Range(0, 1e12)]
        public decimal? DebitAmount { get; set; }
    }

    public class MassTransferModel
    {
        [Required]
        public string Description { get; set; }

        public string Reference { get; set; }

        [Required]
        [DataType(DataType.Time)]
        public DateTime Time { get; set; }

        public IEnumerable<MassTransferItem> Transfers { get; set; }

        public MassTransferModel()
        {
            Transfers = new List<MassTransferItem>();
        }

        public bool IsValidTransfer(out string errorMessage)
        {
            errorMessage = null;

            decimal debitTotal = 0, creditTotal = 0;

            //check both debit and credit in single entry
            foreach (var item in Transfers)
            {
                if(item.CreditAmount.GetValueOrDefault() > 0 && item.DebitAmount.GetValueOrDefault() > 0)
                {
                    errorMessage = "Both credit amount and debit amount has been specified for the same account";
                    return false;
                }

                debitTotal += item.DebitAmount.GetValueOrDefault();
                creditTotal += item.CreditAmount.GetValueOrDefault();
            }

            if(debitTotal != creditTotal)
            {
                errorMessage = string.Format("Credit and debit totals are not equal. {0} and {1}", creditTotal, debitTotal);
                return false;
            }

            if(debitTotal == 0)
            {
                errorMessage = "No transfer amounts specified";
                return false;
            }

            return true;
        }

        public Invoice ToInvoice(Invoice.TansactionTypes transType = Invoice.TansactionTypes.GeneralTransfer)
        {
            string msg;
            if(!IsValidTransfer(out msg))
            {
                throw new Exception("Invalid transfer data");
            }

            var invoice = new Invoice(transType);
            invoice.Description = Description;
            invoice.Status = Invoice.InvoiceStatus.Approved;
            invoice.Time = Time;


            PaymentInstance instance = new PaymentInstance();

            var transfersToAdd = Transfers.Where(tr => tr.CreditAmount.GetValueOrDefault() > 0
                                                || tr.DebitAmount.GetValueOrDefault() > 0)
                                        .Where(tr => tr.AccountId != Account.CapitalAccount);
            foreach (var item in transfersToAdd)
            {
                PaymentReceipt receipt = new PaymentReceipt
                {
                    Description = Description,
                    RefNo = Reference,
                    PayedInstance = instance,
                    PaymentMethod = PaymentReceipt.PaymentType.Transfer,
                    Status = PaymentReceipt.PaymentStatus.Accepted,
                    Time = Time
                };

                if(item.CreditAmount.GetValueOrDefault() > 0)
                {
                    receipt.TransferFromId = item.AccountId;
                    receipt.TransferToId = Account.CapitalAccount;
                    receipt.Amount = item.CreditAmount.Value;
                }
                else
                {
                    receipt.TransferFromId = Account.CapitalAccount;
                    receipt.TransferToId = item.AccountId;
                    receipt.Amount = item.DebitAmount.Value;
                }
                invoice.PaymentReceipts.Add(receipt);
            }

            return invoice;
        }
    }
}