using System;
using System.Collections.Generic;

namespace API.Models.Tests
{
    public class RequestBody
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public int Stock { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }

        private int _size;

        public int Size
        {
            get { return _size == 0 ? 10 : _size; }
            set { _size = value; }
        }

        public List<string> KeyWords { get; set; } = new List<string>();
        public List<int> Quantity { get; set; } = new List<int>();

    }
}
