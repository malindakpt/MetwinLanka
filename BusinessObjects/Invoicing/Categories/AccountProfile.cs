using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Categories
{
    public class AccountProfile : ItemProfile
    {
        [Required]
        [Display(Name="Account")]
        public int AccountId { get; set; }
        public Account Account { get; set; }
    }
}
