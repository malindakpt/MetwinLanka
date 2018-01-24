using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace BusinessObjects
{
    public class Invoice
    {
        public enum TansactionTypes
        {
            Purchases = 0,
            PurchasesReturn = 1,
            Sales = 2,
            SalesReturn = 3,
            Wastage = 4,

            Expenses = 10,
            Income = 11,

            Deposit = 30,

            CapitalDeposite = 40,
            CapitalWithdraw = 41,

            //Transfer-like
            Depreciation = 110,
            PayableTransfer = 111,
            BankTransfer = 112,
            GeneralTransfer = 113,
            SalaryTransfer = 114,
            LoanCreation = 115

        }
        public enum InvoiceStatus
        {
            Approved,
            Pending,
            Rejected,
            Cancelled,
            Draft,
            Order
        }

        public int Id { get; set; }

        public int DisplayInvoiceNo { get; set; }

        [Display(Name="Client")]
        public int? ClientId { get; set; }
        public virtual Client Client { get; set; }
        public DateTime Time { get; set; }
        [Required]
        public string Description { get; set; }

        // Change tracking
       
        public User CreatedBy { get; set; }
        public DateTime CreationTime { get; set; }

        public User AuthorizedBy { get; set; }
        public User LastUpdatedBy { get; set; }
        public DateTime? LastUpdateTime { get; set; }

        //public Account InvoiceType { get; set; }
        [Required]
        public TansactionTypes TransType { get; set; }

        //Related for account invoices only 
        public int? RelatedAccountId { get; set; }

        public int? SalesRepId { get; set; }
        [Display(Name="Sales Rep")]
        public virtual SalesRep SalesRep { get; set; }
        [Required]
        public InvoiceStatus Status { get; set; }

        [Display(Name="Gate Pass")]
        public string GatePassRef { get; set; }
 
        public virtual ICollection<Item> Items { get; set; }
        public virtual ICollection<PaymentReceipt> PaymentReceipts { get; set; }

        /// <summary>
        /// For Sales : Sales Order Id
        /// For Sales Return : Sales Invoice Id
        /// </summary>
        public int? RelatedInvoiceId { get; set; }


        public bool IsTransferInvoice
        {
            get
            {
                var transferTypes =new [] { TansactionTypes.GeneralTransfer,TansactionTypes.PayableTransfer};
                return transferTypes.Contains(TransType);
            }
        }


        public Invoice(TansactionTypes TransType)
        {
            this.TransType = TransType;
            this.Status = InvoiceStatus.Approved;
            this.Items = new List<Item>();
            this.PaymentReceipts = new List<PaymentReceipt>();
            this.DisplayInvoiceNo = 0;
        }

        public Invoice() : this( Invoice.TansactionTypes.Purchases)
        {
        }

        public decimal GetTotal()
        {
            decimal total = 0;

            foreach(Item item in Items)
            {
                total += item.Price ;
            }
            return total;
        }

        public decimal GetPaymentTotal()
        {
            decimal total = 0;

            foreach (PaymentReceipt recpt in PaymentReceipts)
            {
                total += recpt.Amount;
            }
            return total;
        }

        public decimal GetNonCreditPaymentTotal()
        {
            return PaymentReceipts.Where(pr => pr.PaymentMethod != PaymentReceipt.PaymentType.Credit).Sum(pr => pr.Amount);
        }

        [NotMapped]
        [Display(Name ="Invoice No.")]
        public string FormattedInvoiceNo
        {
            get
            {
                return "I" + DisplayInvoiceNo.ToString("00000");
            }
        }

        public static bool TryParseInvoiceNo(string invNumString, out int parsedNumber )
        {
            string numberpart = invNumString;

            if (invNumString.StartsWith("I", StringComparison.OrdinalIgnoreCase))
            {
                numberpart = invNumString.Substring(1);
            }
            return int.TryParse(numberpart, out parsedNumber);
        }
    }
}
