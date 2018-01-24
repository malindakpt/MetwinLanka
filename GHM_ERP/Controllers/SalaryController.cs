using BusinessLogic;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;
using BusinessObjects.Accounting;
using GHM_ERP.Models;


namespace GHM_ERP.Controllers
{

    [Authorize]
    public class SalaryController : GHMController
    {
        AccountManager acMan = new AccountManager();
        InvoiceManager invMan = new InvoiceManager();
        //
        // GET: /Salary/
        public ActionResult Index()
        {
            InvoiceSearchOptions options = new InvoiceSearchOptions
            {
                IncludePaymentReceipts = true,
                TransType = Invoice.TansactionTypes.SalaryTransfer,
                Status = Invoice.InvoiceStatus.Approved
            };

            var salaryPayments = invMan.SearchInvoices(options).OrderBy(inv => inv.Time);
            return View(salaryPayments);
        }

        //
        // GET: /Salary/Details/5
        public ActionResult Details(int id)
        {
            var invoice = invMan.LoadInvoice(id);
            if(invoice == null || invoice.TransType != Invoice.TansactionTypes.SalaryTransfer)
            {
                return ErrorResult("Salary invoice not found");
            }

            return View(invoice);
        }

        //
        // GET: /Salary/Create
        public ActionResult Create()
        {
            var salaryExpenses = acMan.LoadAccounts(Account.AccountTypes.SalaryExpenses);
            ViewBag.SalaryExpenses = salaryExpenses;

            return View();
        }

        //
        // POST: /Salary/Create
        [HttpPost]
        public ActionResult Create(SalaryCreateModel salaryModel)
        {
            if(!ModelState.IsValid)
            {
                var salaryExpenses = acMan.LoadAccounts(Account.AccountTypes.SalaryExpenses);
                ViewBag.SalaryExpenses = salaryExpenses;

                return View(salaryModel);
            }
            string errMsg;
            if(!salaryModel.IsValid(out errMsg))
            {
                SetErrorAlert(errMsg);
                var salaryExpenses = acMan.LoadAccounts(Account.AccountTypes.SalaryExpenses);
                ViewBag.SalaryExpenses = salaryExpenses;

                return View(salaryModel);
            }

            var invoice = salaryModel.ToInvoice();
            invoice.CreatedBy = GetCurrentLoggedUser();
            invoice.CreationTime = DateTime.Now;

            invMan.AddInvoice(invoice);
            SetInfoAlert("Salary Payment added succesfully");
            return RedirectToAction("Index");

        }

       

        //
        // GET: /Salary/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Salary/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
