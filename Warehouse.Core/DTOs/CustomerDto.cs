using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.DTOs
{
    public class CustomerDto 
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? Email { get; set; }
        public string? Phone { get; set; }

        public string? Gender { get; set; }
        public string? DateOfBirth { get; set; }

        public bool? AcceptMarketing { get; set; } = false;

        public string? Note { get; set; }

        public string? CustomerGroupId { get; set; }
        public string? CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdateAt { get; set; }
        public string? UpdateBy { get; set; }

        public string? IdAddress { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? FirstNamePersonRecive { get; set; } // họ ng nhận hàng
        public string? LastNamePersonRecive { get; set; } // tên ng nhận hàng
        public string? Company { get; set; } 
        public string? Mobile { get; set; } 
        public string?   Country { get; set; } 
        public string? PostalCode { get; set; } 
        public string? AddressLine { get; set; } // địa chỉ cụ thể
        public bool? IsDefault { get; set; }
        public List<AddressDto>? Addresses { get; set; }
        //thuế
        public string? CompanyName { get; set; } // công ty
        public string? TaxCode { get; set; } 
        public string? Address { get; set; } 
        public string? EmailTax { get; set; } 
        public bool? IsActiveTax { get; set; } // cho xuât hoa đơn
        public string? BuyerName { get; set; } 
        public string? CardId { get; set; } 
        public string? BudgetCode { get; set; } 
        public string? PhoneTax { get; set; } 
        public string? CreateBy { get; set; } 
   
       
        

    }

}
