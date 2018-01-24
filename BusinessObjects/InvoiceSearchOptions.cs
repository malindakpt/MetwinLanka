using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class InvoiceSearchOptions
    {
      //  Invoice.TansactionTypes? type,Invoice.InvoiceStatus? status = Invoice.InvoiceStatus.Approved,
        //    DateRange range = null, string description = null , string  , int resultCount = 0)

        public Invoice.TansactionTypes TransType { get; set; }
        public Invoice.InvoiceStatus? Status { get; set; }
        public DateRange Range { get; set; }
        public string Description { get; set; }
        public string GatePass { get; set; }

        public int? ResultCount { get; set; }

        public bool IncludePaymentReceipts { get; set; }

        public int? RelatedInvoiceId { get; set; }

        public InvoiceSearchOptions()
        {
            TransType = Invoice.TansactionTypes.Sales;
            Status = Invoice.InvoiceStatus.Approved;
            IncludePaymentReceipts = false;
        }

    }
}
