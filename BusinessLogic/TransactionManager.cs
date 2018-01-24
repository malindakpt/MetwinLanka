using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessObjects; 
using DataAccessLayer;
using NLog;
using BusinessObjects.Categories;

namespace BusinessLogic
{
    class TransactionManager
    {
        TransactionPersister trasactionPersister;
        AccountManager accountManager;
        AccountLink accountLink;
        AccountPersister accountPersister;

        Logger logger = LogManager.GetCurrentClassLogger();

        public TransactionManager()
        {
            trasactionPersister = new TransactionPersister();
            accountManager = new AccountManager();
            accountLink = new AccountLink();
            accountPersister = new AccountPersister();
        }

        public void AddTransactions(Invoice invoice)
        {
            logger.Info("Adding a new trransaction");

            if (invoice.PaymentReceipts.Count == 0)
            {
                throw new ArgumentException("Invoice must contain minimum  1 payment receipt");
            } 
            else
            {
                ICollection<Transaction> transactionList = new List<Transaction>();
                foreach (PaymentReceipt temp in invoice.PaymentReceipts)
                {
                    if (temp.Transaction == null)
                    {
                        AddTransaction(invoice, temp);
                    }
                }
            
            }
        }

        private void AddTransaction(Invoice invoice, PaymentReceipt payment)
        {

            //Purchases = 0,
            //PurchasesReturn = 1,
            //Sales = 2,
            //SalesReturn = 3,
            //Wastage = 4,   n

            //Expenses = 10,
            //Income = 11,

            //Depreciation = 20,
            //PayableTransfer = 21,
            //BankTransfer = 22,D:\Malinda Private\Metwin\BusinessLogic\TransactionManager.cs

            //Deposit = 30  n
            Transaction transaction = new Transaction();
            transaction.Time = payment.Time;
            transaction.InvoiceRef = invoice;
            transaction.Amount = payment.Amount;
            transaction.Description = payment.Description;

            if (payment.PaymentMethod != PaymentReceipt.PaymentType.Credit) //Later Payments going here : Cash or Cheque Payment
            {

                int? otherAccID = 0;
                switch (invoice.TransType)
                {
                    case Invoice.TansactionTypes.Purchases: //Purchases
                        otherAccID = invoice.ClientId.Value;
                        addSendTrRecordsClient(payment, transaction, otherAccID);

                        break;
                    case Invoice.TansactionTypes.PurchasesReturn:   //Purchase Return
                        otherAccID = invoice.ClientId.Value;
                        throw new NotImplementedException("Purchase returns are not supported");

                    case Invoice.TansactionTypes.Sales: //Sales      
                        otherAccID = invoice.ClientId.Value;
                        addRecvTrRecordsClient(payment, transaction, otherAccID);

                        break;
                    case Invoice.TansactionTypes.SalesReturn:   //Sales Return                                             
                        otherAccID = invoice.ClientId.Value;
                        addSendTrRecordsClient(payment, transaction, otherAccID);

                        break;

                    case Invoice.TansactionTypes.Wastage:
                        throw new NotImplementedException();
                    case Invoice.TansactionTypes.Expenses:  //Expenses
                        otherAccID = invoice.RelatedAccountId;
                        if (payment.Side == PaymentReceipt.Sides.Sent)
                        {
                            addExpenseTrRecords(payment, transaction, otherAccID);
                        }
                        else
                        {
                            throw new ArgumentException(" Invalid Transaction type for expenses");
                        }
                        break;
                    case Invoice.TansactionTypes.Income:
                        break;
                    case Invoice.TansactionTypes.Depreciation:
                        otherAccID = invoice.RelatedAccountId;
                        throw new NotImplementedException();
                    case Invoice.TansactionTypes.PayableTransfer:
                        otherAccID = invoice.RelatedAccountId;
                        throw new NotImplementedException();
                    case Invoice.TansactionTypes.BankTransfer:
                        otherAccID = invoice.RelatedAccountId;
                        throw new NotImplementedException();
                    case Invoice.TansactionTypes.Deposit:
                        //Deposite payment type cannot be Credit"
                        transaction.DeAccId = getAccountForType(invoice, payment).Id;
                        transaction.CrAccId = accountManager.GetAccountById(invoice.ClientId.Value).Id;
                        break;
                    case Invoice.TansactionTypes.CapitalDeposite:   //Capital Deposite
                        CapitalPUSHTrRecords(payment, transaction);
                        break;
                    case Invoice.TansactionTypes.CapitalWithdraw:   //Capital Withdraw
                        CapitalPOPTrRecords(payment, transaction);
                        break;
                    default:
                        throw new ArgumentException(" Error Transaction");


                }
            }
            else// Initial Credit Patment
            {
                switch (invoice.TransType)
                {
                    //Here all invoices payment.PaymentMethod equals to PaymentReceipt.PaymentType.Credit
                    case Invoice.TansactionTypes.Purchases: //0
                        if (payment.IsTaxReceipt)
                        {
                            transaction.DeAccId = accountManager.GetAccountById(Account.TaxReceivableAccount).Id;
                            transaction.CrAccId = getAccountForType(invoice, payment).Id;
                            transaction.Description = transaction.Description + " | VAT";
                        }
                        else
                        {
                            transaction.DeAccId = accountManager.GetAccountById(Account.PurchasesAccount).Id;
                            transaction.CrAccId = getAccountForType(invoice, payment).Id;
                        }
                        break;

                    case Invoice.TansactionTypes.PurchasesReturn: //1
                        if (payment.IsTaxReceipt)
                        {
                            transaction.DeAccId = getAccountForType(invoice, payment).Id;
                            transaction.CrAccId = accountManager.GetAccountById(Account.TaxReceivableAccount).Id;
                            transaction.Description = transaction.Description + " | VAT";
                        }
                        else
                        {
                            transaction.DeAccId = getAccountForType(invoice, payment).Id;
                            transaction.CrAccId = accountManager.GetAccountById(Account.PurchasesReturnAccount).Id;
                        }
                        break;

                    case Invoice.TansactionTypes.Sales: //2
                        if (payment.IsTaxReceipt)
                        {
                            transaction.DeAccId = getAccountForType(invoice, payment).Id;
                            transaction.CrAccId = accountManager.GetAccountById(Account.TaxPayableAccount).Id;
                            transaction.Description = transaction.Description + " | VAT";
                        }
                        else {
                            transaction.DeAccId = getAccountForType(invoice, payment).Id;
                            transaction.CrAccId = accountManager.GetAccountById(Account.SalesAccount).Id;
                        }
                        break;

                    case Invoice.TansactionTypes.SalesReturn: //3
                        if (payment.IsTaxReceipt)
                        {
                            transaction.DeAccId = accountManager.GetAccountById(Account.TaxPayableAccount).Id;
                            transaction.CrAccId = getAccountForType(invoice, payment).Id;
                            transaction.Description = transaction.Description + " | VAT";
                        }
                        else {
                            transaction.DeAccId = accountManager.GetAccountById(Account.SalesReturnAccount).Id;
                            transaction.CrAccId = getAccountForType(invoice, payment).Id;
                        }
                        break;

                    case Invoice.TansactionTypes.Expenses: //10
                        if (invoice.Items.Count != 1)
                        {
                            throw new ArgumentException(" Invoice.TansactionTypes.Expenses shoud not contain more than 1 item");
                        }
                        transaction.DeAccId = invoice.RelatedAccountId.Value;
                        transaction.CrAccId = getAccountForType(invoice, payment).Id;
                        break;

                    case Invoice.TansactionTypes.Income: //11
                        if (invoice.Items.Count != 1)
                        {
                            throw new ArgumentException(" Invoice.TansactionTypes.Income shoud not contain more than 1 item");
                        }
                        transaction.DeAccId = getAccountForType(invoice, payment).Id;
                        transaction.CrAccId = invoice.RelatedAccountId.Value;
                        break;

                    case Invoice.TansactionTypes.Depreciation: //20
                        if (invoice.Items.Count != 1)
                        {
                            throw new ArgumentException(" Invoice.TansactionTypes.Depreciation shoud not contain more than 1 item");
                        }
                        Account account = ((AccountProfile)invoice.Items.First().Profile).Account;
                        transaction.DeAccId = invoice.RelatedAccountId.Value;
                        transaction.CrAccId = accountPersister.GetLinkedAccount(account.Id, AccountLink.LinkType.Depreciation).Id;
                        break;

                    case Invoice.TansactionTypes.PayableTransfer: //21
                        if (invoice.Items.Count != 1)
                        {
                            throw new ArgumentException(" Invoice.TansactionTypes.PayableTransfer shoud not contain more than 1 item");
                        }
                        account = ((AccountProfile)invoice.Items.First().Profile).Account;
                        transaction.DeAccId = account.Id;
                        transaction.CrAccId = accountPersister.GetLinkedAccount(account.Id, AccountLink.LinkType.Payable).Id;
                        break;

                    case Invoice.TansactionTypes.BankTransfer: //22
                        if (invoice.Items.Count != 1)
                        {
                            throw new ArgumentException(" Invoice.TansactionTypes.BankTransfer should not contain more than 1 item");
                        }
                        account = ((AccountProfile)invoice.Items.First().Profile).Account;
                        transaction.DeAccId = invoice.RelatedAccountId.Value;
                        transaction.CrAccId = Account.BankAccount;
                        break;
                }
            }

            payment.Transaction = transaction;

            if (payment.Transaction.CrAccId == 0 || payment.Transaction.DeAccId == 0)
            {
                throw new NullReferenceException("Transaction.CrAccId or Transaction.DeAccId is not set");
            }
        }

