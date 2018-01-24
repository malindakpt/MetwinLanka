using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    public class SettingsModel 
    {
        [Display(Name="VAT Percentage")]
        [Range(0,400)]
        [Required]
        public decimal VatCharge { get; set; }

        [Display(Name ="Address")]
        [Required]
        [DataType(DataType.MultilineText)]
        public string Address { get; set; }

        [Display(Name = "VAT Number")]
        public string VatNumber { get; set; }

        [Display(Name = "Hide Unit Price in VAT")]
        [Required]
        public bool HideUnitPriceInVat { get; set; }
    }
}