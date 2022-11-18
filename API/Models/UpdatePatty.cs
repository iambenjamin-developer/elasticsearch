using System;

namespace API.Models
{
    public class UpdatePatty
    {
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime DateOfElaboration { get; set; }
    }
}
