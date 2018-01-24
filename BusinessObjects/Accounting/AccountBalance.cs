using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    [NotMapped]
    public class AccountBalance
    {
        public Account Account { get; set; }
        public decimal Balance { get; set; }

        public AccountBalance (Account ac, decimal bal){
            this.Account = ac;
            this.Balance = bal;
        }

        public AccountBalance()
        { 
        }
    }
}
