using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer.PersisterInterface;
using DataAccessLayer;
using NLog;
using BusinessObjects.Categories;
using BusinessObjects.Invoicing;

namespace BusinessLogic
{
    public class InvoiceManager
    {
        public IAccountPersister AccPersister {get;set;}
        public IInvoicePersister InvPersister { get; set; }

        Logger logger = LogManager.GetCurrentClassLogger();

        public InvoiceManager()
        {
            InvPersister = new InvoicePersister();
        }

        public void AddInvoice(Invoice invoice)
        {
            logger.Info("Adding new {0} invoice - {1} items, {2} payments", invoice.TransType, invoice.Items.Count, invoice.PaymentReceipts.Count);

            if(invoice.Status == Invoice.InvoiceStatus.Cancelled || invoice.Status == Invoice.InvoiceStatus.Rejected)
            {
                logger.Error("Invalid invoice status {0}", invoice.Status);
                throw new ArgumentException("Invalid invoice status - " + invoice.Status);
            }

            TransactionManager tm = new TransactionManager();
            ValidateInvoice(invoice);

            if(invoice.Status == Invoice.InvoiceStatus.Approved)
            {
                if(invoice.TransType == Invoice.TansactionTypes.SalaryTransfer)
                {
                    tm.AddSalaryTransactions(invoice);
                }
                else if (invoice.TransType == Invoice.TansactionTypes.LoanCreation)
                {
                    tm.AddLoanCreateTransactions(invoice);
                }
                else if(invoice.IsTransferInvoice)
                {
                    tm.AddTransferTransactions(invoice);
                }
                else
                {
                    tm.AddTransactions(invoice);
                }
                
            }
            
            InvPersister.AddInvoice(invoice);

        }

        public void AddPaymentToInvoice(IDictionary<int, PaymentReceipt> paymentReceipts)
        {
            foreach (var item in paymentReceipts)
            {
                int invoiceId = item.Key;
                PaymentReceipt receipt = item.Value;

                var invoice = InvPersister.LoadInvoice(invoiceId);
                invoice.PaymentReceipts.Add(receipt);
                TransactionManager tm = new TransactionManager();
                tm.AddTransactions(invoice);
                InvPersister.EditInvoice(invoice);
            }
        }

        public IEnumerable<Invoice> LoadInvoices(Invoice.TansactionTypes? type, DateRange range = null,
            Invoice.InvoiceStatus? status = Invoice.InvoiceStatus.Approved, int resultCount = 0)
        {
            var invoices = InvPersister.LoadInvoices(type, range, status, resultCount);
            return invoices;
        }

        public Invoice LoadInvoice(int id)
        {
            return InvPersister.LoadInvoice(id);
        }

        public Invoice LoadInvoiceByDisplayId(int id,Invoice.TansactionTypes transType, bool isOrder = false)
        {
            return InvPersister.LoadInvoiceByDisplayId(id,transType,isOrder);
        }

        public IEnumerable<RawMaterialBalance> GetRawMaterialBalances(DateTime? toDate=null)
        {
            return InvPersister.GetRawMaterialBalances(toDate);
        }

        public IEnumerable<DueInvoiceModel> GetDueInvoices(Invoice.TansactionTypes transType)
        {
            return InvPersister.GetDueInvoices(transType);
        }

        public void SetInvoiceGatePass(int invoiceId, string gatepass)
        {
            InvPersister.SetInvoiceGatePass(invoiceId, gatepass);
        }

        public IEnumerable<Invoice> SearchInvoices(InvoiceSearchOptions options)
        {
            return InvPersister.SearchInvoices(options);
        }

        public bool CancelInvoice(int invoiceId,string username,out string failedReason)
        {
            InvPersister.CancelInvoice(invoiceId, username, DateTime.Now);

            failedReason = "Success";
            return true;
        }


        //-------------- HELPERS -------------

        private void ValidateInvoice(Invoice invoice)
        {
            ValidateInvoiceItemTypes(invoice);
            ValidateInvoiceItemCounts(invoice);
        }
        private void ValidateInvoiceItemTypes(Invoice invoice)
        {
            //validate invoice item types
            Item invalidItem = null;
            switch (invoice.TransType)
            {
                case Invoice.TansactionTypes.Purchases:
                case Invoice.TansactionTypes.PurchasesReturn:
                    invalidItem = invoice.Items.FirstOrDefault(i =>!(i.Profile is RawMaterialProfile));
                    break;
                case Invoice.TansactionTypes.Sales:
                case Invoice.TansactionTypes.SalesReturn:
                    invalidItem = invoice.Items.FirstOrDefault(i =>! (i.Profile is ProductProfile));
                    break;
                case Invoice.TansactionTypes.Expenses:
                case Invoice.TansactionTypes.Income:
                case Invoice.TansactionTypes.Depreciation:
                case Invoice.TansactionTypes.PayableTransfer:
                case Invoice.TansactionTypes.GeneralTransfer:
                case Invoice.TansactionTypes.SalaryTransfer:
                case Invoice.TansactionTypes.LoanCreation:
                    //items must be empty collection
                    invalidItem = invoice.Items.FirstOrDefault();
                    break;
                case Invoice.TansactionTypes.CapitalDeposite:
                case Invoice.TansactionTypes.CapitalWithdraw:
                case Invoice.TansactionTypes.Deposit:
                    break;
                default:
                    logger.Error("Unknown invoice type {0}({1}) in invoice", (int)invoice.TransType, invoice.TransType);
                    throw new ArgumentException("Unknown Item type in invoice - " + (int)invoice.TransType);
            }

            if(invalidItem != null)
            {
                logger.Error("Unexpected item of {0} in {1} invoice", invalidItem.GetType().Name, invoice.TransType);
                throw new ArgumentException("Unexpected item in invoice - " + invalidItem.GetType().Name);
            }
            if(invoice.TransType == Invoice.TansactionTypes.Expenses && (
                invoice.RelatedAccountId == null || invoice.RelatedAccountId == 0))
            {
                logger.Error("Expense invoice without expense account");
                throw new ArgumentException("Unexpected item in invoice - " + invalidItem.GetType().Name);
            }
        }

        private void ValidateInvoiceItemCounts(Invoice invoice)
        {
            switch (invoice.TransType)
            {
                case Invoice.TansactionTypes.Expenses:
                case Invoice.TansactionTypes.Income:
                case Invoice.TansactionTypes.Depreciation:
                case Invoice.TansactionTypes.PayableTransfer:
                    //if(invoice.Items.Count != 1)
                    //{
                    //    logger.Error("Expected 1 item, found {0} . Transtype - {1}", invoice.Items.Count, invoice.TransType);
                    //    throw new ArgumentException("Invoice type " + invoice.TransType + " must have exactly one item");
                    //}
                    //break;
                default:
                    break;
            }
        }


    }
}
