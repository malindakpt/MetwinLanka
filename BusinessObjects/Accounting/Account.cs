using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
   
    public class Account
    {
        public const int CashAccount = 1;//CurrentAssets_Cash
        public const int PettyCashAccount = 2;//CurrentAssets_Cash
        public const int BankAccount = 3;//CurrentAssets_Bank
        public const int PurchasesAccount = 4; //CostOfSale  isDebit=true
        public const int PurchasesReturnAccount = 5;//CostOfSale    isDebit=false
        public const int SalesAccount = 6;//Income  isDebit=false
        public const int SalesReturnAccount = 7;//Income isDebit=true
        public const int CapitalAccount = 8;//Income isDebit=true
        public const int SalaryPayableAccount = 9;
        public const int TaxPayableAccount = 10;
        public const int TaxReceivableAccount = 11;

        public enum AccountTypes
        { 
            //Main Types
            FixedAssets = 0 ,OtherFixedAssets,CurrentAssets, Equity,
            
            FixedLiabilities =32 , CurrentLiabilities,
            Income,
            CostOfSale = 64, AdministrationExpenses, SellingAndDistributionExpenses, FinancialCost, OtherCost, SalaryExpenses,

            //Other Types
            CurrentAssets_Cash = 96 ,CurrentAssets_Customer,CurrentAssets_Bank,CurrentLiability_Supplier,            
            Depreciation,
            //Account creation for Depreciation and Expenses need to create AccumilatedSepreciation and ExpensePayable accounts

            NA
        }
        public static List<String> GetAccountTypes()
        {
           List<String> t= Enum.GetValues(typeof(AccountTypes))
            .Cast<AccountTypes>()
            .Select(v => v.ToString())
            .ToList();

           return t;
        }
        public Account(string name, AccountTypes type)
        {
            this.IsEnabled = true;
            this.Name = name;
            this.AccountType = type;
            this.Id = 0;
        }
 
        public Account(int Id,string name, AccountTypes type)
        {
            this.Id = Id;
            this.IsEnabled = true;
            this.Name = name;
            this.AccountType = type;
        }

        /// <summary>
        /// Parameterless constructor for EF
        /// </summary>
        public Account()
        {
            this.IsEnabled = true;
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        /// <summary>
        /// Code to to be shown and to maintain the order in reports 
        /// </summary>
        public string Code { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        [Display(Name="Account Type")]
        public AccountTypes AccountType { get; set; }
        public bool IsEnabled { get; set; }

        public int? ClientID { get; set; }
        public Client Client { get; set; }

        [NotMapped]
        public bool IsDebit
        {
            get
            {
                AccountTypes type = AccountType;

                bool isDebit = type == AccountTypes.FixedAssets || type == AccountTypes.OtherFixedAssets || type == AccountTypes.CostOfSale || type == AccountTypes.AdministrationExpenses 
                || type == AccountTypes.SellingAndDistributionExpenses || type == AccountTypes.FinancialCost || type == AccountTypes.OtherCost || type == AccountTypes.CurrentAssets
                 || type == AccountTypes.CurrentAssets_Cash || type == AccountTypes.CurrentAssets_Customer || type == AccountTypes.CurrentAssets_Bank || type == AccountTypes.Depreciation || type == AccountTypes.SalaryExpenses;

                return isDebit;
            }
        }

        [NotMapped]
        public bool IsExpenseAccount
        {
            get
            {
                return AccountType == AccountTypes.CostOfSale || AccountType == AccountTypes.AdministrationExpenses ||
                    AccountType == AccountTypes.SellingAndDistributionExpenses || AccountType == AccountTypes.FinancialCost ||
                    AccountType == AccountTypes.OtherCost ;
            }
        }

    }
}
