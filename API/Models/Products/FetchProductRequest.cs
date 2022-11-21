﻿using API.Models.Commons;
using System;

namespace API.Models.Products
{

    public class FetchProductRequest : QueryStringParameters
    {
        public long Id { get; set; }
        public string Guid { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime? DateOfExpiration { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? DealerId { get; set; }
        public decimal Price { get; set; }
        public bool Enabled { get; set; }
    }
}
