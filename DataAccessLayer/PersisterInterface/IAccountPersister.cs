using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using BusinessObjects;

namespace DataAccessLayer.PersisterInterface
{
    public interface IAccountPersister : IDisposable
    {
        /// <summary>
        /// Load Account balances with optional filtering by type
        /// </summary>
        /// <param name="types">Filtering type. null indicates all types</param>
        /// <returns></returns>
        IEnumerable<AccountBalance> LoadAccountBalances(Account.AccountTypes? types,DateRange range);

        /// <summary>
        /// Load Accounts with optional filtering by type
        /// </summary>
        /// <param name="types">Filtering type. null indicates all types</param>
        /// <returns></returns>
        IEnumerable<Account> LoadAccounts(Account.AccountTypes? types, bool includeDisabledAccs = false);

        /// <summary>
        /// Get Account by ID
        /// </summary>
        /// <param name="accountID"></param>
        /// <returns></returns>
        Account LoadAccount(int? accountID);

        /// <summary>
        /// Update account details
        /// </summary>
        /// <param name="account">Modified Account</param>
        void UpdateAccountDetails(Account account);

        /// <summary>
        /// Delete unused Account
        /// </summary>
        /// <param name="account"></param>
        void DeleteAccount(Account account);

        IEnumerable<AccountLink> GetLinkedAccounts(AccountLink.LinkType type);

        Account GetLinkedAccount(int mainAccountId, AccountLink.LinkType type);

        IEnumerable<AccountBalance> GetDebitBalancesOfAccounts(DateRange range);

        IEnumerable<AccountBalance> GetCreditBalancesOfAccounts(DateRange range);

        void CreateAccount(Account account);

        void ChangeAccountCode(int accountId, string newCode);

    }
}
