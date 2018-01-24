using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using BusinessObjects;
using System.Collections;
using NLog;
using BusinessObjects.Accounting;
using System.Data.Entity;

namespace DataAccessLayer
{
    public class AccountPersister: IAccountPersister
    {
        private AppDBContext context = new AppDBContext();
        Logger logger = LogManager.GetCurrentClassLogger();

        private const int CustomAccountSequenceStart = 100; 

        public AccountBalance LoadAccoutBalance(int accountId,DateRange range)
        {
            var approved = context.Transactions.Where(tr => tr.InvoiceRef.Status == Invoice.InvoiceStatus.Approved);

            if (range != null)
            {
                if (range.Start.HasValue)
                {
                    approved = approved.Where(tr => tr.Time >= range.Start.Value);
                }
                if (range.End.HasValue)
                {
                    approved = approved.Where(tr => tr.Time <= range.End.Value);
                }
            }

            
            
           var debits = approved.Where(tr => tr.DeAcc.Id == accountId);
           var credits = approved.Where(tr => tr.CrAcc.Id == accountId);

           var debitSum = debits.Sum(tr => (decimal?)tr.Amount) ?? 0m ;
           var creditSum = credits.Sum(tr => (decimal?)tr.Amount) ?? 0m;

           var ac = LoadAccount(accountId);
           decimal creditBalance = creditSum - debitSum;
           decimal balance = ac.IsDebit ? -creditBalance : creditBalance;

           return new AccountBalance { Account = ac, Balance = balance };

        }

        public IEnumerable<AccountBalance> LoadAccountBalances(Account.AccountTypes? types,DateRange range)
        {
            var creditBalances = GetBalancesOfAccounts(range, false, types).ToDictionary(cb => cb.Account.Id);
            var debitBalances = GetBalancesOfAccounts(range, true, types).ToDictionary(db => db.Account.Id);

            var allIds = Enumerable.Concat(creditBalances.Keys, creditBalances.Keys).Distinct().ToList();

            List<AccountBalance> result = new List<AccountBalance>();

            foreach (var id in allIds)
            {
                AccountBalance acCred = null, acDeb = null;
                creditBalances.TryGetValue(id, out acCred);
                debitBalances.TryGetValue(id, out acDeb);

                Account ac = null;
                decimal credits = 0, debits = 0;

                if (acCred != null)
                {
                    ac = acCred.Account;
                    credits = acCred.Balance;
                }
                if (acDeb != null)
                {
                    ac = acDeb.Account;
                    debits = acDeb.Balance;
                }

                decimal creditBalance = credits - debits;
                decimal balance = ac.IsDebit ? - creditBalance : creditBalance;
                result.Add(new AccountBalance { Account = ac, Balance = balance });
            }

            return result;
        }


        public IEnumerable<AccountRecord> AccountRecords(DateTime from, DateTime to, int? acc)
        {
            var approved = context.Transactions.Include(tr=>tr.InvoiceRef).Where(tr => tr.InvoiceRef.Status == Invoice.InvoiceStatus.Approved)
                .Where(tr => tr.Time >= from && tr.Time <= to)
                .Where(tr => tr.CrAccId == acc || tr.DeAccId == acc)
                .ToList();

            var records = approved.Select(tr =>
                            new AccountRecord(tr.Id, tr.Time, tr.InvoiceRef.Description +" | " +tr.Description, tr.DeAccId == acc ? tr.Amount : 0m, tr.CrAccId == acc ? tr.Amount : 0m));

            return records.ToList();


        }
 
        public IEnumerable<Account> LoadAccounts(Account.AccountTypes? types, bool includeDisabledAccs = false)
        {

            IQueryable<Account> accs = context.Accounts;
            if(types.HasValue)
            {
                accs = accs.Where(ac => ac.AccountType == types.Value);
            }
            if(!includeDisabledAccs)
            {
                accs.Where(ac => ac.IsEnabled == true);
            }

            return accs.ToList();

        }
        public  Account LoadAccount(int? accountID)
        {
            Account ac = context.Accounts.Where(a => a.Id == accountID).FirstOrDefault(); 
            return ac;
        }

