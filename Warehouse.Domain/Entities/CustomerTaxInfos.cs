using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Warehouse.Domain.Entities
{
    public class CustomerTaxInfos : BaseEntity
    {
      
        public string CustomerId { get; set; }

        public string? CompanyName { get; set; }
        public string? TaxCode { get; set; }
        public string? Address { get; set; }

        public string? Email { get; set; }
        public string? BuyerName { get; set; }
        public string? CardId { get; set; }
        public string? BudgetCode { get; set; }
        public string? Phone { get; set; }
        public bool IsActive { get; set; }

      

    }
}
