using BusinessLogic;
using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class PrintController : Controller
    {
        InvoiceManager invoiceManager = new InvoiceManager();
        //
        // GET: /Print/
        public ActionResult Index()
        {
            return View();
        }

        // GET: /Print/Invoice
        public ActionResult Invoice(int id)
        {
            /*
             * check for load invoice functionality from geeth
             * var invoice = invoiceManager.LoadInvoice(id);
             * here i create just a sample invoice
             */

            var invoice = invoiceManager.LoadInvoice(id);         
            return PartialView(invoice);
            
        }

        public ActionResult VatInvoice(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            return View(invoice);
        }



	}
}