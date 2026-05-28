namespace Warehouse.Core.DTOs
{
    public class AddressDto
    {
        public string? Id { get; set; }
        public string? IdAddress { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Ward { get; set; }
        public string? FirstName { get; set; } // họ ng nhận hàng
        public string? LastName { get; set; } // tên ng nhận hàng
        public string? Company { get; set; }
        public string? Mobile { get; set; }
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
        public string? AddressLine { get; set; } // địa chỉ cụ thể
        public bool? IsDefault { get; set; }
    }
}