using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.DTOs;
namespace Warehouse.Core.Commands
{


    public class CreateProductCommand : IRequest<string>
    {
        public ProductCreateDto Model { get; set; }

        public CreateProductCommand(ProductCreateDto model)
        {
            Model = model;
        }
    }
    public class UpdateProductCommand : IRequest<bool>
    {
        public ProductCreateDto Dto { get; set; }
        public string Id { get; set; }

        public UpdateProductCommand(ProductCreateDto dto, string id)
        {
            Dto = dto;
            Id = id;
        }
    }
    public class DeleteProductCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public DeleteProductCommand(string id)
        {
            Id = id;
        }
    }


    public class DeleteProductIdsCommand : IRequest<bool>
    {
        public List<string> Ids { get; set; }
    }
}
