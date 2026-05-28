using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.DTOs;

namespace Warehouse.Core.Commands
{
    public class CreateCustomersCommand : IRequest<string>
    {
        public CustomerDto Model { get; set; }

        public CreateCustomersCommand(CustomerDto model)
        {
            Model = model;
        }
    }
    public class CreateAddressCommand : IRequest<string>
    {
        public CustomerDto Model { get; set; }

        public CreateAddressCommand(CustomerDto model)
        {
            Model = model;
        }
    }
    public class UpdateCustomerCommand : IRequest<bool>
    {
        public CustomerDto Dto { get; set; }
        public string Id { get; set; }

        public UpdateCustomerCommand(CustomerDto dto, string id)
        {
            Dto = dto;
            Id = id;
        }
    }
    public class UpdateAddressCommand : IRequest<bool>
    {
        public CustomerDto Dto { get; set; }
        public string Id { get; set; }

        public UpdateAddressCommand(CustomerDto dto, string id)
        {
            Dto = dto;
            Id = id;
        }
    }
}
