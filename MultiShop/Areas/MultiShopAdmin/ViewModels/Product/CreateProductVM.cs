﻿using MultiShop.ViewModels;
using System.ComponentModel.DataAnnotations;

namespace MultiShop.Areas.MultiShopAdmin.ViewModels
{
    public class CreateProductVM
    {
        [Required(ErrorMessage = "Name must be entered mutled")]
        [MinLength(1, ErrorMessage = "It should not exceed 1-25 characters")]
        [MaxLength(25, ErrorMessage = "It should not exceed 1-25 characters")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Price must be entered mutled")]
        [Range(1, int.MaxValue, ErrorMessage = "Price must be greater than 0 ")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Order must be entered mutled")]
        [Range(1, int.MaxValue, ErrorMessage = "Order must be greater than 0 ")]
        public int OrderId { get; set; }
        [Required(ErrorMessage = "SKU must be entered mutled")]
        public string SKU { get; set; }
        [Required(ErrorMessage = "Description must be entered mutled")]
        [MinLength(1, ErrorMessage = "It should not exceed 1-100 characters")]
        [MaxLength(100, ErrorMessage = "It should not exceed 100 characters")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Category must be entered mutled")]
        [Range(1, int.MaxValue, ErrorMessage = "Category must be greater than 0 ")]
        public int? CategoryId { get; set; }
        public List<IncludeCategoryVM>? Categories { get; set; }
        public List<IncludeSizeVM>? Sizes { get; set; }
        public List<IncludeColorVM>? Colors { get; set; }
        public List<int> ColorIds { get; set; }
        public List<int> SizeIds { get; set; }
        [Required(ErrorMessage = "Image must be uploaded")]
        public IFormFile MainPhoto { get; set; }
        [Required(ErrorMessage = "Image must be uploaded")]
        public List<IFormFile> Photos { get; set; }
    }
}