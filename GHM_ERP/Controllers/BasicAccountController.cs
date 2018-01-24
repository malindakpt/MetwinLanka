using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class BasicAccountController : Controller
    {
        ReportManager rm = new ReportManager();
        // GET: /BasicAccount/
        public ActionResult BalanceSheet()
        {
            var bal =rm.PrepareBalanceSheet();
            bal.date = DateTime.Now;
            return View(bal);
        }

        public ActionResult IncomeStatement(DateTime? mydate)
        {
            var incomestmt = rm.prepareInStmt(new DateTime(2000,1,1));
            if(mydate.HasValue)
            {
                incomestmt = rm.prepareInStmt(mydate.Value);
                return View(incomestmt);
            }
            return View(incomestmt);
        }

        public ActionResult TrialBalance(DateTime? mydate)
        {
            var tb = rm.prepareTrilaBalance(new DateTime(1990, 1, 1));

            if (mydate.HasValue)
            {
                tb = rm.prepareTrilaBalance(mydate.Value);
                return View(tb);
            }
            return View(tb);
        }
    }
}