using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Warehouse.Core.DTOs;

namespace Warehouse.Core.Commands
{
    public class ConfirmDeliveryCommand : IRequest<ConfirmDeliveryResponse>
    {
        public string OrderId { get; }
        public ConfirmDeliveryDto Model { get; }

        public ConfirmDeliveryCommand(string orderId, ConfirmDeliveryDto model)
        {
            OrderId = orderId;
            Model = model;
        }
    }
}
