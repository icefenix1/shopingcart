﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ShopingCart.Models.DB
{
    public class Cart
    {
        public Guid Id { get; set; }

        [Required(ErrorMessage = "ProductIds are required")]
        public List<Guid> ProductIds { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Customer { get; set; }
    }
}