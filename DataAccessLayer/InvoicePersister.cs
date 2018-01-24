using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataAccessLayer.PersisterInterface;
using BusinessObjects;
using System.Data.Entity;
using BusinessObjects.Invoicing;
using BusinessObjects.Categories;
using Microsoft.AspNet.Identity;


namespace DataAccessLayer
{
    public class InvoicePersister : IInvoicePersister
    {
        AppDBContext db = new AppDBContext();

        public void AddInvoice(Invoice invoice)
        {
            //invoice.Client = db.Clients.First(a => a.Id == invoice.ClientId);
            foreach (var item in invoice.Items)
            {
                item.Profile = db.ItemProfiles.First(prof => prof.Id == item.ProfileId);
            }

            invoice.ClientId = invoice.ClientId;
            invoice.Client = null;
            invoice.CreatedBy = db.Users.First(u => u.Id == invoice.CreatedBy.Id);

            ValidateInvoice(invoice);
            db.Invoices.Add(invoice);
            int? prevMax = 0;
            //make 'orders' have a different invoice Number
            if(invoice.Status == Invoice.InvoiceStatus.Order)
            {
                prevMax = db.Invoices.Where(inv => inv.TransType == invoice.TransType && inv.Status == Invoice.InvoiceStatus.Order)
                                .Max(inv => (int?)inv.DisplayInvoiceNo);
            }
            else
            {
                prevMax = db.Invoices.Where(inv => inv.TransType == invoice.TransType && inv.Status != Invoice.InvoiceStatus.Order)
                                .Max(inv => (int?)inv.DisplayInvoiceNo);
            }
            
            int newDisplayId = prevMax.HasValue ? prevMax.Value + 1 : 1;
            invoice.DisplayInvoiceNo = newDisplayId;
            db.SaveChanges();
        }

        public void EditInvoice(Invoice invoice)
        {
            ValidateInvoice(invoice);
            db.Entry(invoice).State = EntityState.Modified;
            db.SaveChanges();
        }

        public Invoice LoadInvoice(int Id)
        {
            return db.Invoices.Where(inv => inv.Id == Id)
                .Include(i=> i.Client).Include(i=> i.PaymentReceipts).Include(i=> i.Items.Select(it=> it.Profile))
                .Include( i=> i.Items.Select(it=> it.SourceRef))
                .Include(i => i.Items.Select(it => it.SourceRef.Profile))
                .Include(i => i.Items.Select(it => it.SourceRef.Invoice))
                .Include(i => i.PaymentReceipts.Select(p => p.TransferTo))
                .Include(i => i.PaymentReceipts.Select(p => p.TransferFrom))
                .FirstOrDefault();
        }

        public IEnumerable<Invoice> LoadInvoices(Invoice.TansactionTypes? type, DateRange range = null, Invoice.InvoiceStatus? status = Invoice.InvoiceStatus.Approved
                                                        , int resultCount = 0)
        {
            IQueryable<Invoice> invoices = db.Invoices.AsNoTracking().OrderByDescending(inv => inv.Time);
            {
                invoices = invoices.Where(i => i.TransType == type.Value).Include(i=> i.Client).OrderBy(i=> i.Time).Include(i => i.Items);
            }
            if(range != null)
            {
                if(range.Start.HasValue )
                {
                    invoices = invoices.Where(i => i.Time >= range.Start.Value);
                }
                if(range.End.HasValue)
                {
                    invoices = invoices.Where(i => i.Time <= range.End.Value);
                }
            }
            if(status.HasValue)
            {
                invoices = invoices.Where(i => i.Status == status.Value);
            }
            if(resultCount > 0)
            {
                invoices = invoices.Take(resultCount);
            }

            return invoices.ToList();
        }


