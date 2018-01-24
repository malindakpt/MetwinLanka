using BusinessLogic;
using BusinessObjects.Categories;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class AccountProfileController : GHMController
    {
        ProfileManager profMan = new ProfileManager();
        AccountManager accMan = new AccountManager();
        //
        // GET: /AccountProfile/
        public ActionResult Index()
        {
            ViewBag.Accounts = accMan.LoadAccounts(null);
            var profs = profMan.GetAccountProfiles();
            return View(profs);
        }

        //
        // GET: /AccountProfile/Details/5
        public ActionResult Details(int id)
        {
            ViewBag.Accounts = accMan.LoadAccounts(null);

            return View();
        }

        //
        // GET: /AccountProfile/Create
        public ActionResult Create()
        {
            ViewBag.Accounts = accMan.LoadAccounts(null);
            return View();
        }

        //
        // POST: /AccountProfile/Create
        [HttpPost]
        public ActionResult Create(AccountProfile profile)
        {
            if(ModelState.IsValid)
            {
                // TODO: Add insert logic here
                profMan.AddAccountProfile(profile);
                return RedirectToAction("Index");
            }
            else
            {
                return View(profile);
            }
        }

        //
        // GET: /AccountProfile/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /AccountProfile/Edit/5
        [HttpPost]
        public ActionResult Edit(AccountProfile collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        ////
        //// GET: /AccountProfile/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /AccountProfile/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}


        protected override void Dispose(bool disposing)
        {
            profMan.Dispose();
            base.Dispose(disposing);
        }
    }
}
