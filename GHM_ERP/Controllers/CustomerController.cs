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
    public class CustomerController : GHMController
    {
        ClientManager clientManager = new ClientManager();

        //
        // GET: /Customer/
        public ActionResult Index()
        {
            var all = clientManager.GetAllClients(Client.Types.Customer);
            return View(all);
        }

        //
        // GET: /Customer/Details/5
        public ActionResult Details(int id, DateTime? from, DateTime? to)
        {
            var cust = clientManager.GetClient(id);
            if (cust == null)
            {
                return ErrorResult("Invalid Customer ID -" + id);
            }

            //load invoices
            if (from == null || to == null)
            {
                to = DateTime.Today.AddDays(1).Date;
                from = DateTime.Today.AddMonths(-2);
            }
            var rangestart = from.Value;
            var rangeend = to.Value.AddDays(1).Date;

            ViewBag.From = from;
            ViewBag.To = to;
            ViewBag.Invoices = clientManager.GetClientInvoices(id, new DateRange { Start = rangestart, End = rangeend });
            return View(cust);
        }

        //
        // GET: /Customer/Create
        public ActionResult Create()
        {
            ViewBag.Locations = clientManager.GetAllLocations();
            return View();
        }

        //
        // POST: /Customer/Create
        [HttpPost]
        public ActionResult Create(Client client)
        {
            client.Type = Client.Types.Customer;

            if (ModelState.IsValid)
            {
                clientManager.AddClient(client);
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Locations = clientManager.GetAllLocations();
                return View(client);
            }
        }

        //
        // GET: /Customer/Edit/5
        public ActionResult Edit(int id)
        {
            var cust = clientManager.GetClient(id);
            if (cust == null)
            {
                return ErrorResult("Invalid Customer ID -" + id);
            }
            ViewBag.Locations = clientManager.GetAllLocations();
            return View(cust);
        }

        //
        // POST: /Customer/Edit/5
        [HttpPost]
        public ActionResult Edit(Client client)
        {
            var ori = clientManager.GetClient(client.Id);
            if (ori == null || ori.Type != Client.Types.Customer || client.Type != ori.Type)
            {
                return ErrorResult("Invalid Customer -" + client.Id);
            }
            else if (ModelState.IsValid)
            {

                var newCliMan = new ClientManager();
                newCliMan.UpdateClient(client);
                newCliMan.Dispose();
                return RedirectToAction("Index");
            }
            else
            {
                ViewBag.Locations = clientManager.GetAllLocations();
                return View(client);
            }
        }

        //
        // GET: /Customer/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Customer/Delete/5
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

        public ActionResult Locations()
        {
            var locations = clientManager.GetAllLocations();
            var unlocatedClients = clientManager.GetClientsInLocation(null, Client.Types.Customer).Count();
            ViewBag.UnlocatedClientCount = unlocatedClients;
            return View(locations);
        }

        [HttpPost]
        public ActionResult LocationAdd(string locationName)
        {
            if (!string.IsNullOrWhiteSpace(locationName) && locationName.Length <= 75)
            {
                Location loc = new Location
                {
                    Name = locationName,
                    District = Location.Districts.None
                };
                clientManager.AddLocation(loc);
            }
            return RedirectToAction("Locations");
        }

        public ActionResult Loc(int id)
        {
            var loc = clientManager.GetAllLocations().FirstOrDefault(l => l.Id == id);
            if (loc == null)
            {
                return ErrorResult("Cannot find the location");
            }
            var clients = clientManager.GetClientsInLocation(id, Client.Types.Customer);
            loc.Clients = clients.ToList();
            return View(loc);
        }

        [HttpPost]
        public ActionResult Loc(int id,string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return RedirectToAction("Loc", new { id = id });
            }

            var loc = clientManager.GetAllLocations().FirstOrDefault(l => l.Id == id);
            if(loc == null)
            {
                return ErrorResult("Cannot find the location");
            }
            loc.Name = name;
            clientManager.UpdateLocation(loc);

            return RedirectToAction("Loc", new { id = id });
        }

        protected override void Dispose(bool disposing)
        {
            clientManager.Dispose();
            base.Dispose(disposing);
        }

    }
}
