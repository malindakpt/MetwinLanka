using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class IncomeStatement
    {
        
        public IEnumerable<AccountBalance> Income { get; set; }
        public IEnumerable<AccountBalance> CostOfSale { get; set; }
        public IEnumerable<AccountBalance> AdministrationExpenses { get; set; }
        public IEnumerable<AccountBalance> SellingAndDistributionExpenses { get; set; }
        public IEnumerable<AccountBalance> SalaryExpenses { get; set; }
        public IEnumerable<AccountBalance> FinancialCost { get; set; }
        public IEnumerable<AccountBalance> OtherCost { get; set; }

      
        public decimal OpeningInventory { get; set; }
        public decimal Purchases { get; set; }
        public decimal PurchaseReturn { get; set; }
        public decimal ClosingInventory { get; set; }


        public decimal IncomeTotal { get; set; }
        public decimal CostOfSaleTotal { get; set; }
        public decimal AdministrationExpensesTotal { get; set; }
        public decimal SellingAndDistributionExpensesTotal { get; set; }
        public decimal SalaryExpensesTotal { get; set; }
        public decimal FinancialCostTotal { get; set; }
        public decimal OtherCostTotal { get; set; }

        public decimal GrossProfit { get; set; }
        public decimal NetProfit { get; set; }

        public DateTime date { get; set; }

        public IncomeStatement()
        {
            Income = new List<AccountBalance>();
            CostOfSale = new List<AccountBalance>();
            AdministrationExpenses = new List<AccountBalance>();
            SellingAndDistributionExpenses = new List<AccountBalance>();
            FinancialCost = new List<AccountBalance>();
            OtherCost = new List<AccountBalance>();
            SalaryExpenses = new List<AccountBalance>();
        }
    }
}
