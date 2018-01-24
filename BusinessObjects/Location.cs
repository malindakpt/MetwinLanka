using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessObjects
{
    public class Location
    {
        public enum Districts
        {
            None,
            Hambantota,
            Matara,
            Galle,
            Ratnapura,
            Kalutara,
            Colombo,
            Nuwaraeliya,
            Gampaha,
            Kegalle,
            Kandy,
            Badulla,
            Batticaloa,
            Ampara,
            Monaragala,
            Kurunegala,
            Trincomalee,
            Mullaitivu,
            Polonnaruwa,
            Puttalam,
            Vavuniya,
            Kilinochchi,
            Matale,
            Anuradhapura,
            Mannar
        }

        public Location()
        {
            Clients = new List<Client>();
        }

        public Location(string name, Districts district, string country) : this()
        {
            this.Name = name;
            this.District = district;
            this.Country = country;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public Districts District { get; set; }
        public string Country { get; set; }

        public virtual ICollection<Client> Clients { get; set; }

    }
}
