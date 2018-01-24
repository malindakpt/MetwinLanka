using BusinessLogic;
using GHM_ERP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using GHM_ERP.Util;

namespace GHM_ERP.Controllers
{
    

    [Authorize]
    public class LoanController : GHMController
    {
        AccountManager accMan = new AccountManager();
        InvoiceManager invMan = new InvoiceManager();

        public ActionResult Index()
        {
            InvoiceSearchOptions options = new InvoiceSearchOptions
            {
                IncludePaymentReceipts = true,
                TransType = Invoice.TansactionTypes.LoanCreation,
                Status = Invoice.InvoiceStatus.Approved
            };

            var loans = invMan.SearchInvoices(options).OrderBy(inv => inv.Time);
            var accounts = accMan.LoadAccounts(null).ToDictionary(ac => ac.Id);

            foreach (var loan in loans)
            {
                var paymentRecpt = loan.PaymentReceipts.First();
                paymentRecpt.TransferTo = accounts[paymentRecpt.TransferToId.Value];
                paymentRecpt.TransferFrom = accounts[paymentRecpt.TransferFromId.Value];
            }

            return View(loans);
        }

        private void SetAllowedAccounts()
        {
            var bankAccs = accMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank);
            var cashAcc = accMan.GetAccountById(Account.CashAccount);
            var allowedAccs = (new[] { cashAcc }).Concat(bankAccs).Where(ac => ac.Id != Account.BankAccount);

            ViewBag.AllowedAccounts = allowedAccs;
        }

        public ActionResult Create()
        {
            SetAllowedAccounts();
            return View();
        }

        [HttpPost]
        public ActionResult Create(LoanCreateModel loanModel)
        {
            if (ModelState.IsValid)
            {
                var loanAcc = accMan.CreateAccount(loanModel.Name, Account.AccountTypes.FixedLiabilities);
                var invoice = loanModel.ToInvoice(loanAcc.Id);
                invoice.CreatedBy = GetCurrentLoggedUser();
                invoice.CreationTime = DateTime.Now;
                invMan.AddInvoice(invoice);

                SetInfoAlert(string.Format("Loan '{0}' created successfully", loanModel.Name));
                return RedirectToAction("Index");
            }
            else
            {
                SetAllowedAccounts();
                return View();
            }
        }
    }
}