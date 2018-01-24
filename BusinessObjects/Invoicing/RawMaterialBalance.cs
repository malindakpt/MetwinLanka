using BusinessObjects.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects.Invoicing
{
    [NotMapped]
    public class RawMaterialBalance
    {
        public int ItemId { get; set; }
        public Item ItemPurchase { get; set; }
        public RawMaterialProfile Profile {get;set;}
        public int ProfileId { get; set; }
        public decimal RemainingQuantity { get; set; }
    }
}
