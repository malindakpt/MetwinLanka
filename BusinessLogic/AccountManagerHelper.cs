using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;

namespace BusinessLogic
{
    class AccountManagerHelper
    {
        AccountPersister acountPersister;
        InvoicePersister invoicePersister;

        public AccountManagerHelper()
        {
            acountPersister = new AccountPersister();
            invoicePersister = new InvoicePersister();
        }

        public IEnumerable<AccountBalance> GetAccountListBalancesByCategory(Account.AccountTypes types, DateTime from)
        {
            DateRange dr = new DateRange();
            dr.Start = from;
            dr.End =  DateTime.Now;

            IEnumerable<AccountBalance> list = acountPersister.LoadAccountBalances(types, dr);
            return list;
        }

        public IEnumerable<AccountBalance> GetAccountListByCategory(Account.AccountTypes types)
        {
            DateRange dr = new DateRange();
            dr.Start = new DateTime(2015, 1, 1);
            dr.End = DateTime.Now;

            IEnumerable<AccountBalance> list = acountPersister.LoadAccountBalances(types, dr);
            return list;
        }

        public IEnumerable<AccountBalance> GetInventoryList()
        {
            AccountBalance inventoryBal = new AccountBalance();
            Account invntryAcc = new Account(); 
            
            invntryAcc.Name = "Inventory";
           
            inventoryBal.Account = invntryAcc;
            inventoryBal.Balance = invoicePersister.GetRawMaterialBalancesValue();

            List<AccountBalance> list =  new List<AccountBalance>();
            list.Add(inventoryBal);
            return list;
        }

        public IEnumerable<AccountBalance> GetDebtors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountBalance> GetCreditors()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<AccountBalance> GetAccountBalances()
        {
            IEnumerable<AccountBalance> debitList = acountPersister.GetDebitBalancesOfAccounts(null);
            IEnumerable<AccountBalance> creditList = acountPersister.GetCreditBalancesOfAccounts(null);

            Dictionary<int, AccountBalance> debitMap = debitList.ToDictionary(ab => ab.Account.Id);
            Dictionary<int, AccountBalance> creditMap = creditList.ToDictionary(ab => ab.Account.Id);

            var allBalances = Enumerable.Concat(debitList, creditList);
            var accountIds = new HashSet<int>(allBalances.Select(i => i.Account.Id));

            List<AccountBalance> result = new List<AccountBalance>();
            foreach(int id in accountIds )
            {
                AccountBalance cr, db;
                creditMap.TryGetValue(id,out cr);
                debitMap.TryGetValue(id, out db);

                decimal cred = cr != null ? cr.Balance : 0m;
                decimal debit = db != null ? db.Balance : 0m;

                result.Add(new AccountBalance{ Account = (cr ?? db).Account  , Balance= cred - debit });
            }
            return result;
        }
    }
}
