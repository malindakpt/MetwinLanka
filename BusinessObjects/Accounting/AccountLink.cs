using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class AccountLink
    {
        public enum LinkType
        {
            Payable, Depreciation
        }

        [Required]
        [Key, Column(Order = 1)]
        public int MainAccountId { get; set; }
        public virtual Account MainAccount { get; set; }

        [Required]
        [Key, Column(Order = 2)]
        public LinkType Link { get; set; }


        public virtual Account SecondaryAccount { get; set; }


    }
}