        public void AddSalaryTransactions(Invoice invoice)
        {
            logger.Info("Adding a new Salary transaction");

            if (invoice.PaymentReceipts.Count == 0 && invoice.TransType != Invoice.TansactionTypes.SalaryTransfer)
            {
                throw new ArgumentException("Invalid salary transfer invoice");
            }
            else
            {
                ICollection<Transaction> transactionList = new List<Transaction>();
                foreach (PaymentReceipt recp in invoice.PaymentReceipts)
                {
                    if (recp.Transaction == null)
                    { 
                        Transaction transaction = new Transaction();
                        transaction.Time = invoice.Time;
                        transaction.InvoiceRef = invoice;
                        transaction.Amount = recp.Amount;
                         
                        transaction.DeAccId = recp.TransferToId.Value;
                        transaction.CrAccId = accountManager.GetAccountById(Account.SalaryPayableAccount).Id;

                        recp.Transaction = transaction;
                    }
                }

            }
        }
        public void AddLoanCreateTransactions(Invoice invoice)
        {
            logger.Info("Adding a new Loan Creation transaction");

            if (invoice.PaymentReceipts.Count == 0 && invoice.TransType != Invoice.TansactionTypes.LoanCreation)
            {
                throw new ArgumentException("Invalid Loan Creation invoice");
            }
            else
            {
                ICollection<Transaction> transactionList = new List<Transaction>();
                foreach (PaymentReceipt recp in invoice.PaymentReceipts)
                {
                    if (recp.Transaction == null)
                    {
                        Transaction transaction = new Transaction();
                        transaction.Time = invoice.Time;
                        transaction.InvoiceRef = invoice;
                        transaction.Amount = recp.Amount;

                        transaction.DeAccId = recp.TransferToId.Value;
                        transaction.CrAccId = recp.TransferFromId.Value;

                        recp.Transaction = transaction;
                    }
                }

            }
        }
        public void AddTransferTransactions(Invoice invoice)
        {
            logger.Info("Adding a new trransaction");

            if (invoice.PaymentReceipts.Count == 0 && invoice.TransType!= Invoice.TansactionTypes.GeneralTransfer)
            {
                throw new ArgumentException("Invoice invalid");
            }
            else
            {
                ICollection<Transaction> transactionList = new List<Transaction>();
                foreach (PaymentReceipt recp in invoice.PaymentReceipts)
                {
                    if (recp.Transaction == null)
                    { 
                        Transaction transaction = new Transaction();
                        transaction.Time = invoice.Time;
                        transaction.InvoiceRef = invoice;

                        transaction.Amount = recp.Amount;

                        transaction.DeAccId = recp.TransferToId.Value;
                        transaction.CrAccId = recp.TransferFromId.Value;

                        recp.Transaction = transaction;
                    }
                }

            }
        }

