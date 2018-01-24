using BusinessLogic;
using BusinessObjects;
using GHM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GHM_ERP.Util
{
    public class InvoiceSearcher
    {
        public IEnumerable<Invoice> SearchInvoices(Invoice.TansactionTypes transType,Invoice.InvoiceStatus? invStatus,InvoiceSearchModel model,
           out string infoMsg, out string errMsg)
        {
            var emptyList = new Invoice[0];
            InvoiceManager invoiceManager = new InvoiceManager();
            infoMsg = errMsg = null;

            if (model.IsSearchByInvNo)
            {
                int invNum = 0;
                bool isValidId = Invoice.TryParseInvoiceNo(model.InvNo, out invNum);
                if (!isValidId)
                {
                    errMsg = "Invalid invoice number format";
                    return emptyList;
                }
                else
                {
                    var invoice = invoiceManager.LoadInvoiceByDisplayId(invNum,transType,invStatus == Invoice.InvoiceStatus.Order);
                    if (invoice != null &&
                            (!invStatus.HasValue || invoice.Status == invStatus.Value)
                        )
                    {
                        return new[] { invoice };
                    }
                    else
                    {
                        errMsg = "No results for invoice number " + model.InvNo;
                        return emptyList;
                    }
                }
            }
            else //advanced search
            {
                int filterCount;
                InvoiceSearchOptions options = model.ToInvoiceSearchOptions(transType, out filterCount);
                if(invStatus.HasValue)
                {
                    options.Status = invStatus.Value;
                }

                //no filters
                if (filterCount == 0)
                {
                    errMsg = "Please enter at least one field to search";
                    return emptyList;
                }
                else
                {
                    var invoices = invoiceManager.SearchInvoices(options).ToList();
                    if (invoices.Any())
                    {
                        return invoices;
                    }
                    else
                    {
                        errMsg = "No results for the search terms entered";
                        return emptyList;
                    }
                }
            }
            
        }
    }
}