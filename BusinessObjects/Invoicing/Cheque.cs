using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Cheque
    {
        public enum Side
        {
            Received,Paid 
        }
        public enum ChequeStatus
        {
            Pending,Settled,Returned
        }


        [Key]
        public int Id { get; set; }
        [Required]
        public string Bank { get; set; }
        public DateTime? SettleDate { get; set; }
        [Required]
        public DateTime HandedOverDate { get; set; }
        [Required]
        public string ChequeNo { get; set; }

        public ChequeStatus Status { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public Side ChequeSide { get; set; }

        public virtual IEnumerable<PaymentReceipt> PaymentRecpt { get; set; }
    }
}
