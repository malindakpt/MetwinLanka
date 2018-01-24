using BusinessObjects;
using BusinessObjects.Invoicing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.PersisterInterface
{


    public interface IInvoicePersister : IDisposable
    {
        void AddInvoice(Invoice invoice);
        void EditInvoice(Invoice invoice);
        Invoice LoadInvoice(int Id);

        Invoice LoadInvoiceByDisplayId(int Id,Invoice.TansactionTypes transType, bool isOrder = false);
        /// <summary>
        /// Load all invoices
        /// </summary>
        /// <param name="type">Transaction Type for which to load data. null to load all types</param>
        /// <param name="range">Date range, null for all trascations</param>
        /// <param name="status">Invoices status. null for all types</param>
        /// <returns></returns>
        IEnumerable<Invoice> LoadInvoices(Invoice.TansactionTypes? type, DateRange range = null,
            Invoice.InvoiceStatus? status = Invoice.InvoiceStatus.Approved, int resultCount = 0);

        /// <summary>
        /// Get all checks
        /// </summary>
        /// <param name="side">Side - Received/Paid</param>
        /// <param name="status">Status - null for all checks</param>
        /// <param name="range">Date range in cheque payments date</param>
        /// <returns></returns>
        IEnumerable<Cheque> GetChequePayments(Cheque.Side side, Cheque.ChequeStatus? status = null, DateRange range = null);

        IEnumerable<DueInvoiceModel> GetDueInvoices(Invoice.TansactionTypes type);
        IEnumerable<RawMaterialBalance> GetRawMaterialBalances(DateTime? toDate);

        void SetInvoiceGatePass(int invoiceId, string gatepass);


        IEnumerable<Invoice> SearchInvoices(InvoiceSearchOptions options);

        void CancelInvoice(int invoiceId,string username, DateTime updateTime);

    }

}
