using System;

namespace API.Models.Products
{
    public class CreateProductDto
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime? DateOfExpiration { get; set; }
        public int? DealerId { get; set; }
        public decimal Price { get; set; }
        public bool Enabled { get; set; }
    }
}
