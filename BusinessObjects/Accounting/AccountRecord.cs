using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Accounting
{
    public class AccountRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Desc { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }

        public AccountRecord(int Id,DateTime date, string des,decimal de,decimal cr)
        {
            this.Id = Id;
            this.Date = date;
            this.Desc = des;
            this.Debit = de;
            this.Credit = cr;
        }
    }
}
