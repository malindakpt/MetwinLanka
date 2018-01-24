using BusinessObjects;
using BusinessObjects.Statistics;
using BusinessObjects.Statements;
using BusinessObjects.Reports;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using DataAccessLayer;

namespace BusinessLogic
{
    public class ReportManager : IDisposable
    {
        IInvoicePersister invPersister;

        AccountManagerHelper acc_mgr_helper = new AccountManagerHelper();
        AccountManager am = new AccountManager();

        public ReportManager(IInvoicePersister invDB = null)
        {
            invPersister = invDB ?? new InvoicePersister();
        }

        public BalanceSheet PrepareBalanceSheet()
        {
            //BalanceSheet balsht = new BalanceSheet();

            BalanceSheet balsht =  am.GenerateBalanceSheet();
            

            
            //List<AccountBalance> fixedAssets = new List<AccountBalance>();
            //fixedAssets.Add(new AccountBalance { Account = getTestAccount("a") , Balance = 30000 });
            //fixedAssets.Add(new AccountBalance { Account = getTestAccount("b") , Balance = 50000 });
            //balsht.FixedAssets = fixedAssets;

            //List<AccountBalance> otherFA = new List<AccountBalance>();
            //otherFA.Add(new AccountBalance { Account = getTestAccount("c"), Balance = 500000 });
            //otherFA.Add(new AccountBalance { Account = getTestAccount("d"), Balance = 50000 });
            //balsht.OtherFixedAssets = otherFA;

            //List<AccountBalance> CurrentAssCash = new List<AccountBalance>();
            //CurrentAssCash.Add(new AccountBalance { Account = getTestAccount("c"), Balance = 500000 });
            //CurrentAssCash.Add(new AccountBalance { Account = getTestAccount("d"), Balance = 76000 });
            //balsht.CurrentAssets_Cash = CurrentAssCash;

            //List<AccountBalance> CurrentAssDbt = new List<AccountBalance>();
            //CurrentAssDbt.Add(new AccountBalance { Account = getTestAccount("c"), Balance = 500000 });
            //CurrentAssDbt.Add(new AccountBalance { Account = getTestAccount("d"), Balance = 76000 });
            //balsht.CurrentAssets_Debtors = CurrentAssDbt;

            //balsht.CurrentAssets_Creditors = CurrentAssDbt;
            //balsht.CurrentAssets_Bank = CurrentAssDbt;
            //balsht.Equity = fixedAssets;

            //balsht.FixedLiabilities = fixedAssets;
            //balsht.CurrentLiabilities = fixedAssets;
            //balsht.FixedAssetsTotal = getRandomNum();
            //balsht.OtherFixedAssetsTotal = getRandomNum();
            //balsht.CurrentAssets_CashTotal = getRandomNum();
            //balsht.CurrentAssets_CreditorsTotal = getRandomNum();
            //balsht.CurrentAssets_DebtorsTotal = getRandomNum();
            //balsht.CurrentAssets_BankTotal = getRandomNum();
            //balsht.EquityTotal = getRandomNum();
            //balsht.FixedLiabilitiesTotal = getRandomNum();
            //balsht.CurrentLiabilitiesTotal = getRandomNum();
            //balsht.date = DateTime.Today;

            return balsht;
        }

        public IncomeStatement prepareInStmt(DateTime mydate)
        {
            //IncomeStatement instmt = new IncomeStatement();

            IncomeStatement instmt = am.GenerateIncomeStatement(mydate);

            //instmt.date = mydate;

            //List<AccountBalance> income = new List<AccountBalance>();
            //income.Add(new AccountBalance { Account = getTestAccount("a"), Balance = 30000 });
            //income.Add(new AccountBalance { Account = getTestAccount("b"), Balance = 50000 });
            
            //List<AccountBalance> costOfSale = new List<AccountBalance>();
            //costOfSale.Add(new AccountBalance { Account = getTestAccount("c"), Balance = 500000 });
            //costOfSale.Add(new AccountBalance { Account = getTestAccount("d"), Balance = 50000 });
            
            //List<AccountBalance> adminEx = new List<AccountBalance>();
            //adminEx.Add(new AccountBalance { Account = getTestAccount("c"), Balance = 500000 });
            //adminEx.Add(new AccountBalance { Account = getTestAccount("d"), Balance = 76000 });
            //adminEx.Add(new AccountBalance { Account = getTestAccount("r"), Balance = 7667000 });
            
            //List<AccountBalance> sadEx = new List<AccountBalance>();
            //sadEx.Add(new AccountBalance { Account = getTestAccount("c"), Balance = 500000 });
            //sadEx.Add(new AccountBalance { Account = getTestAccount("d"), Balance = 76000 });
            //sadEx.Add(new AccountBalance { Account = getTestAccount("g"), Balance = 56000 });

            //instmt.Income = income;
            //instmt.CostOfSale = costOfSale;
            //instmt.AdministrationExpenses = adminEx;
            //instmt.SellingAndDistributionExpenses = sadEx;
            //instmt.FinancialCost = costOfSale;
            //instmt.OtherCost = adminEx;
            //instmt.IncomeTotal = getRandomNum();
            //instmt.CostOfSaleTotal = getRandomNum();
            //instmt.AdministrationExpensesTotal = getRandomNum();
            //instmt.SellingAndDistributionExpensesTotal = getRandomNum();
            //instmt.FinancialCostTotal = getRandomNum();
            //instmt.OtherCostTotal = getRandomNum();
            //instmt.GrossProfit = getRandomNum();
            //instmt.NetProfit = getRandomNum();

            return instmt;
        }

