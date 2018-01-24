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
    public class RawMaterialProfileController : GHMController
    {

        ProfileManager profileManager;

        public RawMaterialProfileController()
        {
            profileManager = new ProfileManager();
        }


        //
        // GET: /RawMaterialProfile/
        public ActionResult Index()
        {
            var profs = profileManager.GetRawMaterialProfiles(true);
            return View(profs);
        }

        //
        // GET: /RawMaterialProfile/Details/5
        public ActionResult Details(int id)
        {
            var prof = profileManager.GetRawMaterialProfile(id);
            if(prof == null)
            {
                return ErrorResult("Raw Material Profile doesn't exist -" + id);
            }
            return View(prof);
        }

        //
        // GET: /RawMaterialProfile/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /RawMaterialProfile/Create
        [HttpPost]
        public ActionResult Create(RawMaterialProfile prof)
        {
            if(ModelState.IsValid)
            {
                profileManager.AddRawMaterialProfile(prof);
                return RedirectToAction("Index");
            }
            else
            {
                return View(prof);
            }
        }

        //
        // GET: /RawMaterialProfile/Edit/5
        public ActionResult Edit(int id)
        {
            var prof = profileManager.GetRawMaterialProfile(id);
            if (prof == null)
            {
                return ErrorResult("Raw Material Profile doesn't exist -" + id);
            }
            return View(prof);
        }

        //
        // POST: /RawMaterialProfile/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, RawMaterialProfile prof)
        {
            if (ModelState.IsValid)
            {
                profileManager.UpdateRawMaterialProfile(prof);
                return RedirectToAction("Index");
            }
            else
            {
                return View(prof);
            }
        }

        ////
        //// GET: /RawMaterialProfile/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /RawMaterialProfile/Delete/5
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
    }
}
