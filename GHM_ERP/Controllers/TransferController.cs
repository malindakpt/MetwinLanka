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
    public class TransferController : GHMController
    {
        AccountManager acMan = new AccountManager();
        InvoiceManager invMan = new InvoiceManager();

        //
        // GET: /Transfer/
        public ActionResult Index()
        {
            int resultLimit = 400;
            var generalInvs = invMan.LoadInvoices(Invoice.TansactionTypes.GeneralTransfer, resultCount: resultLimit);
            var liabilityInvs = invMan.LoadInvoices(Invoice.TansactionTypes.PayableTransfer, resultCount: resultLimit);

            var allTypes = generalInvs.Concat(liabilityInvs).OrderByDescending(inv => inv.Time).Take(resultLimit);

            return View(allTypes.ToArray());
        }

        public ActionResult Create()
        {
            ViewBag.FromAccounts = acMan.LoadAccounts(null);
            ViewBag.ToAccounts = ViewBag.FromAccounts;
            return View();
        }

        [HttpPost]
        public ActionResult Create(TransferAccountModel transferModel)
        {
            string errorMsg = "";
            bool isValidData = transferModel.IsValid(out errorMsg);
            if (!ModelState.IsValid || !isValidData)
            {
                ViewBag.FromAccounts = acMan.LoadAccounts(null);
                ViewBag.ToAccounts = ViewBag.FromAccounts;

                if(!isValidData)
                {
                    ModelState.AddModelError("", errorMsg);
                }

                return View(transferModel);
            }

            var invoice = transferModel.ToInvoice(Invoice.TansactionTypes.GeneralTransfer);
            invoice.CreatedBy = GetCurrentLoggedUser();
            invoice.CreationTime = DateTime.Now;
            invMan.AddInvoice(invoice);

            return RedirectToAction("Index");
        }

        private void SetLiabilityAccountList()
        {
            var allAccounts = acMan.LoadAccounts(null);
            var allowedLiabilities = new[] { Account.AccountTypes.CurrentLiabilities, Account.AccountTypes.FixedLiabilities };
            ViewBag.ToAccounts = allAccounts.Where(acc => allowedLiabilities.Contains(acc.AccountType)).ToList();
            var allowedAssets = new[] { Account.AccountTypes.CurrentAssets, Account.AccountTypes.CurrentAssets_Bank, Account.AccountTypes.CurrentAssets_Cash };
            ViewBag.FromAccounts = allAccounts.Where(ac => allowedAssets.Contains(ac.AccountType)).ToList();
        }

        public ActionResult NewLiability()
        {
            SetLiabilityAccountList();
            return View();
        }

        [HttpPost]
        public ActionResult NewLiability(TransferAccountModel transferModel)
        {
            string errorMsg = "";
            bool isValidData = transferModel.IsValid(out errorMsg);
            if (!ModelState.IsValid || !isValidData)
            {
                SetLiabilityAccountList();

                if (!isValidData)
                {
                    ModelState.AddModelError("", errorMsg);
                }

                return View(transferModel);
            }

            var invoice = transferModel.ToInvoice(Invoice.TansactionTypes.GeneralTransfer);
            invoice.CreatedBy = GetCurrentLoggedUser();
            invoice.CreationTime = DateTime.Now;
            invMan.AddInvoice(invoice);

            return RedirectToAction("Index");
        }

        private void SetAssetAccountList()
        {
            var allAccounts = acMan.LoadAccounts(null);
            var allowedAssets = new[] { Account.AccountTypes.OtherFixedAssets, Account.AccountTypes.FixedAssets,
                Account.AccountTypes.CurrentAssets, Account.AccountTypes.CurrentAssets_Bank, Account.AccountTypes.CurrentAssets_Cash,
                Account.AccountTypes.CurrentAssets_Customer };

            ViewBag.ToAccounts = allAccounts.Where(acc => allowedAssets.Contains(acc.AccountType)).ToList();
            ViewBag.FromAccounts = ViewBag.ToAccounts;
        }

        public ActionResult NewAsset()
        {
            SetAssetAccountList();
            return View();
        }

        [HttpPost]
        public ActionResult NewAsset(TransferAccountModel transferModel)
        {
            string errorMsg = "";
            bool isValidData = transferModel.IsValid(out errorMsg);
            if (!ModelState.IsValid || !isValidData)
            {
                SetAssetAccountList();

                if (!isValidData)
                {
                    ModelState.AddModelError("", errorMsg);
                }

                return View(transferModel);
            }

            var invoice = transferModel.ToInvoice(Invoice.TansactionTypes.GeneralTransfer);
            invoice.CreatedBy = GetCurrentLoggedUser();
            invoice.CreationTime = DateTime.Now;
            invMan.AddInvoice(invoice);

            return RedirectToAction("Index");
        }

        public ActionResult Details(int id)
        {
            var invoice = invMan.LoadInvoice(id);
            if(invoice == null || !invoice.IsTransferInvoice)
            {
                return ErrorResult("Cannot find invoice");
            }
            if(invoice.PaymentReceipts.Count  == 1)
            {
                return View(invoice);
            }
            else
            {
                return View("DetailsMultiple", invoice);
            }
            
        }

        public ActionResult MassTransfer()
        {
            var accounts = acMan.LoadAccounts(null, true).OrderBy(ac => ac.Code + "|" + ac.Name);
            ViewBag.Accounts = accounts;
            return View(); 
        }

        [HttpPost]
        public ActionResult MassTransfer(MassTransferModel model)
        {
            string errorMsg = "";
            if(!ModelState.IsValid)
            {
                var errorMsgs = ModelState.SelectMany(x => x.Value.Errors).Select(er => er.ErrorMessage);
                errorMsg = string.Join("\n", errorMsgs);
                return ErrorResult(errorMsg);
            }
            else if(! model.IsValidTransfer(out errorMsg))
            {
                return ErrorResult(errorMsg);
            }

            var invoice = model.ToInvoice(Invoice.TansactionTypes.GeneralTransfer);
            invoice.CreatedBy = GetCurrentLoggedUser();
            invoice.CreationTime = DateTime.Now;
            invMan.AddInvoice(invoice);
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
             base.Dispose(disposing);
        }
    }
}