using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class ReportController : Controller
    {
        ReportManager rm = new ReportManager();
        //
        // GET: /Report/
        public ActionResult Index()
        {
            return View();
        }

        //GET: /Report/SalesByPeriod
        public ActionResult SalesByPeriod(DateTime? fromDate, DateTime? toDate)
        {
            //date range is not selected by user... default rage is 2 months from current date
            DateTime t_date = DateTime.Today;
            DateTime f_date = t_date.AddMonths(-2);

            ViewBag.fromDate = t_date;
            ViewBag.toDate = f_date;
            //date range is selected by user
            if (fromDate.HasValue && toDate.HasValue)
            {
                t_date = toDate.Value;
                f_date = fromDate.Value;
            }
            var list = rm.getSalesperiod(f_date, t_date);

            ViewBag.From = f_date;
            ViewBag.To = t_date;
            return View(list);
        }

        //GET: /Report/PurchaseByPeriod
        public ActionResult PurchaseByPeriod(DateTime? fromDate, DateTime? toDate)
        {
            //date range is not selected by user... default rage is 2 months from current date
            DateTime t_date = DateTime.Today;
            DateTime f_date = t_date.AddMonths(-2);

            ViewBag.fromDate = t_date;
            ViewBag.toDate = f_date;
            //date range is selected by user
            if (fromDate.HasValue && toDate.HasValue)
            {
                t_date = toDate.Value;
                f_date = fromDate.Value;

            }
            var list = rm.getPurchasePeriod(f_date, t_date);


            ViewBag.From = f_date;
            ViewBag.To = t_date;


            return View(list);
        }
	}
}