        public TrialBalance prepareTrilaBalance(DateTime tbDate)
        {
            TrialBalance tb = new TrialBalance();

            tb.Date = tbDate;
            List<AccountBalance> accountList = am.GenerateTrialBalance(tbDate).BalanceList;

            tb.BalanceList = accountList.OrderBy(ac => ac != null ? ac.Account.Code : "").ToList();

            return tb;
        }

        public Account getTestAccount(String s)
        {
            Account acc = new Account("Test Account"+s, Account.AccountTypes.FixedAssets);
            return acc;
        }
        Random r = new Random();
        public decimal getRandomNum()
        {
            
            decimal d = (decimal)(r.Next(5000000, 10000000)) / 100;
            return d;
        }

        public List<MonthSummary> getSummary(DateTime fromdate, DateTime todate)
        {
            //Debug.Print(fromdate+":"+todate);
            int diffMonths = (todate.Month + todate.Year * 12) - (fromdate.Month + fromdate.Year * 12);
            
            //if (diffMonths > 12) diffMonths = 12;

            var months = new List<String> {"Jan","Feb","Mar","Apr","May","Jun","Jul","Aug","Sep","Oct","Nov","Dec" };

            var month = months.Concat(months).Concat(months).ToList();

            var monthSummary = new List<MonthSummary>();

            for (int i = 0; i < diffMonths; i++)
            {
                monthSummary.Add(new MonthSummary { month = month[fromdate.Month + i], income = getRandomNum(), expences = getRandomNum() });
            }

            return monthSummary;
        }

        public List<SalesAreaObj> getSaleSummary(DateTime fromdate, DateTime todate)
        {
            //TODO: get all sales for given time period and set into respectable arrays

            List<SalesAreaObj> dataList = new List<SalesAreaObj>(); //final data list

            //get all locations

            ClientManager cm = new ClientManager();

            List<String> month = new List<String> { };
            List<String> area = new List<String> { "Pannala","Kuliyapitiya","Giriulla","Kelaniya"};//example data
            area = cm.GetAllLocations().Select(loc => loc.Name).ToList();
            //are id
            List<int> areaID = cm.GetAllLocations().Select(loc => loc.Id).ToList();

            //if cx select two days from same month, system will take last month
            if (fromdate.Year == todate.Year)
            {
                if (fromdate.Month == todate.Month)
                {
                    fromdate = fromdate.AddMonths(-1);
                    todate = todate.AddMonths(1);
                }
            }

            //get months
            DateTime From = fromdate.AddDays(-fromdate.Day +1);
            DateTime To = todate.AddDays(-todate.Day +1);
            
            //creating month array
            for (int i = 0; i < 12; i++)
            {
                DateTime temp = From.AddMonths(i);
                month.Add(temp.ToString("MMM"));
                if (temp.Month.Equals(To.Month))
                {
                    break;
                }
            }

            //load invoices for given duration
            InvoiceManager im = new InvoiceManager();
            IEnumerable<Invoice> invoiceList = im.LoadInvoices(Invoice.TansactionTypes.Sales,new DateRange(From,To));
            
            for (int j = 0; j < area.Count;j++ )
            {
                SalesAreaObj sao = new SalesAreaObj();
                sao.area = area[j];
                int areaid = areaID[j];
                List<SalesAreaSub> sasList = new List<SalesAreaSub>(); //list of month and respective sales values

                for (int i = 0; i < month.Count; i++)
                {
                    SalesAreaSub sas = new SalesAreaSub();
                    sas.month = month[i];
                    var monthStart = From.AddMonths(i);
                    var monthEnd = From.AddMonths(i + 1);
                    var total = invoiceList.Where(inv => inv.Client.LocationId == areaid)
                                        .Where(inv => inv.Time <= monthEnd && inv.Time >= monthStart)
                                        .Sum(inv => inv.GetTotal());
                    sas.value = total;//get total sales for respective month
                    sasList.Add(sas);
                }
                sao.monthSummary = sasList;

                dataList.Add(sao);

            }
            return dataList;
        }

        public List<Sales_Period> getSalesperiod(DateTime? fromdate, DateTime? todate)
        {

            DateRange range = new DateRange { Start = fromdate, End = todate };
            var invoices = invPersister.LoadInvoices(Invoice.TansactionTypes.Sales, range).AsEnumerable();

            var infos = invoices.Select(i => new Sales_Period
            {
                amount = i.GetTotal(),
                customerName = i.Client.Name,
                date = i.Time,
                invoiceId = i.Id,
                saleRepName = i.SalesRepId.HasValue ? i.SalesRep.FullName : null,
                DisplayInvoiceId = i.FormattedInvoiceNo
            });

            return infos.ToList();
        }

        public List<Purchase_Period> getPurchasePeriod(DateTime? fromdate, DateTime? todate)
        {
            DateRange range = new DateRange { Start = fromdate, End = todate };
            var invoices = invPersister.LoadInvoices(Invoice.TansactionTypes.Purchases, range).AsEnumerable();

            var infos = invoices.Select(i => new Purchase_Period
            {
                amount = i.GetTotal(),
                supplier = i.Client.Name,
                date = i.Time,
                 purchadeId = i.Id,
                  DisplayInvoiceId = i.FormattedInvoiceNo
            });

            return infos.ToList();
        }





        public void Dispose()
        {
            invPersister.Dispose();
        }
    }
}
