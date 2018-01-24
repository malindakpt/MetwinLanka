using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class DateRange
    {
        public DateTime? Start { get; set; }
        public DateTime? End { get; set; }

        public DateRange(DateTime d1, DateTime d2)
        {
            this.Start = d1;
            this.End = d2;
        }

        public DateRange()
        { 
        }
    }
}
