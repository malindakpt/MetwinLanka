using BusinessLogic;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessObjects;

namespace GHM_ERP.Controllers
{
    public class SalesRepController : GHMController
    {
        private SalesRepManager manager = new SalesRepManager();

        //
        // GET: /SalesRep/
        public ActionResult Index()
        {
            var all = manager.GetAllSalesReps();
            return View(all);
        }

        //
        // GET: /SalesRep/Details/5
        public ActionResult Details(int id)
        {
            var rep = manager.GetSalesRep(id);
            if(rep == null)
            {
                return ErrorResult("Invalid Sales Rep ID -" + id);
            }
            return View(rep);
        }

        //
        // GET: /SalesRep/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /SalesRep/Create
        [HttpPost]
        public ActionResult Create(SalesRep rep)
        {
            if(ModelState.IsValid)
            {
                manager.AddSalesRep(rep);
                return RedirectToAction("Index");
            }
            else
            {
                return View(rep);
            }
        }

        //
        // GET: /SalesRep/Edit/5
        public ActionResult Edit(int id)
        {
            var rep = manager.GetSalesRep(id);
            if (rep == null)
            {
                return ErrorResult("Invalid Sales Rep ID -" + id);
            }
            return View(rep);
        }

        //
        // POST: /SalesRep/Edit/5
        [HttpPost]
        public ActionResult Edit(SalesRep rep)
        {
            var old = manager.GetSalesRep(rep.Id);

            if (rep == null)
            {
                return ErrorResult("Invalid Sales Rep ID -" + rep.Id);
            }
            else if(! ModelState.IsValid)
            {
                return View(rep);
            }
            else
            {
                manager.UpdateSalesRep(rep);
                return RedirectToAction("Index");
            }

        }

        //
        // GET: /SalesRep/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /SalesRep/Delete/5
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

        //
        //GET: /SalesRep/GetCommision
        [HttpGet]
        public ActionResult Commisions(int? id, DateTime? fromDate, DateTime? toDate)
        {
            int Repid = 0;
            var all = manager.GetAllSalesReps();
            ViewBag.SalesReps = all;

            //date range is not selected by user... default rage is 2 months from current date
            DateTime t_date = DateTime.Today;
            DateTime f_date = t_date.AddMonths(-2);

            //date range is selected by user
            if (fromDate.HasValue && toDate.HasValue)
            {
                t_date = toDate.Value;
                f_date = fromDate.Value;
            }

            ViewBag.fromDate = f_date;
            ViewBag.toDate = t_date;

            if (id.HasValue)
            {
                Repid = (int)id;
                var list = manager.FindRelatedInvoices(Repid, f_date, t_date);
                ViewBag.RepId = Repid;
                ViewBag.Rep = all.First(sr => sr.Id == Repid);
                return View(list);
            }

            else
            {
                ViewBag.RepId = Repid;
                return View();
            }

        }


        protected override void Dispose(bool disposing)
        {
            manager.Dispose();
            base.Dispose(disposing);
        }

    }
}
