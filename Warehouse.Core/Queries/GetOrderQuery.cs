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
    public class GetOrderQuery : IRequest<OrderDto>
    {
        public string Id { get; set; }

        public GetOrderQuery(string id)
        {
            Id = id;
        }
    }
    public class GetAllOrderQuery : IRequest<PagedResult<OrderDto>>
    {
        public int Page { get; set; }
        public int Limit { get; set; }
        public string? Search { get; set; }


        public GetAllOrderQuery(int page, int limit, string? search)
        {
            Page = page;
            Limit = limit;
            Search = search;
        }
    }

}
