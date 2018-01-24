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
    [Authorize]
    public class SalesOrderController : GHMController
    {

        ProfileManager profManager = new ProfileManager();
        ClientManager clientManager = new ClientManager();
        InvoiceManager invoiceManager = new InvoiceManager();
        SalesRepManager salesRepManager = new SalesRepManager();
        //
        // GET: /ProductOrder/
        public ActionResult Index()
        {
            var invoices = invoiceManager.LoadInvoices(Invoice.TansactionTypes.Sales, null, Invoice.InvoiceStatus.Order, 20);
            return View(invoices);
        }

        //
        // GET: /ProductOrder/Details/5
        public ActionResult Details(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            if (invoice == null || invoice.TransType != Invoice.TansactionTypes.Sales || invoice.Status != Invoice.InvoiceStatus.Order)
            {
                return ErrorResult("Invalid Sales Order id");
            }
            else
            {
                var options = new InvoiceSearchOptions { RelatedInvoiceId = id };
                var relatedSales = invoiceManager.SearchInvoices(options);
                ViewBag.RelatedSales = relatedSales;
                return View(invoice);
            }
        }

        //
        // GET: /ProductOrder/Create
        public ActionResult Create()
        {

            var rawprofs = profManager.GetRawMaterialProfiles();

            var prodProfs = profManager.GetProductProfiles();


            var clients = clientManager.GetAllClients(Client.Types.Customer).Select(cl => new ClientViewModel
            {
                ClientID = cl.Id,
                Credits = 0,
                Location = cl.Address,
                Name = cl.Name
            });

            ViewBag.RawMaterialProfiles = rawprofs;
            ViewBag.ProductProfiles = prodProfs;

            ViewBag.Clients = clients;
            ViewBag.SalesReps = salesRepManager.GetAllSalesReps();

            return View();
        }

        //
        // POST: /ProductOrder/Create
        [HttpPost]
        public ActionResult Create(SalesOrderViewModel order)
        {

            if (!ModelState.IsValid)
            {
                var rawprofs = profManager.GetRawMaterialProfiles();
                var prodProfs = profManager.GetProductProfiles();

                var clients = clientManager.GetAllClients(Client.Types.Customer).Select(cl => new ClientViewModel
                {
                    ClientID = cl.Id,
                    Credits = 0,
                    Location = cl.Address,
                    Name = cl.Name
                }).ToList();

                ViewBag.RawMaterialProfiles = rawprofs;
                ViewBag.ProductProfiles = prodProfs;

                ViewBag.Clients = clients;
                ViewBag.SalesReps = salesRepManager.GetAllSalesReps();
                return View();
            }
            else
            {
                var prodProfs = profManager.GetProductProfiles();
                var invoice = order.ToInvoice(prodProfs);
                invoice.CreationTime = DateTime.Now;
                invoice.CreatedBy = GetCurrentLoggedUser();

                invoiceManager.AddInvoice(invoice);
                return RedirectToAction("Details", new { id = invoice.Id });
            }

        }

        /// <summary>
        /// New Sale for order
        /// </summary>
        /// <param name="id">Order ID</param>
        public ActionResult NewSale(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            if (invoice == null || invoice.TransType != Invoice.TansactionTypes.Sales || invoice.Status != Invoice.InvoiceStatus.Order)
            {
                return ErrorResult("Invalid Sales Order id");
            }
            else
            {
                ConfigManager confMan = new ConfigManager();

                var balances = invoiceManager.GetRawMaterialBalances(null);
                var rawprofs = profManager.GetRawMaterialProfiles();
                var prodProfs = profManager.GetProductProfiles();

                var clients = clientManager.GetAllClients(Client.Types.Customer).Select(cl => new ClientViewModel
                {
                    ClientID = cl.Id,
                    Credits = 0,
                    Location = cl.Address,
                    Name = cl.Name
                });

                ViewBag.RawMaterialProfiles = rawprofs;
                ViewBag.ProductProfiles = prodProfs;
                ViewBag.Balances = balances;
                ViewBag.Clients = clients;
                ViewBag.SalesReps = salesRepManager.GetAllSalesReps();
                ViewBag.TaxPercentage = confMan.VatPercentage * 100;

                ViewBag.SourceOrder = invoice;
                return View();
            }
        }

        protected override void Dispose(bool disposing)
        {
            profManager.Dispose();
            salesRepManager.Dispose();
            clientManager.Dispose();
           
            base.Dispose(disposing);
        }
    }
}
