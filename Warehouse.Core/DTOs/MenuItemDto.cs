namespace Warehouse.Core.DTOs
{
    public class MenuItemDto
    {
        public string Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Path { get; set; }
        public string? Icon { get; set; }
        public string ParentId { get; set; }
        public List<MenuItemDto> Children { get; set; } = new List<MenuItemDto>();
    }
}