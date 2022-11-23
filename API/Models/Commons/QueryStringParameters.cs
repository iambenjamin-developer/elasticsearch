using System;

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

        /*
2022-01-01T00:00:00Z
2022-12-31T23:59:59Z

(DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ssZ"));
         */
        private string _filterByDateOfExpiration;
        private string _filterDateFrom;
        private string _filterDateTo;

        public string FilterByDateOfExpiration
        {
            get
            {
                var dateExpiration = Convert.ToDateTime(_filterByDateOfExpiration);

                return dateExpiration.ToString("yyyy-MM-ddT00:00:00Z");
            }
            set { _filterByDateOfExpiration = value; }
        }

        public string FilterDateFrom
        {
            get
            {
                var dateFrom = Convert.ToDateTime(_filterDateFrom);

                return dateFrom.ToString("yyyy-MM-ddT00:00:00Z");
            }
            set { _filterDateFrom = value; }
        }

        public string FilterDateTo
        {
            get
            {
                var dateTo = Convert.ToDateTime(_filterDateTo);

                return dateTo.ToString("yyyy-MM-ddT23:59:59Z");
            }
            set { _filterDateTo = value; }
        }


    }
}
