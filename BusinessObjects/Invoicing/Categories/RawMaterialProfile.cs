using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Categories
{
    public class RawMaterialProfile : ItemProfile
    {
        public enum RawMaterialType
        {
            Coil, Sheet, Channel
        }


        [Required]
        public string Code { get; set; }
        
        [Required]
        public string Color { get; set; }
        [Required]
        public decimal Thickness { get; set; }
        [Required]
        public decimal Width { get; set; }
        [Required]
        public string Type { get; set; }

        public virtual ICollection<ProductProfile> ProductProfiles { get; set; }
    }
}
