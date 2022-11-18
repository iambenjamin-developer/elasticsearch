using System;

namespace API.Models.Tests
{
    public class RequestBody
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
    }
}
