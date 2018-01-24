using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using DataAccessLayer;
using BusinessObjects;
namespace BusinessLogic
{
    public class SalesRepManager : IDisposable
    {
        ISalesRepPersister db;

        public SalesRepManager(ISalesRepPersister persister = null)
        {
            db = persister ?? new SalesRepPersister();
        }

        public void AddSalesRep(SalesRep rep)
        {
            db.AddSalesRep(rep);
        }

        public void UpdateSalesRep(SalesRep rep)
        {
            db.UpdateSalesRep(rep);
        }

        public IEnumerable<SalesRep> GetAllSalesReps(bool includeDisabled = false)
        {
            return db.GetAllSalesReps(includeDisabled);
        }

        public SalesRep GetSalesRep(int id)
        {
            return db.GetSalesRep(id);
        }

        public IEnumerable<SalesRepComis> FindRelatedInvoices(int repId, DateTime from, DateTime to)
        {
            var invoices = db.FindInvoices(repId, new DateRange { End = to, Start = from });
            var infos = invoices.Select(i => new SalesRepComis
                {
                    Client = i.Client.Name,
                    inv_amount = i.GetTotal(),
                    inv_date = i.Time,
                    inv_no = i.Id,
                    DisplayInvNo = i.FormattedInvoiceNo
                });

            return infos.OrderBy(i => i.inv_date);
        }

      

        public void Dispose()
        {
            db.Dispose();
        }
    }
}
