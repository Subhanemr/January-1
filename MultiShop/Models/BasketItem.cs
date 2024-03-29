﻿using MultiShop.Models.Common;

namespace MultiShop.Models
{
    public class BasketItem : BaseEntity
    {
        public decimal Price { get; set; }
        public int Count { get; set; }
        public string AppUserId { get; set; } = null!;
        public AppUser AppUser { get; set; } = null!;
        public int ProductId { get; set; } 
        public Product Product { get; set; } = null!;
        public int? OrderId { get; set; }
        public Order? Order { get; set; }

    }
}
