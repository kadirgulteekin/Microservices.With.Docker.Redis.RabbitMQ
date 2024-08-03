﻿using Services.Order.Domain.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.Order.Domain.OrderAggregate
{
    public class Address : ValueObject
    {
        public string? Province { get;private set; }
        public string? District { get;private set; }
        public string? Street { get;private set; }
        public string? ZipCode { get; private set; }
        public string? Line { get; private set; }

        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            return new List<object>
            {
                Province ?? string.Empty,
                District ?? string.Empty,
                Street ?? string.Empty,
                ZipCode ?? string.Empty,
                Line ?? string.Empty,
            };
           
        }
    }
}
