using BusinessLogic;
using BusinessObjects;
using GHM_ERP.Models;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class SalesReturnController : GHMController
    {
        InvoiceManager invoiceManager = new InvoiceManager();
        ProfileManager profMan = new ProfileManager();


        public ActionResult FindInvoice(string query = null)
        {
            ViewBag.Query = query;
            Invoice invoice = null;

            if(query != null)
            {
                int id;
                if (Invoice.TryParseInvoiceNo(query, out id))
                {
                    invoice = invoiceManager.LoadInvoiceByDisplayId(id, Invoice.TansactionTypes.Sales);
                    if(invoice == null || invoice.TransType != Invoice.TansactionTypes.Sales)
                    {
                        ViewBag.ErrorMsg = "Sales Invoice not found";
                        invoice = null;
                    }
                }
                else
                {
                    ViewBag.ErrorMsg = "Invalid format for invoice number";
                }
            }

            return View(invoice);
        }

        public ActionResult Create(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            if (invoice == null || invoice.TransType != Invoice.TansactionTypes.Sales)
            {
                return ErrorResult("Sales Invoice not found");
            }

            return View(invoice);
        }

        [HttpPost]
        public ActionResult Create(int id,SalesReturnModel returnModel)
        {
            if(!ModelState.IsValid || !returnModel.IsValid())
            {
                TempData["ErrorMsg"] = "Form data incomplete. Please select the date and item quantities to return";
                return RedirectToAction("Create", new { id = id });
            }
            else
            {
                var original = invoiceManager.LoadInvoice(id);
                var invoice = returnModel.ToInvoice(original, profMan.GetProductProfiles(true));
                invoice.CreatedBy = GetCurrentLoggedUser();
                invoice.CreationTime = DateTime.Now;
                invoiceManager.AddInvoice(invoice);
                return RedirectToAction("Details", new { id = invoice.Id });
            }
        }

        public ActionResult Details(int id)
        {
            
            Invoice invoice = invoiceManager.LoadInvoice(id);

            if (invoice == null || invoice.TransType != Invoice.TansactionTypes.SalesReturn)
            {
                var errorMsg = "Sales Return Invoice not found";
                return ErrorResult(errorMsg);
            }
            ViewBag.SalesInvoice = invoiceManager.LoadInvoice(invoice.RelatedInvoiceId.Value);
            return View(invoice);
        }

        public ActionResult Index()
        {
            var invoices = invoiceManager.LoadInvoices(Invoice.TansactionTypes.SalesReturn, null, Invoice.InvoiceStatus.Approved,20);
            return View(invoices);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}