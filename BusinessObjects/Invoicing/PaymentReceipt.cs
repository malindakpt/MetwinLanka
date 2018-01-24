using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class PaymentReceipt
    {
        
        public enum Sides
        {
            Received,Sent,Other
        }

        public enum PaymentType
        {
            Cash, Cheque, Credit,FromDeposit , Transfer
           //   Cash, Cheque, Credit,Transfer,FromDeposit
        }

        public enum PaymentStatus
        {
            Accepted, Cancelled
        }

        public int Id { get; set; }
        [Required]
        public PaymentType PaymentMethod { get; set; }
        [Required]
        public Sides Side { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Time { get; set; }

        public PaymentStatus Status { get; set; }

        [StringLength(50)]
        public string RefNo { get; set; }
        [StringLength(150)]
        public string Description { get; set; }

        public int? ChequeId { get; set; }
        [ForeignKey("ChequeId")]
        public virtual Cheque ChequeRef { get; set; }

        [Required]
        public int InvoiceId { get; set;}

        [Required]
        public virtual Invoice Invoice { get; set; }

        [Required]
        public int TransactionId { get; set; }
        public virtual Transaction Transaction {get; set;}

        public int? BankAccountId { get; set; }

        /// <summary>
        /// Only for Transfer Amount invoices
        /// </summary>
        public int? TransferFromId { get; set; }
        /// <summary>
        /// Only for Transfer Amount invoices
        /// </summary>
        public Account TransferFrom { get; set; }
        /// <summary>
        /// Only for Transfer Amount invoices
        /// </summary>
        public int? TransferToId { get; set; }
        /// <summary>
        /// Only for Transfer Amount invoices
        /// </summary>
        public Account TransferTo { get; set; }

        public bool IsTaxReceipt { get; set; }

        /// <summary>
        /// instance to identify multiple payments at once
        /// </summary>
        public int PayedInstanceId { get; set; }
        [Required]
        public PaymentInstance PayedInstance { get; set; }
        public PaymentReceipt()
        {
            IsTaxReceipt = false;
        }
    }
}
