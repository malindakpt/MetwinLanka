using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;

namespace DataAccessLayer
{
    public class TransactionPersister : ITransactionPersister
    {

        public BusinessObjects.Transaction LoadTransaction(int Id)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }


        public IEnumerable<BusinessObjects.Transaction> GetApprovedTransactions()
        {
            throw new NotImplementedException();
        }
    }
}
