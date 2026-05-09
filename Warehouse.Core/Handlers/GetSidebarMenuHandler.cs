using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Core.Queries;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers;

public class GetSidebarMenuHandler : IRequestHandler<GetSidebarMenuQuery, List<MenuItemDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetSidebarMenuHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<List<MenuItemDto>> Handle(GetSidebarMenuQuery request, CancellationToken cancellationToken)
    {
        // 1. Lấy toàn bộ danh sách menu từ DB về (Chỉ 1 lần truy vấn duy nhất)
        // Giả sử GetAllAsync() trả về Task<List<MenuItem>>
        var allItems = await _unitOfWork.MenuItemRepository.GetAll();

        // 2. Chuyển toàn bộ danh sách sang DTO (để tránh lỗi tham chiếu DB) 
        // và tạo Dictionary để tra cứu nhanh theo ParentId
        var allDtos = allItems.Select(x => new MenuItemDto
        {
            Id = x.Id,
            Title = x.Title,
            Path = x.Path,
            Icon = x.Icon,
            ParentId = x.ParentId, // Đảm bảo MenuItemDto có trường này
            Children = new List<MenuItemDto>()
        }).ToList();

        // 3. Xây dựng cấu trúc cây
        var lookup = allDtos.ToLookup(x => x.ParentId);
        var rootItems = allDtos.Where(x => x.ParentId == null).ToList();

        foreach (var item in allDtos)
        {
            // Gán các con tương ứng dựa trên ParentId
            item.Children = lookup[item.Id].ToList();
        }

        return rootItems;
    }
}