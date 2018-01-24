using BusinessObjects.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GHM_ERP.Models
{
    [NotMapped]
    public class ProductProfileEditModel :ProductProfile
    {
        [Display(Name="Raw Materials")]
        public List<int> RawMaterialIds { get; set; }

        public ProductProfileEditModel()
        {
            RawMaterialIds = new List<int>();
        }

        public ProductProfile ToProductProfile(IEnumerable<RawMaterialProfile> profiles)
        {
            ProductProfile prof = new ProductProfile
            {
                Code = Code,
                Color = Color,
                Description = Description,
                Id = Id,
                IsEnabled = IsEnabled,
                Thickness = Thickness,
                Type = Type,
                UnitPrice = UnitPrice,
                Width = Width,
                ProductWidth = ProductWidth
            };

            foreach (var id in RawMaterialIds)
            {
                var rm = profiles.First(p => p.Id == id);
                prof.RawMaterialProfiles.Add(rm);
            }

            return prof;
        }
    }
}