using BusinessObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.PersisterInterface
{
    public interface ISalesRepPersister : IDisposable
    {
        void AddSalesRep(SalesRep rep);
        void UpdateSalesRep(SalesRep rep);
        IEnumerable<SalesRep> GetAllSalesReps(bool includeDisabled = false);
        SalesRep GetSalesRep(int id);
        IEnumerable<Invoice> FindInvoices(int salesRep, DateRange range);
    }
}
