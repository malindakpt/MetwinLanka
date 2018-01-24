using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public abstract class ItemProfile
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }
        [Display(Name="Active")]
        public bool IsEnabled { get; set; }

        public ItemProfile()
        {
            IsEnabled = true;
        }
    }
}
