using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Invoicing
{
    public class DueInvoiceModel
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Description { get; set; }
        public string Date { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal DueAmount { get; set; }
        public decimal ReturnAmount { get; set; }
        public string FormattedInvNo { get; set; }
    }
}
