using MediatR;
using Microsoft.EntityFrameworkCore;
using Warehouse.Core.DTOs;
using Warehouse.Core.Interfaces;
using Warehouse.Core.Queries;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Handlers;

public class GetCategoryQueryHandler
    : IRequestHandler<GetCategoryQuery, Category?>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetCategoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Category?> Handle(GetCategoryQuery request, CancellationToken cancellationToken)
    {
        return await _unitOfWork.CategoryRepository.GetByIdAsync(request.Id);
    }
}


public class GetAllCategoryQueryHandler
    : IRequestHandler<GetALLCategoryQuery, PagedResult<CategoryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllCategoryQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<CategoryDto>> Handle(GetALLCategoryQuery request, CancellationToken cancellationToken)
    {
        var query = _unitOfWork.CategoryRepository
            .Query(); // IQueryable<Category>

        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            query = query.Where(x => x.Name.Contains(request.Search));
        }

        var total = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip((request.Page - 1) * request.Limit)
            .Take(request.Limit)
            .Select(x => new CategoryDto
            {
                Id = x.Id,
                Name = x.Name,
                ImageUrl = x.ImageUrl
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<CategoryDto>
        {
            Items = items,
            Total = total
        };
    }
}