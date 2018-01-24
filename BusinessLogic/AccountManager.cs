using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects;
using DataAccessLayer;
using System.Collections; 
using BusinessObjects.Statements;

namespace BusinessLogic
{
    public class AccountManager
    {

        InvoiceManager invoiceManager;
        InvoicePersister invoicePersister;
        AccountManagerHelper accountHelper;
        AccountPersister accountPersister;

        public AccountManager()
        {
            invoiceManager = new InvoiceManager();
            accountHelper = new AccountManagerHelper();
            accountPersister = new AccountPersister();
            invoicePersister = new InvoicePersister();

        }

        public Account CreateAccount(string name, Account.AccountTypes accType)
        {
            Account account = new Account(name, accType);
            accountPersister.CreateAccount(account);
            return account;
        }


        public Account CreateAccount(string name, Account.AccountTypes accType,string clientName,Client.Types clientType,string phone,string address, int? locationId)
        {
            Account account = new Account(name, accType);
            Client client = new Client(clientName, clientType, phone, address, locationId);
            account.Client = client;
            accountPersister.CreateAccount(account);
            return account;
        }


        public Account GetAccountByMainTansactionType(Invoice.TansactionTypes trType)
        {
            throw new NotImplementedException();
        }

        public Account GetAccountById(int? accountId)
        {
            return accountPersister.LoadAccount(accountId);
        }

        public TrialBalance GenerateTrialBalance(DateTime date)
        {
            TrialBalance trialBalance = new TrialBalance();
            trialBalance.BalanceList = accountHelper.GetAccountBalances().ToList();
            trialBalance.Date = date;
            
            return ModifyTrialBalance(trialBalance);
        }
        public TrialBalance ModifyTrialBalance(TrialBalance trialBalance)
        {
            AccountBalance Debtors = new AccountBalance();
            Debtors.Account = new Account("Debtors", Account.AccountTypes.CurrentAssets_Customer);
            Debtors.Balance = 0;
            for(int i = 0; i < trialBalance.BalanceList.Count(); i++)
            {
                if (trialBalance.BalanceList.ElementAt(i).Account.AccountType == Account.AccountTypes.CurrentAssets_Customer)
                {
                    Debtors.Balance = Debtors.Balance + trialBalance.BalanceList.ElementAt(i).Balance;
                    trialBalance.BalanceList[i] = null; 
                }

            }
            trialBalance.BalanceList.Add(Debtors);
            return trialBalance;

        }
        public IncomeStatement GenerateIncomeStatement(DateTime from)
        {
            IncomeStatement incomeStatement = new IncomeStatement();

            incomeStatement.date = from;
           
            //Creating income accounts 
            IEnumerable<AccountBalance> incomeList = accountHelper.GetAccountListBalancesByCategory(Account.AccountTypes.Income, from); 
            incomeStatement.Income = incomeList;
            incomeStatement.IncomeTotal = GetTotalOfAccounts(incomeList);
           
            //Creating Cost of Sale
            DateTime firstDay = new DateTime(2000, 1,1, 16, 0, 0);
            DateTime today = DateTime.Now;
            List<AccountBalance> costOfSaleList = new List<AccountBalance>();
            AccountBalance acOpenInventory = new AccountBalance(new Account("Opening Inventory", Account.AccountTypes.NA), 0);
            AccountBalance acPurchase = new AccountBalance(GetAccountById(Account.PurchasesAccount), accountPersister.LoadAccoutBalance(Account.PurchasesAccount, new DateRange(firstDay, today)).Balance);
            AccountBalance acPurchaseReturn = new AccountBalance(GetAccountById(Account.PurchasesReturnAccount), accountPersister.LoadAccoutBalance(Account.PurchasesReturnAccount, new DateRange(firstDay, today)).Balance);
            AccountBalance acCloseInventory = new AccountBalance(new Account("Closing Inventory", 0), invoicePersister.GetRawMaterialBalancesValue());

            costOfSaleList.Add(acOpenInventory);
            costOfSaleList.Add(acPurchase);
            costOfSaleList.Add(acPurchaseReturn);
            costOfSaleList.Add(acCloseInventory);

            incomeStatement.CostOfSale = costOfSaleList;
            incomeStatement.CostOfSaleTotal = acOpenInventory.Balance + acPurchase.Balance - acPurchaseReturn.Balance - acCloseInventory.Balance;
                
            //Creating AdminExpenses
            IEnumerable<AccountBalance> adminExpList = accountHelper.GetAccountListBalancesByCategory(Account.AccountTypes.AdministrationExpenses, from); 
            incomeStatement.AdministrationExpenses = adminExpList;
            incomeStatement.AdministrationExpensesTotal = GetTotalOfAccounts(adminExpList);

            //Creating Selling and Distribution
            IEnumerable<AccountBalance> sellingAndDistributionCostList = accountHelper.GetAccountListBalancesByCategory(Account.AccountTypes.SellingAndDistributionExpenses, from); 
            incomeStatement.SellingAndDistributionExpenses = sellingAndDistributionCostList;
            incomeStatement.SellingAndDistributionExpensesTotal = GetTotalOfAccounts(sellingAndDistributionCostList);

            //Creating Salary Expenses
            IEnumerable<AccountBalance> SalaryExpCostList = accountHelper.GetAccountListBalancesByCategory(Account.AccountTypes.SalaryExpenses, from);
            incomeStatement.SalaryExpenses = SalaryExpCostList;
            incomeStatement.SalaryExpensesTotal = GetTotalOfAccounts(SalaryExpCostList);

            //Creating FinacialCost
            IEnumerable<AccountBalance> financialCostList = accountHelper.GetAccountListBalancesByCategory(Account.AccountTypes.FinancialCost, from); 
            incomeStatement.FinancialCost = financialCostList;
            incomeStatement.FinancialCostTotal=GetTotalOfAccounts(financialCostList);

            //Creating OtherCost
            IEnumerable<AccountBalance> otherCostList = accountHelper.GetAccountListBalancesByCategory(Account.AccountTypes.OtherCost, from); 
            incomeStatement.OtherCost = otherCostList;
            incomeStatement.OtherCostTotal=GetTotalOfAccounts(otherCostList);

            //Calcualting the profit
            incomeStatement.GrossProfit = 1*(incomeStatement.IncomeTotal - incomeStatement.CostOfSaleTotal);
            incomeStatement.NetProfit = 1 * (incomeStatement.IncomeTotal - incomeStatement.CostOfSaleTotal - incomeStatement.AdministrationExpensesTotal - incomeStatement.SellingAndDistributionExpensesTotal - incomeStatement.SalaryExpensesTotal - incomeStatement.FinancialCostTotal - incomeStatement.OtherCostTotal);

            return incomeStatement;

        }

