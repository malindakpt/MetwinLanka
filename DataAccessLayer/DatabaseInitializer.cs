using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace DataAccessLayer
{
    //DropCreateDatabaseIfModelChanges<AppDBContext>



    public class DatabaseInitializer : DropCreateDatabaseIfModelChanges<AppDBContext>
    {
        public static void RegisterInitializer()
        {
            Database.SetInitializer<AppDBContext>(new DatabaseInitializer());
        }

        protected override void Seed(AppDBContext context)
        {
            base.Seed(context);
            CreateAccounts(context);
            InitializeUsers(context);

            
        }

        private void InitializeUsers(AppDBContext context)
        {
            var roleMan = context.GetRoleManager();

            var adminRole = roleMan.Create(new IdentityRole("admin"));
            var cashierRole = roleMan.Create(new IdentityRole("cashier"));

            var userMan = context.GetUserManager();

            var admin = new User
            {
                FirstName = "Admin",
                LastName = "Admin",
                Phone = "-",
                UserName = "Admin",
            };

            var cashier = new User
            {
                FirstName = "Cashier",
                LastName = "Cashier",
                Phone = "-",
                UserName = "Cashier",
            };

            userMan.Create(admin, "admin");
            userMan.Create(cashier, "cashier");

            userMan.AddToRole(admin.Id,"admin");
            userMan.AddToRole(cashier.Id, "cashier");
            
        }
        private void CreateAccounts(AppDBContext context)
        {
            context.Accounts.Add(new Account(Account.CashAccount, "Cash", Account.AccountTypes.CurrentAssets_Cash));
            context.Accounts.Add(new Account(Account.PettyCashAccount, "Petty Cash", Account.AccountTypes.CurrentAssets_Cash));
            context.Accounts.Add(new Account(Account.BankAccount, "Bank", Account.AccountTypes.CurrentAssets_Bank));
            context.Accounts.Add(new Account(Account.PurchasesAccount, "Purchases", Account.AccountTypes.CostOfSale));
            context.Accounts.Add(new Account(Account.PurchasesReturnAccount, "Purchases Return", Account.AccountTypes.CostOfSale));
            context.Accounts.Add(new Account(Account.SalesAccount, "Sales", Account.AccountTypes.Income));
            context.Accounts.Add(new Account(Account.SalesReturnAccount, "Sales Return", Account.AccountTypes.Income));
            context.Accounts.Add(new Account(Account.CapitalAccount, "Capital", Account.AccountTypes.FixedLiabilities));
            context.Accounts.Add(new Account(Account.SalaryPayableAccount, "Salary Payable", Account.AccountTypes.CurrentLiabilities));
            context.Accounts.Add(new Account(Account.TaxPayableAccount, "Tax Payable", Account.AccountTypes.CurrentLiabilities));
            context.Accounts.Add(new Account(Account.TaxReceivableAccount, "Tax Receivable", Account.AccountTypes.CurrentAssets));
        }
    }
}
