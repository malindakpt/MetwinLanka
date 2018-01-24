using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Categories
{
    public class ProductProfile : ItemProfile
    {
        public enum ProductType
        {
            Gutter,Sheet,Channel
        }

        [Required]
        public string Code { get; set; }
        [Required]
        public string Type { get; set; }
        [Required]
        public string Color { get; set; }

        [Range(0,1000)]
        public decimal Thickness { get; set; }

        [Required]
        [Range(0,1000)]
        [Display(Name = "Material Width")]
        public decimal Width { get; set; }

        /// <summary>
        /// Display Width
        /// </summary>
        [Required]
        [Range(0, 1000)]
        [Display(Name ="Display Width")]
        public decimal ProductWidth { get; set; }

        [Required]
        [Range(0,10000000,ErrorMessage="Unit price cannot be negative")]
        [Display(Name="Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name="Related Raw Materials")]
        public virtual ICollection<RawMaterialProfile> RawMaterialProfiles { get; set; }


        public ProductProfile()
        {
            RawMaterialProfiles = new List<RawMaterialProfile>();
        }
    }
}
