using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer.PersisterInterface;
using System.Data.Entity;

namespace DataAccessLayer
{
    public class SalesRepPersister :ISalesRepPersister
    {
        AppDBContext db = new AppDBContext();

        public void AddSalesRep(SalesRep rep)
        {
            db.SalesReps.Add(rep);
            db.SaveChanges();
        }

        public void UpdateSalesRep(SalesRep rep)
        {
            db.Entry(rep).State = EntityState.Modified;
            db.SaveChanges();
        }

        public IEnumerable<SalesRep> GetAllSalesReps(bool includeDisabled = false)
        {
            var reps = db.SalesReps.AsNoTracking();
            if(!includeDisabled)
            {
                //reps = reps.Where(r => r.en)
            }

            return reps.ToList();
        }

        public SalesRep GetSalesRep(int id)
        {
            return db.SalesReps.AsNoTracking().Where(r => r.Id == id).FirstOrDefault();
        }

        public IEnumerable<Invoice> FindInvoices(int salesRep,DateRange range)
        {
            IQueryable<Invoice> invoices = db.Invoices.Include(i=>i.Client).Include(i=>i.Items).AsNoTracking()
                .Where(i => i.TransType == Invoice.TansactionTypes.Sales && i.Status == Invoice.InvoiceStatus.Approved);
            
            if (range != null)
            {
                if (range.Start.HasValue)
                {
                    invoices = invoices.Where(i => i.Time >= range.Start.Value);
                }
                if (range.End.HasValue)
                {
                    invoices = invoices.Where(i => i.Time <= range.End.Value);
                }
            }

            invoices = invoices.Where(i => i.SalesRepId == salesRep);


            return invoices.ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }


    }
}
