using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Client
    {
        public Client(string name,Types type,string phone,string address,int? location)
        {
            IsActive = true;
            this.Name = name;
            this.Type = type;
            this.Phone = phone;
            this.Address = address;
            this.LocationId = location;
        }

        public Client()
        {
            IsActive = true;
        }

        public enum Types
        {
            Customer, Supplier
        }

        [Key]
        [ForeignKey("Account")]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Types Type { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        
        [Display(Name="Location")]
        public int? LocationId { get; set; }
        public virtual Location Location { get; set; }

        [Display(Name="Active")]
        public bool IsActive { get; set; }

        public virtual Account Account { get; set; }
        public virtual IEnumerable<Invoice> Invoices{get; set;}
    }
}
