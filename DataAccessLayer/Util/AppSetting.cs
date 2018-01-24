using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Util
{
    public class AppSetting
    {
        [Key]
        public string Key { get; set; }

        public int? IntValue { get; set; }

        public decimal? DecimalValue { get; set; }

        public string StringValue { get; set; }

    }
}
