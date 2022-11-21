namespace API.Models.Commons
{
    public class QueryStringParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string FilterBy { get; set; }
        public object StrictValue { get; set; }
        public object MinValue { get; set; }
        public object MaxValue { get; set; }
        public string OrderBy { get; set; }
        public object Order { get; set; }
    }
}
