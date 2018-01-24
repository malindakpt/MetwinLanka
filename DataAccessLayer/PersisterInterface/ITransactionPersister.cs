using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace DataAccessLayer.PersisterInterface
{
    interface ITransactionPersister: IDisposable
    {
        Transaction LoadTransaction(int Id);


        IEnumerable<Transaction> GetApprovedTransactions();
    }
}
