using BusinessLogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GHM_ERP.Controllers
{
    public class InventoryController : Controller
    {
        //
        // GET: /Inventory/

        InvoiceManager invMan = new InvoiceManager();

        public ActionResult Index()
        {
            var rmBalances = invMan.GetRawMaterialBalances().OrderBy(rm => rm.ProfileId);
            return View(rmBalances);
        }
    }
}