        public BalanceSheet GenerateBalanceSheet()
        {
            BalanceSheet balanceSheet = new BalanceSheet();
            DateTime firstDay = new DateTime(2000, 1, 1, 16, 0, 0);
            IncomeStatement incomeStatement = GenerateIncomeStatement(firstDay);
            balanceSheet.Profit = incomeStatement.NetProfit;
            

            //Creating FixedAssets accounts 1
            IEnumerable<AccountBalance> fixedAssetsList = accountHelper.GetAccountListByCategory(Account.AccountTypes.FixedAssets); 
            balanceSheet.FixedAssets = fixedAssetsList;
            balanceSheet.FixedAssetsTotal = GetTotalOfAccounts(fixedAssetsList);

            //Creating OtherFixedAssets accounts 2
            IEnumerable<AccountBalance> otherFixedAssetsList = accountHelper.GetAccountListByCategory(Account.AccountTypes.OtherFixedAssets); 
            balanceSheet.OtherFixedAssets = otherFixedAssetsList;
            balanceSheet.OtherFixedAssetsTotal = GetTotalOfAccounts(otherFixedAssetsList);

            //Creating CurrentAssets accounts 3
            IEnumerable<AccountBalance> currentAssetsCashList = accountHelper.GetAccountListByCategory(Account.AccountTypes.CurrentAssets_Cash);
            balanceSheet.CurrentAssets_Cash = currentAssetsCashList;
            balanceSheet.CurrentAssets_CashTotal = GetTotalOfAccounts(currentAssetsCashList);

            // 4
            IEnumerable<AccountBalance> currentAssetsDebtorsList = accountHelper.GetAccountListByCategory(Account.AccountTypes.CurrentAssets_Customer);
            balanceSheet.CurrentAssets_Debtors = currentAssetsDebtorsList;
            balanceSheet.CurrentAssets_DebtorsTotal = GetTotalOfAccounts(currentAssetsDebtorsList);

            //5
            IEnumerable<AccountBalance> currentAssetsBankList = accountHelper.GetAccountListByCategory(Account.AccountTypes.CurrentAssets_Bank);
            balanceSheet.CurrentAssets_Bank = currentAssetsBankList;
            balanceSheet.CurrentAssets_BankTotal = GetTotalOfAccounts(currentAssetsBankList);

            //Creating Equity accounts 6
            IEnumerable<AccountBalance> equityList = accountHelper.GetAccountListByCategory(Account.AccountTypes.Equity); 
            balanceSheet.Equity = equityList;
            balanceSheet.EquityTotal = GetTotalOfAccounts(equityList);

            //Creating FixedLiabilities accounts 7
            IEnumerable<AccountBalance> fixedLiabilityList = accountHelper.GetAccountListByCategory(Account.AccountTypes.FixedLiabilities); 
            balanceSheet.FixedLiabilities = fixedLiabilityList;
            balanceSheet.FixedLiabilitiesTotal = GetTotalOfAccounts(fixedLiabilityList);

            //Creating FixedLiabilities accounts suppli 8
            IEnumerable<AccountBalance> currentLiabilityCreditorList = accountHelper.GetAccountListByCategory(Account.AccountTypes.CurrentLiability_Supplier);
            balanceSheet.CurrentLiability_Creditors = currentLiabilityCreditorList;
            balanceSheet.CurrentLiability_CreditorsTotal = GetTotalOfAccounts(currentLiabilityCreditorList);

            //Creating CurrentLiabilities accounts 9
            IEnumerable<AccountBalance> currentLiabilityList = accountHelper.GetAccountListByCategory(Account.AccountTypes.CurrentLiabilities); 
            balanceSheet.CurrentLiabilities = currentLiabilityList;
            balanceSheet.CurrentLiabilitiesTotal = GetTotalOfAccounts(currentLiabilityList);

            //Creating InventoryBalance 10
            IEnumerable<AccountBalance> inventoryList = accountHelper.GetInventoryList();
            balanceSheet.InventoryBalance = inventoryList;
            balanceSheet.InventoryBalanceTotal = GetTotalOfAccounts(inventoryList);

            return balanceSheet;
        }

        private decimal GetTotalOfAccounts(IEnumerable<AccountBalance> list)
        {
            decimal total = 0;
            foreach(AccountBalance acc in list){  
                total += acc.Balance; 
            }
            return total;
        }

        public IEnumerable<Account> LoadAccounts(Account.AccountTypes? type, bool includeDisabledAccs = false)
        {
            return accountPersister.LoadAccounts(type, includeDisabledAccs);
        }

        public void ChangeAccountCode(int accountId,string newCode)
        {
            accountPersister.ChangeAccountCode(accountId, newCode);
        }
    }
}
