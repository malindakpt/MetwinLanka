using BusinessLogic;
using BusinessObjects;
using BusinessObjects.Invoicing;
using GHM_ERP.Models;
using GHM_ERP.Util;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class PaymentController : GHMController
    {

        ClientManager cliMan = new ClientManager();
        InvoiceManager invMan = new InvoiceManager();
        AccountManager acMan = new AccountManager();
        //
        // GET: /Payment/
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Send()
        {
            ViewBag.Clients = cliMan.GetAllClients(Client.Types.Supplier);
            
            ViewBag.DueInvoices = GetDuePurchaseInvoices();
            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank).Where(ac => ac.Id != Account.BankAccount).ToList(); 
            return View();
        }

        [HttpPost]
        [Authorize]
        public ActionResult Send(PaymentModel payment)
        {
            ViewBag.Clients = cliMan.GetAllClients(Client.Types.Supplier);

            ViewBag.DueInvoices = GetDuePurchaseInvoices();
            var bankAccs = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank).Where(ac => ac.Id != Account.BankAccount).ToList();
            ViewBag.BankAccounts = bankAccs;

            string errorMsg;
            if(ModelState.IsValid && !payment.IsValid(out errorMsg) )
            {
                ModelState.AddModelError("", errorMsg);
                return View(payment);
            }
            if (payment.ChequeBankAccount.HasValue)
            {
                payment.ChequeBank = bankAccs.First(ac => ac.Id == payment.ChequeBankAccount.Value).Name;
            }

            var paymentDetails = payment.GetPaymentSendDetails();
            invMan.AddPaymentToInvoice(paymentDetails.PaymentReceiptsAdded);
            if (paymentDetails.DepositInvoice != null)
            {
                paymentDetails.DepositInvoice.CreatedBy = GetCurrentLoggedUser();
                paymentDetails.DepositInvoice.CreationTime = DateTime.Now;
                invMan.AddInvoice(paymentDetails.DepositInvoice);
            }
            
            var invID = payment.PayedInvoices.First().InvoiceId;
            return RedirectToAction("PurchaseDetails", "ProductInvoice", new { id = invID });
        }


        [Authorize]
        public ActionResult Receive()
        {
            ViewBag.Clients = cliMan.GetAllClients(Client.Types.Customer);
            ViewBag.DueInvoices = GetDueSaleInvoices();
            ViewBag.ClientDeposit = cliMan.GetClientsAvailableDeposits(Client.Types.Customer).ToDictionary(cl => cl.Key.Id.ToString(), cl => cl.Value);
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult Receive(PaymentModel payment)
        {
            ViewBag.Clients = cliMan.GetAllClients(Client.Types.Customer);
            ViewBag.DueInvoices = GetDueSaleInvoices();
            ViewBag.ClientDeposit = cliMan.GetClientsAvailableDeposits(Client.Types.Customer).ToDictionary(cl => cl.Key.Id.ToString(), cl => cl.Value);
            string errorMsg;
            if (ModelState.IsValid && !payment.IsValid(out errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View(payment);
            }
            if(payment.Method == PaymentModel.PaymentMethod.Deposit)
            {
                var available = cliMan.GetClientAvailableDeposits(payment.ClientId);
                if(!available.HasValue || available.Value < payment.Amount)
                {
                    ModelState.AddModelError("", "Insufficient deposit");
                    return View(payment);
                }
            }

            var paymentDetails = payment.GetPaymentRecvDetails();
            invMan.AddPaymentToInvoice(paymentDetails.PaymentReceiptsAdded);
            if (paymentDetails.DepositInvoice != null)
            {
                paymentDetails.DepositInvoice.CreatedBy = GetCurrentLoggedUser();
                paymentDetails.DepositInvoice.CreationTime = DateTime.Now;
                invMan.AddInvoice(paymentDetails.DepositInvoice);
            }

            var invID = payment.PayedInvoices.First().InvoiceId;
            return RedirectToAction("SaleDetails", "ProductInvoice", new { id = invID });
        }

         [Authorize]
        public ActionResult SalesReturn()
        {
            ViewBag.Clients = cliMan.GetAllClients(Client.Types.Customer);
            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank).Where(ac => ac.Id != Account.BankAccount).ToList();
            ViewBag.DueInvoices = GetDueSaleReturnInvoices();
            return View();
        }


        [HttpPost]
        [Authorize]
        public ActionResult SalesReturn(PaymentModel payment)
        {
            ViewBag.Clients = cliMan.GetAllClients(Client.Types.Customer);
            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank).Where(ac => ac.Id != Account.BankAccount).ToList();
            ViewBag.DueInvoices = GetDueSaleReturnInvoices();
            string errorMsg;
            if (ModelState.IsValid && !payment.IsValid(out errorMsg))
            {
                ModelState.AddModelError("", errorMsg);
                return View(payment);
            }
            var paymentDetails = payment.GetPaymentRecvDetails();
            invMan.AddPaymentToInvoice(paymentDetails.PaymentReceiptsAdded);
            if (paymentDetails.DepositInvoice != null)
            {
                paymentDetails.DepositInvoice.CreatedBy = GetCurrentLoggedUser();
                paymentDetails.DepositInvoice.CreationTime = DateTime.Now;
                invMan.AddInvoice(paymentDetails.DepositInvoice);
            }
            var invID = payment.PayedInvoices.First().InvoiceId;
            return RedirectToAction("Details", "SalesReturn", new { id = invID });
        }


        [Authorize]
         public ActionResult CreateDeposit()
         {
             ViewBag.Clients = cliMan.GetAllClients(Client.Types.Customer);
             return View();
         }


        #region helper methods
        private IEnumerable<DueInvoiceModel> GetDuePurchaseInvoices()
        {
            return invMan.GetDueInvoices(Invoice.TansactionTypes.Purchases);
        }

        private IEnumerable<DueInvoiceModel> GetDueSaleInvoices()
        {
            return invMan.GetDueInvoices(Invoice.TansactionTypes.Sales);
        }

        private IEnumerable<DueInvoiceModel> GetDueSaleReturnInvoices()
        {
            return invMan.GetDueInvoices(Invoice.TansactionTypes.SalesReturn);
        }

        protected override void Dispose(bool disposing)
        {
            cliMan.Dispose();
            base.Dispose(disposing);
        }

        #endregion
    }
}