        private void addRecvTrRecordsClient(PaymentReceipt payment, Transaction transaction, int? clientAccID)
        {
            if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cash)
            {
                transaction.DeAccId = accountManager.GetAccountById(Account.CashAccount).Id;
                transaction.CrAccId = accountManager.GetAccountById(clientAccID).Id;
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cheque)//When receive check we cant decide where we deposite this cheque
            { 
                transaction.DeAccId = accountManager.GetAccountById(Account.BankAccount).Id;
                transaction.CrAccId = accountManager.GetAccountById(clientAccID).Id;
                
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.FromDeposit)
            {
                transaction.DeAccId = accountManager.GetAccountById(clientAccID).Id;
                transaction.CrAccId = accountManager.GetAccountById(clientAccID).Id;
            }
            else
            {
                throw new ArgumentException("Invalid Payment Method for Payment Reciept");
            }
        }


        private void addSendTrRecordsClient(PaymentReceipt payment, Transaction transaction, int? clientAccID)
        {
            if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cash)
            {
                transaction.CrAccId = accountManager.GetAccountById(Account.CashAccount).Id;
                transaction.DeAccId = accountManager.GetAccountById(clientAccID).Id;
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cheque)//When we pay money as a cheque, we know from which bank we deduct thar money
            {
                transaction.CrAccId = accountManager.GetAccountById(payment.BankAccountId).Id;
                transaction.DeAccId = accountManager.GetAccountById(clientAccID).Id;
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.FromDeposit)
            {
                transaction.CrAccId = accountManager.GetAccountById(clientAccID).Id;
                transaction.DeAccId = accountManager.GetAccountById(clientAccID).Id;
            }
            else
            {
                throw new ArgumentException("Invalid Payment Method for Payment Reciept");
            }
        }

        private void addExpenseTrRecords(PaymentReceipt payment, Transaction transaction, int? otherAccId)
        {
            if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cash)
            {
                transaction.CrAccId = accountManager.GetAccountById(Account.CashAccount).Id;
                transaction.DeAccId = accountManager.GetAccountById(otherAccId).Id;
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cheque)
            {
                transaction.CrAccId = accountManager.GetAccountById(payment.BankAccountId).Id;
                transaction.DeAccId = accountManager.GetAccountById(otherAccId).Id;
            }
            else 
            { 
                throw new ArgumentException("Invalid Payment Method for Expense Payment Reciept");
            }
        }

        private void  CapitalPUSHTrRecords(PaymentReceipt payment, Transaction transaction)
        {
            if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cash)
            {
                transaction.DeAccId = accountManager.GetAccountById(Account.CashAccount).Id;
                transaction.CrAccId = accountManager.GetAccountById(Account.CapitalAccount).Id;
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cheque)
            {
                transaction.DeAccId = accountManager.GetAccountById(Account.BankAccount).Id;
                transaction.CrAccId = accountManager.GetAccountById(Account.CapitalAccount).Id;
            }
            else
            {
                throw new ArgumentException("Invalid Payment Method for Expense Payment Reciept");
            }
        }
        private void CapitalPOPTrRecords(PaymentReceipt payment, Transaction transaction)
        {
            if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cash)
            {
                transaction.CrAccId = accountManager.GetAccountById(Account.CashAccount).Id;
                transaction.DeAccId = accountManager.GetAccountById(Account.CapitalAccount).Id;
            }
            else if (payment.PaymentMethod == PaymentReceipt.PaymentType.Cheque)
            {
                transaction.CrAccId = accountManager.GetAccountById(payment.BankAccountId).Id;
                transaction.DeAccId = accountManager.GetAccountById(Account.CapitalAccount).Id;
            }
            else
            {
                throw new ArgumentException("Invalid Payment Method for Expense Payment Reciept");
            }
        }
 
        private Account getAccountForType(Invoice invoice,PaymentReceipt payment)
        {
            switch (payment.PaymentMethod)
            { 
                case PaymentReceipt.PaymentType.Cash:
                    return accountManager.GetAccountById(Account.CashAccount);
                case PaymentReceipt.PaymentType.Cheque:
                    return accountManager.GetAccountById(Account.BankAccount);
                case PaymentReceipt.PaymentType.Credit:
                    return accountManager.GetAccountById(invoice.ClientId.Value);
                default:
                    throw new ArgumentException("Invalied PaymentType");
            }
        }

         
         
    }
}