        public void UpdateAccountDetails(Account account)
        {
            throw new NotImplementedException();
        }

        public void DeleteAccount(Account account)
        {
            throw new NotImplementedException();
        }


        public IEnumerable<AccountLink> GetLinkedAccounts(AccountLink.LinkType type)
        {
            return context.AccountLinks.Where(link => link.Link == type).ToList();
        }

        public void Dispose()
        {
            context.Dispose();
        }
 

        public Account GetLinkedAccount(int mainAccountId, AccountLink.LinkType type)
        {
            AccountLink link = context.AccountLinks.Where(l => l.MainAccountId == mainAccountId && l.Link == type)
                                                .FirstOrDefault();

            Account ac = link != null ? link.MainAccount : null;
            return ac;
        }

        private IEnumerable<AccountBalance> GetBalancesOfAccounts (DateRange range, bool debitBalance,Account.AccountTypes? type = null)
        {
            var accountList = LoadAccounts(type, true).ToList();
            var accountMap = accountList.ToDictionary(ac => ac.Id);

            var approved = context.Transactions.Where(tr => tr.InvoiceRef.Status == Invoice.InvoiceStatus.Approved);

            if(range != null)
            {
                if(range.Start.HasValue )
                {
                    approved = approved.Where(tr => tr.Time >= range.Start.Value);
                }
                if(range.End.HasValue)
                {
                    approved = approved.Where(tr => tr.Time <= range.End.Value);
                }
            }

            if(type.HasValue)
            {
                if(debitBalance)
                {
                    approved = approved.Where(tr => tr.DeAcc.AccountType == type);
                }
                else
                {
                    approved = approved.Where(tr => tr.CrAcc.AccountType == type);
                }
                
            }

            var grouped = approved.GroupBy(tr => tr.CrAccId);

            if(debitBalance)
            {
                grouped = approved.GroupBy(tr => tr.DeAccId);
            }


            var summed = grouped.Select(g => new { Id = g.Key, Sum = g.Sum(t => t.Amount) })
                                .ToDictionary(item => item.Id);

            //var mapped = summed.Select(item => new AccountBalance { Account = accountMap[item.Id], Balance = item.Sum });
            var resultList = new List<AccountBalance>();
            foreach (var acc in accountList)
            {
                var accBal = new AccountBalance { Account = acc };
                accBal.Balance = summed.ContainsKey(acc.Id) ? summed[acc.Id].Sum : 0m;

                resultList.Add(accBal);
            }

            return resultList;
        }

        public IEnumerable<AccountBalance> GetDebitBalancesOfAccounts(DateRange range)
        {
            return GetBalancesOfAccounts(range, true);
        
        }
        public IEnumerable<AccountBalance> GetCreditBalancesOfAccounts(DateRange range)
        {
            return GetBalancesOfAccounts(range, false);
        }

        public IEnumerable<AccountBalance> results { get; set; }


        public void CreateAccount(Account account)
        {
            if(account.Id == 0)
            {
                account.Id = GetNextAccountId();
            }
            ValidateAccount(account);
            context.Accounts.Add(account);
            context.SaveChanges();
        }

        private int GetNextAccountId()
        {
            var max= context.Accounts.Max(ac => ac.Id);
            if(max < CustomAccountSequenceStart )
            {
                return CustomAccountSequenceStart;
            }
            else
            {
                return max + 1;
            }
        }

        private void ValidateAccount(Account account)
        {
            if (account.AccountType == Account.AccountTypes.CurrentAssets_Customer ||
                    account.AccountType == Account.AccountTypes.CurrentLiability_Supplier)
            {
                if (account.Client == null)
                {
                    throw new ArgumentException("Client is required for customer and supplier types");
                }
            }
            else
            {
                if (account.Client != null)
                {
                    throw new ArgumentException("Client is not supported for this account - " + account.AccountType);
                }
            }
        }


        public void ChangeAccountCode(int accountId, string newCode)
        {
            var account = context.Accounts.First(ac => ac.Id == accountId);
            account.Code = newCode;
            context.SaveChanges();
        }
    }
}
