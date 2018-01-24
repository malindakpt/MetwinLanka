using BusinessLogic;
using BusinessObjects.Categories;
using DataAccessLayer;
using DataAccessLayer.PersisterInterface;
using GHM_ERP.Models;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class ProductProfileController : GHMController
    {
        ProfileManager profileManager;

        public ProductProfileController()
        {
            profileManager = new ProfileManager(); 
        }

        //
        // GET: /ProductProfile/
        public ActionResult Index()
        {
            var prods = profileManager.GetProductProfiles(true);
            return View(prods);
        }

        //
        // GET: /ProductProfile/Details/5
        public ActionResult Details(int id)
        {
            var prod = profileManager.GetProductProfile(id);
            if(prod == null)
            {
                return ErrorResult("Product Profile doesn't exist -" + id);
            }
            return View(prod);
        }

        //
        // GET: /ProductProfile/Create
        public ActionResult Create()
        {
            ViewBag.RawMaterialProfs = profileManager.GetRawMaterialProfiles();
            return View();
        }

        //
        // POST: /ProductProfile/Create
        [HttpPost]
        public ActionResult Create(ProductProfileEditModel prof)
        {
            IEnumerable<RawMaterialProfile> rawProfs = profileManager.GetRawMaterialProfiles();;
            if(ModelState.IsValid)
            {
                profileManager.AddProductProfile(prof.ToProductProfile(rawProfs));
                return RedirectToAction("Index");
            }
            else 
            {
                ViewBag.RawMaterialProfs = rawProfs;
                return View(prof);
            }
        }

        //
        // GET: /ProductProfile/Edit/5
        public ActionResult Edit(int id)
        {
            var prod = profileManager.GetProductProfile(id);
            if (prod == null)
            {
                return ErrorResult("Product Profile doesn't exist -" + id);
            }
            ViewBag.RawMaterialProfs = profileManager.GetRawMaterialProfiles();
            return View(prod);
        }

        //
        // POST: /ProductProfile/Edit/5
        [HttpPost]
        public ActionResult Edit(ProductProfileEditModel editProf)
        {
            var rmprofs = profileManager.GetRawMaterialProfiles();

            var prof = editProf.ToProductProfile(rmprofs);
            var oriprof = profileManager.GetProductProfile(prof.Id);

            if (oriprof == null)
            {
                return ErrorResult("Product Profile doesn't exist -" + prof.Id);
            }


            if( ModelState.IsValid)
            {
                profileManager.UpdateProductProfile(prof);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.RawMaterialProfs = rmprofs;
                return View(prof);
            }
        }

        ////
        //// GET: /ProductProfile/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        //
        // POST: /ProductProfile/Delete/5
        [HttpPost]
        public ActionResult Delete(int id)
        {
            try
            {
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
