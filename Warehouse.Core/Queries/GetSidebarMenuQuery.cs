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
    public record GetSidebarMenuQuery() : IRequest<List<MenuItemDto>>;
}
