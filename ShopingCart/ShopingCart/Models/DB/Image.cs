using System;
using System.ComponentModel.DataAnnotations;

namespace ShopingCart.Models.DB
{
    public class Image
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "ProductId is required")]
        public Guid ProductId { get; set; }

        [Required(ErrorMessage = "Url is required")]
        [Url(ErrorMessage = "Invalid URL format")]
        public string Url { get; set; }
    }
}
