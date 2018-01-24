using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Invoicing
{
    public class PaymentInstance
    {
        public int Id { get; set; }

        public DateTime Time { get; set; }

        public PaymentInstance()
        {
            Time = DateTime.Now;
        }
    }
}
