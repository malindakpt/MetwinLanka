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
    public class ExpenseController : GHMController
    {
        AccountManager acMan = new AccountManager();
        InvoiceManager invMan = new InvoiceManager();

        //
        // GET: /Expense/
        public ActionResult Index()
        {
            var accounts = acMan.LoadAccounts(null);
            var invoices = invMan.LoadInvoices(Invoice.TansactionTypes.Expenses);
            ViewBag.Accounts = accounts.ToList();
            return View(invoices);
        }

        //
        // GET: /Expense/Details/5
        public ActionResult Details(int id)
        {
            var invoice = invMan.LoadInvoice(id);
            var expenseAcc = acMan.GetAccountById(invoice.RelatedAccountId.Value);
            ViewBag.ExpenseAccount = expenseAcc;
            return View(invoice);
        }

        //
        // GET: /Expense/Create
        [Authorize]
        public ActionResult Create()
        {
            var acs = acMan.LoadAccounts(null).ToList();
            acs = acs.Where(ac => ac.AccountType == Account.AccountTypes.AdministrationExpenses || ac.AccountType == Account.AccountTypes.SellingAndDistributionExpenses)
                        .ToList();

            ViewBag.ExpenseAccounts = acs;
            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank).Where(ac => ac.Id != Account.BankAccount).ToList(); 

            return View();
        }

        //
        // POST: /Expense/Create
        [HttpPost]
        [Authorize]
        public ActionResult Create(ExpenseViewModel model)
        {
            try
            {
                var invoice = model.ToInvoice();
                invoice.CreationTime = DateTime.Now;
                invoice.CreatedBy = GetCurrentLoggedUser();
                invMan.AddInvoice(invoice);
                return RedirectToAction("Index");
            }
            catch(Exception e)
            {
                var acs = acMan.LoadAccounts(null).ToList();
                acs = acs.Where(ac => ac.AccountType == Account.AccountTypes.AdministrationExpenses || ac.AccountType == Account.AccountTypes.SellingAndDistributionExpenses)
                            .ToList();

                ViewBag.ExpenseAccounts = acs;
                ModelState.AddModelError("", e.Message);
                return View();
            }
        }

    }
}
