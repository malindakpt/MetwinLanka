using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BusinessObjects.Accounting;
using DataAccessLayer;
using BusinessObjects;
using GHM_ERP.Models;
using GHM_ERP.Util;
using BusinessLogic;

namespace GHM_ERP.Controllers
{
    public class AccountingController : GHMController
    {
        private AppDBContext db = new AppDBContext();
        private AccountPersister accPersister = new AccountPersister();
        private AccountManager accMan = new AccountManager();

        // GET: /Accounting/
        public ActionResult Index(int? id, DateTime? fromdate, DateTime? todate)
        {
            IEnumerable<Account> accountList = accPersister.LoadAccounts(null, false);
            ViewBag.accountList = accountList;

            //date range is not selected by user... default rage is 2 months from current date
            DateTime t_date = DateTime.Today.AddDays(1);
            DateTime f_date = t_date.AddMonths(-2);

            //date range is selected by user
            if (fromdate.HasValue && todate.HasValue)
            {
                t_date = todate.Value;
                f_date = fromdate.Value;
            }

            ViewBag.FromDate = f_date;
            ViewBag.ToDate = t_date;

            if (id.HasValue)
            {
                ViewBag.accountName = accountList.First(account => account.Id == id.Value).Name;
                ViewBag.pageId = id;
                return View(accPersister.AccountRecords(f_date, t_date, id.Value));
            }
            else
            {
                id = 1;
                ViewBag.accountName = accountList.First(account => account.Id == id.Value).Name;
                ViewBag.pageId = id;
                return View(accPersister.AccountRecords(f_date, t_date, id.Value));
            }
                
            //return View();
        }

        // GET: /Accounting/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountRecord accountrecord = db.AccountRecords.Find(id);
            if (accountrecord == null)
            {
                return HttpNotFound();
            }
            return View(accountrecord);
        }

        // GET: /Accounting/Create
        public ActionResult Create(Account.AccountTypes? lastType)
        {
            IDictionary<Account.AccountTypes, string> accTypes = new Dictionary<Account.AccountTypes, string> {
                { Account.AccountTypes.AdministrationExpenses, "Administration Expenses"},
                {Account.AccountTypes.CurrentAssets_Bank,"Bank"},
               // {Account.AccountTypes.CurrentAssets_Cash,"Cash"},
                {Account.AccountTypes.SellingAndDistributionExpenses,"Selling and Distribution Expenses"},
                {Account.AccountTypes.SalaryExpenses, "Salary Expenses"}
            };
            ViewBag.accTypes = accTypes;
            ViewBag.LastType = lastType.HasValue ? lastType.Value : Account.AccountTypes.AdministrationExpenses;
            
            return View();
        }

        // POST: /Accounting/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult Create([Bind(Include = "Name,AccountType")] BusinessObjects.Account account)
        {
            if (ModelState.IsValid)
            {
                accPersister.CreateAccount(account);
                SetInfoAlert(string.Format("Account '{0}' created successfully", account.Name));
                return RedirectToAction("Create", new { lastType = account.AccountType });
            }

            return View(account);
        }

        // GET: /Accounting/Edit/5
        public ActionResult EditCodes()
        {
            IEnumerable<Account> accountList = accPersister.LoadAccounts(null,false);
            var sortedAccs = accountList.OrderBy(ac => ac.Code).ToList();
            return View(sortedAccs);
        }

        // POST: /Accounting/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        public ActionResult EditCodes(AccountingCodeEditModel editModel)
        {
            if (!ModelState.IsValid)
            {
                return ErrorResult("Invalid data for codes");
            }
            var changedItems = editModel.EditItems.Where(cei => cei.IsChanged).ToList();
            if(! changedItems.Any())
            {
                //No changes in codes
                return RedirectToAction("Index");
            }
            foreach (var item in changedItems)
            {
                accMan.ChangeAccountCode(item.AccountId, item.NewCode);
            }
            TempData["InfoMsg"] = "Account codes changed successfully";

            return RedirectToAction("EditCodes");
        }

        // GET: /Accounting/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountRecord accountrecord = db.AccountRecords.Find(id);
            if (accountrecord == null)
            {
                return HttpNotFound();
            }
            return View(accountrecord);
        }

        // POST: /Accounting/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountRecord accountrecord = db.AccountRecords.Find(id);
          //  db.AccountRecords.Remove(accountrecord);
         //   db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AccountSummary(int id, DateTime from, DateTime to)
        {

            return View();
        }
    }
}
