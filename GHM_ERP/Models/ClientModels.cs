using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class LocationAddModel
    {
        [Required]
        [StringLength(75)]
        public string Name { get; set; }
    }
}