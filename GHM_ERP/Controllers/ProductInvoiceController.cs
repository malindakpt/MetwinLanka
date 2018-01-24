using BusinessLogic;
using BusinessObjects;
using BusinessObjects.Categories;
using BusinessObjects.Invoicing;
using GHM_ERP.Models;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class ProductInvoiceController : GHMController
    {
        ProfileManager profManager = new ProfileManager();
        ClientManager clientManager = new ClientManager();
        InvoiceManager invoiceManager = new InvoiceManager();
        SalesRepManager salesRepManager = new SalesRepManager();
        ConfigManager confMan = new ConfigManager();
        //
        // GET: /ProductInvoice/
        public ActionResult Index()
        {
            return View();
        }

        #region Purchases
        [Authorize]
        public ActionResult EditPurchases(int? id)
        {
            var rawprofs = profManager.GetRawMaterialProfiles();
            var clients = clientManager.GetAllClients(Client.Types.Supplier);
            var clientVMs = clients.Select(cl => new ClientViewModel
            {
                ClientID = cl.Id,
                Credits = 0,
                Location = cl.Address,
                Name = cl.Name
            });

            ViewBag.RawMaterialProfiles = rawprofs;
            ViewBag.Clients = clientVMs;
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditPurchases(PurchaseViewModel invoiceModel)
        {
            string errorMsg = "";
            if(!ModelState.IsValid || !invoiceModel.IsValid(out errorMsg))
            {
                var rawprofs = profManager.GetRawMaterialProfiles();
                var clients = clientManager.GetAllClients(Client.Types.Supplier);
                var clientVMs = clients.Select(cl => new ClientViewModel
                {
                    ClientID = cl.Id,
                    Credits = 0,
                    Location = cl.Address,
                    Name = cl.Name
                });

                ViewBag.RawMaterialProfiles = rawprofs;
                ViewBag.Clients = clientVMs;

                ModelState.AddModelError("", errorMsg);
                return View(invoiceModel);
            }
            else
            {
                var rawprofs = profManager.GetRawMaterialProfiles();
                var invoice = invoiceModel.ToInvoice(rawprofs);
                invoice.CreationTime = DateTime.Now;
                //TODO: get current user
                invoice.CreatedBy = GetCurrentLoggedUser();

                invoiceManager.AddInvoice(invoice);
                return RedirectToAction("PurchaseDetails",new {id = invoice.Id});
            }
            
        }

        public ActionResult Purchases()
        {
            var invoices = invoiceManager.LoadInvoices(Invoice.TansactionTypes.Purchases);//.Take(50);
            return View(invoices);
        }

        public ActionResult PurchaseDetails(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            if(invoice == null || invoice.TransType !=  Invoice.TansactionTypes.Purchases)
            {
                return ErrorResult("Invalid Purchase id");
            }
            else
            {
                return View(invoice);
            }
        }
        #endregion

        #region Sales

        public ActionResult Sales(string filter)
        {
            var invoices = invoiceManager.LoadInvoices(Invoice.TansactionTypes.Sales);//.Take(50);
            var salesReturns = invoiceManager.LoadInvoices(Invoice.TansactionTypes.SalesReturn, null, Invoice.InvoiceStatus.Approved);
            var returnAmounts = salesReturns.GroupBy(inv => inv.RelatedInvoiceId)
                                .ToDictionary(grp => grp.Key.Value, grp => grp.Sum(inv => inv.GetTotal()));
            var returnPayments = salesReturns.GroupBy(inv => inv.RelatedInvoiceId)
                                .ToDictionary(grp => grp.Key.Value, grp => grp.Sum(inv => inv.GetNonCreditPaymentTotal()));

            ViewBag.ReturnAmounts = returnAmounts;
            ViewBag.ReturnPayments = returnPayments;
            ViewBag.Filter = filter ?? "";
            return View(invoices);
        }

        public ActionResult FindSale(InvoiceSearchModel model)
        {
            ViewBag.InitialAdvancedTab = model.IsSearchRequested &&  !model.IsSearchByInvNo;
            ViewBag.ShowGatePass = true;
            ViewBag.SalesReps = salesRepManager.GetAllSalesReps(true);

            //before search
            if(!model.IsSearchRequested)
            {
                return View();
            }

            InvoiceSearcher searcher = new InvoiceSearcher();
            string errMsg, infoMsg;
            var invoices = searcher.SearchInvoices(Invoice.TansactionTypes.Sales, null, model, out infoMsg, out errMsg);
            
            SetErrorAlert(errMsg);
            SetInfoAlert(infoMsg);

            return View(invoices);
            
        }

        public ActionResult FindPurchase(InvoiceSearchModel model)
        {
            ViewBag.InitialAdvancedTab = model.IsSearchRequested && !model.IsSearchByInvNo;
            ViewBag.ShowGatePass = false;

            //before search
            if (!model.IsSearchRequested)
            {
                return View();
            }

            InvoiceSearcher searcher = new InvoiceSearcher();
            string errMsg, infoMsg;
            var invoices = searcher.SearchInvoices(Invoice.TansactionTypes.Purchases, null, model, out infoMsg, out errMsg);

            SetErrorAlert(errMsg);
            SetInfoAlert(infoMsg);

            return View(invoices);
        }


        [Authorize]
        public ActionResult EditSales(int? id)
        {
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
            return View();
        }

        [HttpPost]
        public ActionResult EditSales(SaleViewModel invoiceModel)
        {
            string errorMsg = "";
            if (!ModelState.IsValid || !invoiceModel.IsValid(out errorMsg))
            {
                var rawprofs = profManager.GetRawMaterialProfiles();
                var prodProfs = profManager.GetProductProfiles();

                var balances = invoiceManager.GetRawMaterialBalances(null);

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
                ModelState.AddModelError("", errorMsg);
                return View();
            }
            else
            {
                var prodProfs = profManager.GetProductProfiles();
                var invoice = invoiceModel.ToInvoice(prodProfs,confMan.VatPercentage);
                invoice.CreationTime = DateTime.Now;
                invoice.CreatedBy = GetCurrentLoggedUser();

                invoiceManager.AddInvoice(invoice);
                return RedirectToAction("SaleDetails", new { id = invoice.Id});
            }
        }


        public ActionResult SaleDetails(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            if (invoice == null || invoice.TransType != Invoice.TansactionTypes.Sales || invoice.Status == Invoice.InvoiceStatus.Order)
            {
                return ErrorResult("Invalid sales invoice id");
            }
            else
            {
                var searchOptions = new InvoiceSearchOptions
                {
                    RelatedInvoiceId = invoice.Id,
                    TransType = Invoice.TansactionTypes.SalesReturn
                };
                var salesReturns = invoiceManager.SearchInvoices(searchOptions);
                ViewBag.SalesReturns = salesReturns.ToList(); 
                return View(invoice);
            }
        }

        public ActionResult SetGatePass(int id)
        {
            var invoice = invoiceManager.LoadInvoice(id);
            if (invoice == null || invoice.TransType != Invoice.TansactionTypes.Sales || invoice.Status == Invoice.InvoiceStatus.Order)
            {
                return ErrorResult("Invalid sales invoice id");
            }
            else
            {
                return View(invoice);
            }
        }

        [HttpPost]
        public ActionResult SetGatePass(int id, string gatepass)
        {
            invoiceManager.SetInvoiceGatePass(id, gatepass);
            SetInfoAlert("Gate Pass was set to " + gatepass);
            return RedirectToAction("SetGatePass", new { id = id });
        }


        /// <summary>
        /// Sale from order
        /// </summary>
        /// <param name="id">Sales Order ID</param>
        /// <returns></returns>
        public ActionResult SaleFromOrder(int id)
        {
            Invoice invoice = invoiceManager.LoadInvoice(id);
            if(invoice.Status != Invoice.InvoiceStatus.Order || invoice.TransType != Invoice.TansactionTypes.Sales)
            {
                return ErrorResult("Invalid Order id :" + id);
            }
            return View(invoice);
        }

        private IEnumerable<Invoice.TansactionTypes> CancellableInvoices
        {
            get
            {
                var validTypes = new[] {
                    Invoice.TansactionTypes.Sales,
                    Invoice.TansactionTypes.Purchases,
                    Invoice.TansactionTypes.Expenses,
                    Invoice.TansactionTypes.SalesReturn,
                    Invoice.TansactionTypes.GeneralTransfer,
                    Invoice.TansactionTypes.SalaryTransfer
                };
                return validTypes;
            }
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public ActionResult CancelInvoice(string id, Invoice.TansactionTypes? invoiceType)
        {
            ViewBag.CancellableInvoices = CancellableInvoices;

            //search requested 
            if (id != null && invoiceType.HasValue)
            {
                ViewBag.Id = id;
                ViewBag.invoiceType = invoiceType.Value;

                int parsedId;
                if(! Invoice.TryParseInvoiceNo(id,out parsedId))
                {
                    SetErrorAlert("Invalid invoice number -" + id);
                    return View();
                }

                var invoice = invoiceManager.LoadInvoiceByDisplayId(parsedId,invoiceType.Value);
                if(invoice == null || invoice.TransType != invoiceType || invoice.Status == Invoice.InvoiceStatus.Order)
                {
                    SetErrorAlert("Cannot find invoice with number " + id);
                    return View();
                }

                return View(invoice);
            }
            //if not searched
            else if (id == null && !invoiceType.HasValue)
            {
                return View(); 
            }
            //if one field is missing
            else
            {
                SetErrorAlert("One of the required field is missing");
                return View();
            }
            
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public ActionResult CancelInvoice(int InvoiceId, Invoice.TansactionTypes? invoiceType,bool Confirmed)
        {
            ViewBag.CancellableInvoices = CancellableInvoices;

            if (!Confirmed)
            {
                return ErrorResult("Invalid Form submission");
            }

            string reason;
            bool success = invoiceManager.CancelInvoice(InvoiceId, GetCurrentLoggedUser().UserName, out reason);
            string invFormatted = FormatAsInvoiceNo(InvoiceId);
            if(success)
            {
                SetInfoAlert("Invoice "+ invFormatted + " cancelled successfully");
            }
            else
            {
                SetErrorAlert("Cancellation failed. Reason - " + reason);
            }
            return RedirectToAction("CancelInvoice", new { id = invFormatted, invoiceType = invoiceType });
        }


        #endregion

        protected override void Dispose(bool disposing)
        {
            profManager.Dispose();
            salesRepManager.Dispose();
            clientManager.Dispose();
           
            base.Dispose(disposing);
        }
    }
}