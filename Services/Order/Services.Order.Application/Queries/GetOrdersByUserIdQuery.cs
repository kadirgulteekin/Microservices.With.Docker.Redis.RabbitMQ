﻿using Azure;
using MediatR;
using Services.Order.Application.Dtos;
using Shared.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Order.Application.Queries
{
    public class GetOrdersByUserIdQuery : IRequest<ResponseDto<List<OrderDto>>>
    {
        public string? UserId { get; set; }

    }

   
}
