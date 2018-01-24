using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using BusinessObjects;
using System.Data.Entity;

namespace DataAccessLayer
{
    public class ClientPersister : IClientPersister
    {
        AppDBContext db = new AppDBContext();

        public void AddClient(Client cli)
        {
            //TODO: Create account 
            db.Clients.Add(cli);
            db.SaveChanges();
        }

        public Client LoadClient(int Id)
        {
            return db.Clients.AsNoTracking().Where(c => c.Id == Id).FirstOrDefault();
        }

        public void UpdateClient(Client client)
        {
            db.Entry(client).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<Client> LoadClients(Client.Types? type)
        {
            IQueryable<Client> clients = db.Clients.AsNoTracking().Include(cl=>cl.Location) ;
            if(type.HasValue)
            {
                clients = clients.Where(cl => cl.Type == type.Value);
            }
            return clients.ToList();
        }

        public IEnumerable<Invoice> GetClientInvoices(int clientID, DateRange range)
        {
            //var client = db.Clients.Include(cl => cl.Invoices).Include(cl => cl.Invoices.Select(inv => inv.PaymentReceipts))
            //    .Include(cl => cl.Invoices.Select(inv => inv.Items))
            //    .FirstOrDefault(cl => cl.Id == clientID);
            //if(client == null)
            //{
            //    return null;
            //}

            var invs = db.Invoices.Include(inv => inv.PaymentReceipts)
                                    .Include(inv => inv.Items)
                                    .Where(inv => inv.ClientId == clientID);
            if(range != null)
            {
                if(range.Start.HasValue)
                {
                    invs = invs.Where(inv => inv.Time >= range.Start.Value);
                }
                if(range.End.HasValue)
                {
                    invs = invs.Where(inv => inv.Time <= range.End.Value);
                }
            }

            return invs.ToList();
        }


        public void AddLocation(Location loc)
        {
            db.Locations.Add(loc);
            db.SaveChanges();
        }

        public Location GetLocation(int id)
        {
            return db.Locations.AsNoTracking().FirstOrDefault(l => l.Id == id);
        }

        public IEnumerable<Location> GetAllLocations()
        {
            return db.Locations.Include(c=>c.Clients).ToList();
        }

        public IEnumerable<Client> GetClientsInLocation(int? locationID)
        {
            return db.Clients.Where(c => c.LocationId == locationID).ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public void UpdateLocation(Location location)
        {
            //var loc = db.Locations.First(l => l.Id == location.Id);
            //loc.Name = location.Name;
            db.Entry(location).State = EntityState.Modified;
            db.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns>null if client doesn't exist</returns>
        public decimal? GetClientDepositedAmount(int clientId)
        {
            if (LoadClient(clientId) == null)
                return null;

            var deposits = db.Invoices.Where(inv => inv.TransType == Invoice.TansactionTypes.Deposit && inv.ClientId == clientId && inv.Status == Invoice.InvoiceStatus.Approved)
                        .SelectMany(inv => inv.PaymentReceipts).Sum(recp => recp.Amount);
            var withdrawals = db.Invoices.Where(inv => inv.Status == Invoice.InvoiceStatus.Approved && inv.ClientId == clientId)
                        .SelectMany(inv => inv.PaymentReceipts).Where(recp => recp.PaymentMethod == PaymentReceipt.PaymentType.FromDeposit)
                        .Sum(recp => (decimal?)recp.Amount);

            return deposits - (withdrawals?? 0m);
        }

        public IDictionary<Client, decimal> GetClientDepositedAmounts(Client.Types clientType)
        {
            var depositsGroupedById = db.Invoices.Where(inv => inv.TransType == Invoice.TansactionTypes.Deposit && inv.Client != null&& inv.Client.Type == clientType
                                                            && inv.Status == Invoice.InvoiceStatus.Approved)
                                .GroupBy(inv => inv.ClientId, inv => inv.PaymentReceipts.Sum(rc =>rc.Amount));

            var clientDeposits = depositsGroupedById.ToDictionary(grp => grp.Key.Value, grp => grp.Sum());


            var withdrawalsById = db.Invoices.Where(inv => inv.Status == Invoice.InvoiceStatus.Approved && inv.Client != null && inv.Client.Type == clientType)
                        .GroupBy(inv => inv.ClientId, inv => inv.PaymentReceipts.Where(recp => recp.PaymentMethod == PaymentReceipt.PaymentType.FromDeposit).Sum(rec => (decimal?)rec.Amount));

            var clientWithdrawals = withdrawalsById.ToDictionary(grp => grp.Key.Value, grp => grp.Sum() ?? 0m);

            var allClients = LoadClients(clientType);

            var availableDeposit = new Dictionary<Client, decimal>();
            foreach (var client in allClients)
            {
                var deposit = 0m;
                if(clientDeposits.ContainsKey(client.Id))
                {
                    deposit += clientDeposits[client.Id];
                }
                if(clientWithdrawals.ContainsKey(client.Id))
                {
                    deposit -= clientWithdrawals[client.Id];
                }
                availableDeposit[client] = deposit;
            }

            return availableDeposit;
        }
    }
}
