using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class CustomerAddresses : BaseEntity
    {
        

        public string CustomerId { get; set; } = null!;

        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Company { get; set; }

        public string? Phone { get; set; }

        public string? Country { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }

        public string? AddressLine { get; set; }
        public string? PostalCode { get; set; }

        public bool IsDefault { get; set; } = false;

        

    }
}
