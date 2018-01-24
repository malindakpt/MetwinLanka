using BusinessObjects;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    [NotMapped]
    public class PaymentDetails
    {
        public IDictionary<int, PaymentReceipt> PaymentReceiptsAdded { get; set; }
        public Invoice DepositInvoice { get; set; }
    }

    [NotMapped]
    public class PaymentModel
    {
        public enum PaymentMethod
        {
            Cash, Cheque, Deposit
        }

        public class InvoicePayment
        {
            public int InvoiceId { get; set; }
            public decimal? Payment { get; set; }
        }

        public PaymentModel()
        {
            PayedInvoices = new List<InvoicePayment>();
        }


        //[Required(ErrorMessage="Please select invoices to pay for")]
        public List<InvoicePayment> PayedInvoices { get; set; }

        [Required]
        [Display(Name="Date")]
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime Time { get; set; }


        [Required]
        [Display(Name="Client")]
        public int ClientId { get; set; }

        [Required]
        [Display(Name="Payment Method")]
        public PaymentMethod Method { get; set; }

        [Required]
        [Range(1,1e15,ErrorMessage="Amount must be positive")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(150)]
        public string Description { get; set; }

        [StringLength(50)]
        [Display(Name="Reference")]
        public string ReferenceNo { get; set; }

        /* Cheque Attributes */
        [Display(Name="Bank Account")]
        public int? ChequeBankAccount { get; set; }

        [Display(Name="Bank")]
        public string ChequeBank { get; set; }

        [Display(Name="Cheque No.")]
        public string ChequeNumber { get; set; }

        [Display(Name="Settle Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ChequeSettleDate { get; set; }

        //keep payment instance
        private PaymentInstance paymentInstance = null;

        private PaymentInstance GetPaymentInstance()
        {
            if(paymentInstance == null)
            {
                paymentInstance = new PaymentInstance { Time = DateTime.Now };   
            }
            return paymentInstance;
        }


        public bool IsValid(out string errorString)
        {
            errorString = "";

            if (Method == PaymentMethod.Cheque)
            {
                if((string.IsNullOrWhiteSpace(ChequeBank) && !ChequeBankAccount.HasValue) || string.IsNullOrWhiteSpace(ChequeNumber))
                {
                    errorString = "Cheque Information is missing";
                    return false;
                }
                if(ChequeSettleDate < new DateTime(2005,10,10) || ChequeSettleDate > DateTime.Now.AddYears(10))
                {
                    errorString = "Invalid Cheque settle date";
                    return false;
                }
            }

            var invalidPayment=PayedInvoices.FirstOrDefault(p => p.Payment.HasValue && p.Payment.Value < 0);
            if (invalidPayment != null )
            {
                errorString = "Invalid invoice payment -" + invalidPayment.Payment.Value;
                return false;
            }

            if (Amount < PayedInvoices.Sum(p => p.Payment))
            {
                errorString = "Invoice payments exceed the paid amount";
                return false;
            }
            if(Amount > PayedInvoices.Sum(p => p.Payment) && Method == PaymentMethod.Deposit)
            {
                errorString = "Payments from deposit must be completely allocated to invoices";
                return false;
            }

            return true;
        }


        public PaymentDetails GetPaymentSendDetails()
        {
            var outReceipts = new Dictionary<int,PaymentReceipt>();
            PaymentInstance paySession = GetPaymentInstance();

            Cheque cheque = null;
            if(Method == PaymentMethod.Cheque)
            {
                cheque = new Cheque
                {
                    Bank = ChequeBank,
                    ChequeNo = ChequeNumber,
                    SettleDate = ChequeSettleDate,
                    ChequeSide = Cheque.Side.Paid,
                    HandedOverDate = Time,
                    Status = Cheque.ChequeStatus.Pending,
                    Amount =Amount
                };
            }

            foreach (var invPayment in PayedInvoices.Where(p=> p.Payment > 0) )
            {
                PaymentReceipt receipt = new PaymentReceipt
                {
                    Amount = invPayment.Payment.Value,
                    InvoiceId = invPayment.InvoiceId,
                    PayedInstance = paySession,
                    Description = Description,
                    PaymentMethod = PaymentReceipt.PaymentType.Cash,
                    RefNo = ReferenceNo,
                    Time = Time,
                    Side = PaymentReceipt.Sides.Sent,
                    Status = PaymentReceipt.PaymentStatus.Accepted,
                    ChequeRef = cheque,
                    BankAccountId = ChequeBankAccount
                };

                if(Method == PaymentMethod.Cheque)
                {
                    receipt.PaymentMethod = PaymentReceipt.PaymentType.Cheque; 
                }
                else if (Method == PaymentMethod.Deposit)
                {
                    receipt.PaymentMethod = PaymentReceipt.PaymentType.FromDeposit;
                }

                outReceipts[invPayment.InvoiceId] = receipt;
            }

            var deposit = Amount - outReceipts.Sum(rc => rc.Value.Amount);
            Invoice depositInvoice = null;
            if(deposit > 0)
            {
                depositInvoice = new Invoice(Invoice.TansactionTypes.Deposit)
                {
                    ClientId = ClientId,
                    Description = Description,
                    Status = Invoice.InvoiceStatus.Approved,
                    Time = Time
                };

                PaymentReceipt receipt = new PaymentReceipt
                {
                    Amount = deposit,
                    PayedInstance = paySession,
                    Description = Description,
                    PaymentMethod = PaymentReceipt.PaymentType.Cash,
                    RefNo = ReferenceNo,
                    Time = Time,
                    Side = PaymentReceipt.Sides.Sent,
                    Status = PaymentReceipt.PaymentStatus.Accepted,
                    ChequeRef = cheque,
                    BankAccountId = ChequeBankAccount
                };
                if (Method == PaymentMethod.Cheque)
                {
                    receipt.PaymentMethod = PaymentReceipt.PaymentType.Cheque;
                }


                depositInvoice.PaymentReceipts.Add(receipt);
            }


            var outDetails = new PaymentDetails
            {
                PaymentReceiptsAdded = outReceipts,
                DepositInvoice = depositInvoice
            };
            return outDetails;
        }

 

        public PaymentDetails GetPaymentRecvDetails()
        {
            var details = GetPaymentSendDetails();

            if(details.DepositInvoice != null && Method == PaymentMethod.Deposit)
            {
                throw new ArgumentException("Amount not allocated completely");
            }

            foreach(var additions in details.PaymentReceiptsAdded)
            {
                var receipt = additions.Value;
                receipt.Side = PaymentReceipt.Sides.Received;
                if(receipt.ChequeRef != null)
                {
                    receipt.ChequeRef.ChequeSide = Cheque.Side.Received;
                }
            }

            return details;
        }
    }


}