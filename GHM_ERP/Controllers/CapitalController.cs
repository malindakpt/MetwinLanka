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
    public class CapitalController : GHMController
    {
        AccountManager acMan = new AccountManager();
        InvoiceManager invMan = new InvoiceManager();

        //
        // GET: /Capital/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Push()
        {
            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank)
                .Where(ac => ac.Id != Account.BankAccount).ToList(); 

            return View();
        }

        [HttpPost]
        public ActionResult Push(PushCapitalModel model)
        {
            if(ModelState.IsValid)
            {
                var invoice = model.ToInvoice();
                invoice.CreatedBy = GetCurrentLoggedUser();
                invoice.CreationTime = DateTime.Now;
                invMan.AddInvoice(invoice);

                TempData["InfoMsg"] = "Capital added successfully";
                return RedirectToAction("Push");
            }

            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank)
                .Where(ac => ac.Id != Account.BankAccount).ToList(); 

            return View();
        }

        public ActionResult Pop()
        {
            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank)
                .Where(ac => ac.Id != Account.BankAccount).ToList(); 

            return View();
        }

        [HttpPost]
        public ActionResult Pop(PopCapitalModel model)
        {
            if (ModelState.IsValid)
            {
                var invoice = model.ToInvoice();
                invoice.CreatedBy = GetCurrentLoggedUser();
                invoice.CreationTime = DateTime.Now;
                invMan.AddInvoice(invoice);

                TempData["InfoMsg"] = "Capital withdrawn successfully";
                return RedirectToAction("Push");
            }

            ViewBag.BankAccounts = acMan.LoadAccounts(Account.AccountTypes.CurrentAssets_Bank)
                .Where(ac => ac.Id != Account.BankAccount).ToList();

            return View();
        }

    }
}