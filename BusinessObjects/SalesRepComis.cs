using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class SalesRepComis
    {
        public DateTime inv_date { get; set; }
        public int inv_no { get; set; }
        public decimal inv_amount { get; set; }
        public double commision { get; set; }
        public string Client { get; set; }
        public string DisplayInvNo { get; set; }


    }
}
