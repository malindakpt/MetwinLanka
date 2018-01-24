using BusinessObjects.Categories;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace BusinessObjects
{
    public  class Item
    {

        public Item()
        {
            Qty = 1.0m;
            Length = 1.0m;
        }

        public int Id { get; set; }

        public string Reference { get; set; }
        public string Description { get; set; }

        [Required]
        public int ProfileId { get; set; }
        public virtual ItemProfile Profile { get; set; }

        [Required]
        public decimal Qty { get; set; }

        public decimal Length { get; set; }

        public decimal Discount { get; set; }
        //Different prices for different customers. Not : Qty * Unit Price
        [Required]
        public decimal Price { get; set; }

        [NotMapped]
        public decimal UnitPrice
        {
            get
            {
                return Price / (Qty * Length);
            }
        }

        /// <summary>
        /// RawMaterial Cost. Valid only for Sales Items
        /// </summary>
        [NotMapped]
        public decimal RawMaterialCost
        {
            get
            {
                var rmCostPerUnitArea = SourceRef.UnitPrice/((RawMaterialProfile)SourceRef.Profile).Width;
                var totalArea = Qty * Length * ((ProductProfile)Profile).Width;
                return rmCostPerUnitArea * totalArea;
            }
        }

        //Raw material Ref No
        public int? SourceRefId { get; set; }
        public virtual Item SourceRef { get; set; }

        [InverseProperty("SourceRef")]
        // Items Produced from Rawmaterials
        public virtual ICollection<Item> Produced { get; set; }
        //invoice
        public int InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; }

    }
}
