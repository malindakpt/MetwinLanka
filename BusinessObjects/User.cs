using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace BusinessObjects
{
    public class User : IdentityUser
    {  
        [Required]
        [Display(Name="First Name")]
        public string FirstName { get; set; }

        [Display(Name = "Last Name")]
        [Required]
        public string LastName { get; set; }


        [Required]
        [Display(Name = "Phone")]
        public string Phone { get; set; }
    }
}
