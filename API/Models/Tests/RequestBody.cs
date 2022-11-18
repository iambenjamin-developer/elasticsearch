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

        private int _size;

        public int Size
        {
            get { return _size == 0 ? 10 : _size; }
            set { _size = value; }
        }

    }
}
