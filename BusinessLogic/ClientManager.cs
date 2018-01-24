using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using DataAccessLayer;

namespace BusinessLogic
{
    public class ClientManager : IDisposable
    {
        IClientPersister db;

        public ClientManager(IClientPersister persister = null)
        {
            db = persister ?? new ClientPersister();
        }

        public void AddClient(Client cli)
        {
            var accDb = new AccountManager();
            var acctype = (cli.Type == Client.Types.Customer) ? Account.AccountTypes.CurrentAssets_Customer : Account.AccountTypes.CurrentLiability_Supplier;
            var acc = accDb.CreateAccount(cli.Name, acctype,
                            cli.Name, cli.Type, cli.Phone, cli.Address,cli.LocationId);
            //cli.Id = acc.Id;
            //db.AddClient(cli);
        }

        public void UpdateClient(Client cli)
        {
            db.UpdateClient(cli);
        }

        public Client GetClient(int id)
        {
            return db.LoadClient(id);
        }

        public IEnumerable<Client> GetAllClients(Client.Types? type)
        {
            return db.LoadClients(type);
        }


        public IEnumerable<Location> GetAllLocations()
        {
            return db.GetAllLocations();
        }

        public void AddLocation(Location loc)
        {
            db.AddLocation(loc);
        }

        public void UpdateLocation(Location loc)
        {
            db.UpdateLocation(loc);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loc">null for clients without a location set</param>
        /// <returns></returns>
        public IEnumerable<Client> GetClientsInLocation(int? loc, Client.Types? type)
        {
            var clients = db.GetClientsInLocation(loc);
            if(type.HasValue)
            {
                clients = clients.Where(c => c.Type == type.Value);
            }
            return clients;
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IEnumerable<Invoice> GetClientInvoices(int clientID, DateRange range)
        {
            return db.GetClientInvoices(clientID, range);
        }

        public IDictionary<Client,decimal> GetClientsAvailableDeposits(Client.Types type)
        {
            return db.GetClientDepositedAmounts(type);
        }

        public decimal? GetClientAvailableDeposits(int clientId)
        {
            return db.GetClientDepositedAmount(clientId);
        }
    }
}
