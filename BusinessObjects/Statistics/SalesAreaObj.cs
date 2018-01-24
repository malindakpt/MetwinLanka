using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Statistics
{
    public class SalesAreaObj
    {
        public String area { get; set; }
        public List<SalesAreaSub> monthSummary = new List<SalesAreaSub>();
    }
}
