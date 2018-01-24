using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Transaction
    {
        [Required]
        [Key, ForeignKey("PaymentReceipt")]
        public int Id { get; set; }
        [Required]
        public DateTime Time { get; set; }
        [Required]
        public int InvoiceRefId { get; set; }
        public Invoice InvoiceRef { get; set; }

        public string Description { get; set; }
        [Required]
        public int DeAccId { get; set; }
        public virtual Account DeAcc { get; set; }
        [Required]
        public int CrAccId { get; set; }
        public virtual Account CrAcc { get; set; }
        [Required]
        public decimal Amount { get; set; }

        [Required]
        public int PaymentReceiptId { get; set; }
        public virtual PaymentReceipt PaymentReceipt { get; set; }

    }
}
