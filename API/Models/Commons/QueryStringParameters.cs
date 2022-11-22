namespace API.Models.Commons
{
    public class QueryStringParameters
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string Order { get; set; }
        //public string FilterBy { get; set; }
        //public object StrictValue { get; set; }
        //public object MinValue { get; set; }
        //public object MaxValue { get; set; }
        //public string OrderBy { get; set; }
        //public object Order { get; set; }

        public string FilterById { get; set; }
        public string FilterByGuid { get; set; }
        public string FilterByName { get; set; }
        public string FilterByStock { get; set; }
        public string FilterByDateOfExpiration { get; set; }
    }
}
