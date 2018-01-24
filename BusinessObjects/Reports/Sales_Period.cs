using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Reports
{
    public class Sales_Period
    {
        public DateTime date { get; set; }
        public int invoiceId { get; set; }
        public string DisplayInvoiceId { get; set; }
        public String customerName { get; set; }
        public String saleRepName { get; set; }
        public Decimal amount { get; set; }
    }
}
