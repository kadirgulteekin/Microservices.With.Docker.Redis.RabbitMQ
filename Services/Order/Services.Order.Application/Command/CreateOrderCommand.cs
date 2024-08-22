using MediatR;
using Services.Order.Application.Dtos;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Order.Application.Command
{
    public class CreateOrderCommand : IRequest<ResponseDto<CreatedOrderDto>>
    {
        public string? BuyerId { get; set; }
        public List<OrderItemDto>?  OrderItemDtos { get; set; }
        public AddressDto? Address { get; set; }
    }
}
