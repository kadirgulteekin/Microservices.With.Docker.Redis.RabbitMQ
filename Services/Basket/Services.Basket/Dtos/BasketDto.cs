﻿namespace Services.Basket.Dtos
{
    public class BasketDto
    {
        public string? UserId { get; set; }
        public string? DiscountCode { get; set; }
        public required List<BasketItemDto> BasketItems { get; set; }

        public decimal TotalPrice => BasketItems.Sum(x => x.Price * x.Quantity);
    }
}