        public IEnumerable<Cheque> GetChequePayments(Cheque.Side side, Cheque.ChequeStatus? status = null, DateRange range = null)
        {
            IQueryable<Cheque> cheques = db.Cheques;
            cheques = cheques.Where(c => c.ChequeSide == side);

            if (range != null)
            {
                if (range.Start.HasValue)
                {
                    cheques = cheques.Where(c=> c.HandedOverDate >= range.Start.Value);
                }
                if (range.End.HasValue)
                {
                    cheques = cheques.Where(c => c.HandedOverDate <= range.End.Value);
                }
            }
            if (status.HasValue)
            {
                cheques = cheques.Where(i => i.Status == status.Value);
            }

            return cheques.ToList();
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IEnumerable<DueInvoiceModel> GetDueInvoices(Invoice.TansactionTypes type)
        {
            if(type == Invoice.TansactionTypes.Purchases || type == Invoice.TansactionTypes.Sales || type == Invoice.TansactionTypes.SalesReturn)
            {
                var invoices = db.Invoices.Where(i => i.TransType == type && i.Status == Invoice.InvoiceStatus.Approved);

                var dues = invoices.Where(i => (i.Items.Sum(it => (decimal?)it.Price)) >
                    ( i.PaymentReceipts.Where(pr => pr.PaymentMethod != PaymentReceipt.PaymentType.Credit).Sum(pr => (decimal?)pr.Amount) ?? 0.0m )
                 ).ToList();

                var returnList = new Invoice[0];
                // initialize returned amounts to 0
                var returnedAmounts = dues.ToDictionary(inv => inv.Id, inv => 0m);
                if (type == Invoice.TansactionTypes.Sales)
                {
                    var salesInvoiceIds = dues.Select(inv => inv.Id).ToArray();
                    returnList = db.Invoices.Include(inv => inv.PaymentReceipts)
                                .Where(inv => inv.TransType == Invoice.TansactionTypes.SalesReturn
                                    && inv.Status == Invoice.InvoiceStatus.Approved
                                    && inv.RelatedInvoiceId.HasValue
                                    && salesInvoiceIds.Contains(inv.RelatedInvoiceId.Value)).ToArray();
                }

                var returns = returnList.GroupBy(inv => inv.RelatedInvoiceId)
                                                .ToDictionary(grp => grp.Key.Value,
                                                        grp => grp.Sum(inv => inv.GetTotal() - inv.GetNonCreditPaymentTotal()));
                foreach (var item in returns)
                {
                    returnedAmounts[item.Key] = item.Value;
                }
                
                var dueModels = dues.Select(inv => new DueInvoiceModel
                    {
                        Id = inv.Id,
                        ClientId = inv.ClientId.Value,
                        Date = inv.Time.ToString("yyyy-MM-dd hh:mm tt"),
                        Description = inv.Description,
                        TotalAmount = inv.GetTotal(),
                        ReturnAmount =  returnedAmounts[inv.Id],
                        DueAmount = inv.GetTotal() - inv.GetNonCreditPaymentTotal() - returnedAmounts[inv.Id],
                        FormattedInvNo = inv.FormattedInvoiceNo
                    });
                return dueModels;
            }

            return null;
        }

        public IEnumerable<RawMaterialBalance> GetRawMaterialBalances(DateTime? toDate)
        {
           var purchasesquery= db.Invoices.Where(i => i.Status == Invoice.InvoiceStatus.Approved && i.TransType == Invoice.TansactionTypes.Purchases);
           if(toDate.HasValue)
           {
               purchasesquery = purchasesquery.Where(i => i.Time <= toDate.Value);
           }
           var purchases = purchasesquery.SelectMany(invoice => invoice.Items)
               .Include(i=>i.Profile).Include(i=> i.Invoice)
               .Select(item => new { Id = item.Id, PurchQty = item.Qty , ProfId =item.ProfileId,Prof = item.Profile,ItemPurchase=item}).ToList();

           var salesquery = db.Invoices.Where(i => i.Status == Invoice.InvoiceStatus.Approved && i.TransType == Invoice.TansactionTypes.Sales);
           if (toDate.HasValue)
           {
               salesquery = salesquery.Where(i => i.Time <= toDate.Value);
           }
           var sales = salesquery.SelectMany(inv => inv.Items).Where(item => item.SourceRefId.HasValue)
                       .GroupBy(item => new { item.SourceRefId.Value, item.Profile })
               //quantity is retrived as widths
                       .Select(grp => new { Key = grp.Key, SalesQty = grp.Sum(i => i.Qty * i.Length) })
               //materialize results
                       .ToList()
               //select qty as areas
                       .Select(result => new { Id = result.Key.Value, SalesQty = result.SalesQty * ((ProductProfile)result.Key.Profile).Width });


           var salesReturnquery = db.Invoices.Where(i => i.Status == Invoice.InvoiceStatus.Approved && i.TransType == Invoice.TansactionTypes.SalesReturn);
           if (toDate.HasValue)
           {
               salesReturnquery = salesReturnquery.Where(i => i.Time <= toDate.Value);
           }

           var salesReturns = salesReturnquery.SelectMany(inv => inv.Items).Where(item => item.SourceRefId.HasValue)
                      .GroupBy(item => new { item.SourceRefId.Value, item.Profile })
               //quantity is retrived as widths
                      .Select(grp => new { Key = grp.Key, SalesQty = grp.Sum(i => i.Qty * i.Length) })
               //materialize results
                      .ToList()
               //select qty as areas
                      .Select(result => new { Id = result.Key.Value, SalesQty = result.SalesQty * ((ProductProfile)result.Key.Profile).Width });

           Dictionary<int, decimal> soldQtys = new Dictionary<int, decimal>();
           Dictionary<int, decimal> salesReturnQtys = new Dictionary<int, decimal>();

           foreach (var item in sales)
           {
               if(!soldQtys.ContainsKey(item.Id))
               {
                   soldQtys[item.Id] = 0m;
               }
               soldQtys[item.Id] += item.SalesQty;
           }
           foreach (var item in salesReturns)
           {
               if(!salesReturnQtys.ContainsKey(item.Id))
               {
                   salesReturnQtys[item.Id] = 0;
               }
               salesReturnQtys[item.Id] -= item.SalesQty;
           }

           var remaining = new List<RawMaterialBalance>();
           foreach (var purch in purchases)
           {
               var rmb = new RawMaterialBalance
               {
                   ItemId = purch.Id,
                   Profile = (RawMaterialProfile)purch.Prof,
                   ProfileId = purch.ProfId,
                   RemainingQuantity = purch.PurchQty,
                   ItemPurchase = purch.ItemPurchase
               };
               //turn purchase quantity into area
               rmb.RemainingQuantity *= rmb.Profile.Width;


               if (soldQtys.ContainsKey(purch.Id))
               {
                   rmb.RemainingQuantity -= soldQtys[purch.Id];
               }
               if(salesReturnQtys.ContainsKey(purch.Id))
               {
                   rmb.RemainingQuantity += salesReturnQtys[purch.Id];
               }

               //convert qty back to widths
               rmb.RemainingQuantity /= rmb.Profile.Width;

               if(rmb.RemainingQuantity > 0.00005m)
               {
                   remaining.Add(rmb);
               }
           }

           var balances = remaining;
           return balances;
        }

        public decimal GetRawMaterialBalancesValue(DateTime? toDate = null)
        {
            var balances = GetRawMaterialBalances(toDate);
            var value = balances.Sum(balance => balance.RemainingQuantity * balance.ItemPurchase.UnitPrice);

            return value;
        }

        private void ValidateInvoice(Invoice invoice)
        {

        }


        public void SetInvoiceGatePass(int invoiceId, string gatepass)
        {
            var invoice = db.Invoices.FirstOrDefault(inv => inv.Id == invoiceId);
            if(invoice == null)
            {
                throw new ArgumentException("Invoice not found");
            }

            if(invoice.TransType == Invoice.TansactionTypes.Sales && invoice.Status != Invoice.InvoiceStatus.Order)
            {
                invoice.GatePassRef = gatepass;
                db.Entry(invoice).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                throw new ArgumentException("Wrong type of invoice - not a sales invoice");
            }
        }


        public IEnumerable<Invoice> SearchInvoices(InvoiceSearchOptions options)
        {
            IQueryable<Invoice> invoices = db.Invoices.AsNoTracking();
            invoices = invoices.Where(i => i.TransType == options.TransType).Include(i => i.Client).OrderBy(i => i.Time).Include(i => i.Items);

            if(options.IncludePaymentReceipts)
            {
                invoices = invoices.Include(inv => inv.PaymentReceipts);
            }

            if (options.Range != null)
            {
                if (options.Range.Start.HasValue)
                {
                    invoices = invoices.Where(i => i.Time >= options.Range.Start.Value);
                }
                if (options.Range.End.HasValue)
                {
                    invoices = invoices.Where(i => i.Time <= options.Range.End.Value);
                }
            }
            if (options.Status.HasValue)
            {
                invoices = invoices.Where(i => i.Status == options.Status.Value);
            }

            if(!string.IsNullOrWhiteSpace( options.GatePass))
            {
                invoices = invoices.Where(inv => inv.GatePassRef.ToLower().Contains(options.GatePass.ToLower()));
            }
            if (!string.IsNullOrWhiteSpace(options.Description))
            {
                invoices = invoices.Where(inv => inv.Description.ToLower().Contains(options.Description.ToLower()));
            }
            if(options.RelatedInvoiceId.HasValue)
            {
                invoices = invoices.Where(inv => inv.RelatedInvoiceId == options.RelatedInvoiceId.Value);
            }


            if (options.ResultCount != null)
            {
                invoices = invoices.Take(options.ResultCount.Value);
            }

            return invoices.ToList();
        }


        public void CancelInvoice(int invoiceId, string username, DateTime updateTime)
        {
            var userManager = db.GetUserManager();
            var user = userManager.FindByName(username);

            var invoice = db.Invoices.FirstOrDefault(inv => inv.Id == invoiceId);
            if (invoice == null)
            {
                throw new ArgumentException("Invoice not found");
            }
            else
            {
                invoice.LastUpdatedBy = user;
                invoice.LastUpdateTime = updateTime;
                invoice.Status = Invoice.InvoiceStatus.Cancelled;
                db.SaveChanges();
            }

        }

        public Invoice LoadInvoiceByDisplayId(int id, Invoice.TansactionTypes transType, bool isOrder = false)
        {
            var allOfType = db.Invoices.Where(inv => inv.TransType == transType && inv.DisplayInvoiceNo == id);

            if(isOrder)
            {
                return allOfType.FirstOrDefault(inv => inv.Status == Invoice.InvoiceStatus.Order);
            }
            else
            {
                return allOfType.FirstOrDefault(inv => inv.Status != Invoice.InvoiceStatus.Order);
            }
        }
    }
}
