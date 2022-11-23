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
                DateTime dateExpiration;
                DateTime.TryParse(_filterByDateOfExpiration, out dateExpiration);

                return dateExpiration == DateTime.MinValue ? null : dateExpiration.ToString("yyyy-MM-dd");
            }
            set { _filterByDateOfExpiration = value; }
        }

        public string FilterDateFrom
        {
            get
            {
                DateTime dateFrom;
                DateTime.TryParse(_filterDateFrom, out dateFrom);

                return dateFrom == DateTime.MinValue ? null : dateFrom.ToString("yyyy-MM-ddT00:00:00Z");
            }
            set { _filterDateFrom = value; }
        }

        public string FilterDateTo
        {
            get
            {
                DateTime dateTo;
                DateTime.TryParse(_filterDateTo, out dateTo);

                return dateTo == DateTime.MinValue ? null : dateTo.ToString("yyyy-MM-ddT23:59:59Z");
            }
            set { _filterDateTo = value; }
        }


    }
}
