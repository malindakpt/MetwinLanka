using BusinessLogic;
using GHM_ERP.Models;
using GHM_ERP.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    [Authorize]
    public class SettingsController : Controller
    {
        ConfigManager settingsMan = new ConfigManager();

        private SettingsModel GetCurrentSettings()
        {
            var settingsModel = new SettingsModel
            {
                VatCharge = settingsMan.VatPercentage * 100,
                Address = settingsMan.Address,
                HideUnitPriceInVat = settingsMan.HideUnitPriceInVatInvoice,
                VatNumber = settingsMan.VatNumber
            };
            return settingsModel;
        }

        // GET: Settings
        public ActionResult Index()
        {
            var settingsModel = GetCurrentSettings();
            return View(settingsModel);
        }

        [GHMAuthorize(Roles ="admin")]
        public ActionResult Edit()
        {
            var settingsModel = GetCurrentSettings();
            return View(settingsModel);
        }

        [HttpPost]
        [GHMAuthorize(Roles = "admin")]
        public ActionResult Edit(SettingsModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }
            else
            {
                settingsMan.VatPercentage = model.VatCharge / 100;
                settingsMan.VatNumber = model.VatNumber;
                settingsMan.HideUnitPriceInVatInvoice = model.HideUnitPriceInVat;
                settingsMan.Address = model.Address;
                return RedirectToAction("Index");
            }
        }
    }
}