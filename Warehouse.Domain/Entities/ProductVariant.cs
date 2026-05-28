namespace Warehouse.Domain.Entities
{
    public class ProductVariant
    {
        public string Id { get; set; }

        public string ProductId { get; set; }

        public string SKU { get; set; }

        public string Barcode { get; set; }

        public string Unit { get; set; }

        public decimal Price { get; set; }

        public decimal CostPrice { get; set; }

        public Product Product { get; set; }
    }
}