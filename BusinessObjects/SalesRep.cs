using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class SalesRep
    {
        
        public int Id { get; set; }
        [Required]
        [Display(Name="First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name="Last Name")]
        public string LastName { get; set; }
        public string Address { get; set; }
        [Required]
        public string Phone { get; set; }

        public virtual ICollection<Invoice> Invoices { get; set; }

        [NotMapped]
        [Display(Name="Full Name")]
        public string FullName
        {
            get
            {
                return string.Join(" ", FirstName, LastName);
            }
        }
    }
}
