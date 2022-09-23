using System;

namespace API.Models
{
    public class Patty
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime DateOfElaboration { get; set; }
    }
}
