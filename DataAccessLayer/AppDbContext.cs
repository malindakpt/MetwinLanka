using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System.Data.Entity;


namespace DataAccessLayer
{
    public class AppDBContext : IdentityDbContext<User>
    {
        public UserManager<User> GetUserManager()
        {
            var man = new UserManager<User>(new UserStore<User>(this));
            man.PasswordValidator = new MinimumLengthValidator(3);
            return man;
            
        }

        public RoleManager<IdentityRole> GetRoleManager()
        {
            return new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(this));
        }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<AccountLink> AccountLinks { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<ItemProfile> ItemProfiles { get; set; }
        public DbSet<Cheque> Cheques { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Item> Items { get; set; }
        public DbSet<PaymentReceipt> PaymentReceipts { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<SalesRep> SalesReps { get; set; }
        public DbSet<Util.AppSetting> AppSettings { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Transaction>().HasRequired(t => t.DeAcc)
                .WithMany().WillCascadeOnDelete(false);
        }

        public System.Data.Entity.DbSet<BusinessObjects.Accounting.AccountRecord> AccountRecords { get; set; }

    }
}
