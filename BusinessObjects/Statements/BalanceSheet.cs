using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;

namespace BusinessObjects
{
    public class BalanceSheet
    {  
        public IEnumerable<AccountBalance> FixedAssets { get; set; }
        public IEnumerable<AccountBalance> OtherFixedAssets { get; set; }
        public IEnumerable<AccountBalance> CurrentAssets_Cash { get; set; }
        public IEnumerable<AccountBalance> CurrentLiability_Creditors { get; set; }
        public IEnumerable<AccountBalance> CurrentAssets_Debtors { get; set; }
        public IEnumerable<AccountBalance> CurrentAssets_Bank { get; set; }
        public IEnumerable<AccountBalance> Equity { get; set; }
        public IEnumerable<AccountBalance> FixedLiabilities { get; set; }
        public IEnumerable<AccountBalance> CurrentLiabilities { get; set; }
        public IEnumerable<AccountBalance> InventoryBalance { get; set; }
        
        public decimal FixedAssetsTotal { get; set; }
        public decimal OtherFixedAssetsTotal { get; set; }
        public decimal CurrentAssets_CashTotal { get; set; }
        public decimal CurrentLiability_CreditorsTotal { get; set; }
        public decimal CurrentAssets_DebtorsTotal { get; set; }
        public decimal CurrentAssets_BankTotal { get; set; }
        public decimal EquityTotal { get; set; }
        public decimal FixedLiabilitiesTotal { get; set; }
        public decimal CurrentLiabilitiesTotal { get; set; }
        public decimal InventoryBalanceTotal { get; set; }

        public decimal Profit { get; set; }

        public DateTime date{ get; set; }

        public BalanceSheet()
        {
            FixedAssets = new List<AccountBalance>();
            OtherFixedAssets = new List<AccountBalance>();
            CurrentAssets_Cash = new List<AccountBalance>();
            CurrentLiability_Creditors = new List<AccountBalance>();
            CurrentAssets_Debtors = new List<AccountBalance>();
            CurrentAssets_Bank = new List<AccountBalance>();
            Equity = new List<AccountBalance>();
            FixedLiabilities = new List<AccountBalance>();
            CurrentLiabilities = new List<AccountBalance>();
            InventoryBalance = new List<AccountBalance>(); 
        }
 

    }
}
