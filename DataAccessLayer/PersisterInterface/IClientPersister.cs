using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.PersisterInterface
{
    public interface IClientPersister : IDisposable
    {
        void AddClient(Client cli);

        void UpdateClient(Client client);
        Client LoadClient(int Id);

        /// <summary>
        /// </summary>
        /// <param name="type">null for all types</param>
        /// <returns></returns>
        IEnumerable<Client> LoadClients(Client.Types? type);


        void AddLocation(Location loc);
        Location GetLocation(int id);
        IEnumerable<Location> GetAllLocations();
        void UpdateLocation(Location loc);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationID">null for reps without a location</param>
        /// <returns></returns>
        IEnumerable<Client> GetClientsInLocation(int? locationID);


        IEnumerable<Invoice> GetClientInvoices(int clientID, DateRange range);

        decimal? GetClientDepositedAmount(int clientId);
        IDictionary<Client, decimal> GetClientDepositedAmounts(Client.Types clientType);
    }
}
