using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.DTOs;
using Warehouse.Domain.Entities;

namespace Warehouse.Core.Queries
{
    public class GetCategoryQuery : IRequest<Category>
    {
        public string Id { get; set; }

        public GetCategoryQuery(string id)
        {
            Id = id;
        }
    }
    public class GetALLCategoryQuery : IRequest<PagedResult<CategoryDto>>
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public string? Search { get; set; }


        public GetALLCategoryQuery(int page, int limit, string? search)
        {
            Page = page;
            Limit = limit;
            Search = search;
        }
    }

}
