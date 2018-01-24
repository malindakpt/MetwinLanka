using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Reports
{
    public class Purchase_Period
    {
        public DateTime date { get; set; }
        public int purchadeId { get; set; }
        public String supplier { get; set; }
        public decimal amount { get; set; }
        public string DisplayInvoiceId { get; set; }
    }
}
