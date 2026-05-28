using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.DTOs;
namespace Warehouse.Core.Commands
{


    public class CreateCategoryCommand : IRequest<string>
    {
        public CategoryCreateDto Model { get; set; }

        public CreateCategoryCommand(CategoryCreateDto model)
        {
            Model = model;
        }
    }
    public class UpdateCategoryCommand : IRequest<bool>
    {
        public CategoryUpdateDto Dto { get; set; }
        public string Id { get; set; }

        public UpdateCategoryCommand(CategoryUpdateDto dto, string id)
        {
            Dto = dto;
            Id = id;
        }
    }
    public class DeleteCategoryCommand : IRequest<bool>
    {
        public string Id { get; set; }
        public DeleteCategoryCommand(string id)
        {
            Id = id;
        }
    }


    public class DeleteCategoryIdsCommand : IRequest<bool>
    {
        public List<string> Ids { get; set; }
    }


}