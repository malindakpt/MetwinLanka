using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BusinessLogic;
using BusinessObjects;
using GHM_ERP.Util;


namespace GHM_ERP.Controllers
{
    public class SupplierController : GHMController
    {
        ClientManager clientManager = new ClientManager();

        //
        // GET: /Supplier/
        public ActionResult Index()
        {
            var all = clientManager.GetAllClients(Client.Types.Supplier);
            return View(all);
        }

        //
        // GET: /Supplier/Details/5
        public ActionResult Details(int id, DateTime? from, DateTime? to)
        {
            var cust = clientManager.GetClient(id);
            if(cust == null || cust.Type != Client.Types.Supplier )
            {
                return ErrorResult("Invalid Supplier ID -" + id);
            }

            //load invoices
            if (from == null || to == null)
            {
                to = DateTime.Today.AddDays(1);
                from = DateTime.Today.AddMonths(-2);
            }
            var rangestart = from.Value;
            var rangeend = to.Value;

            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.Invoices = clientManager.GetClientInvoices(id, new DateRange { Start = rangestart, End = rangeend });

            return View(cust);
        }

        //
        // GET: /Supplier/Create
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Supplier/Create
        [HttpPost]
        public ActionResult Create(Client client)
        {
            client.Type = Client.Types.Supplier;

            if(ModelState.IsValid)
            {
                clientManager.AddClient(client);
                return RedirectToAction("Index");
            }
            else
            {
                return View(client);
            }
        }

        //
        // GET: /Supplier/Edit/5
        public ActionResult Edit(int id)
        {
            var cust = clientManager.GetClient(id);
            if (cust == null || cust.Type != Client.Types.Supplier)
            {
                return ErrorResult("Invalid Supplier ID -" + id);
            }
            return View(cust);
        }

        //
        // POST: /Supplier/Edit/5
        [HttpPost]
        public ActionResult Edit(Client client)
        {
            var ori = clientManager.GetClient(client.Id);
            if (ori == null || ori.Type != Client.Types.Supplier || client.Type != ori.Type)
            {
                return ErrorResult("Invalid Supplier -" + client.Id);
            }
            else if(ModelState.IsValid)
            {
                clientManager.UpdateClient(client);
                return RedirectToAction("Index");
            }
            else
            {
                return View(client);
            }
        }

        //
        // GET: /Supplier/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Supplier/Delete/5
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
    }
}
