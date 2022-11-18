using System.Transactions;

namespace API.Models.Tickets
{
    //[ElasticsearchType(Name = "atenea-transaction", IdProperty = "Id")]
    public class Transaction : BaseEntity
    {
        public long Id { get; set; }

        public TransactionStatus Status { get; set; }

        public TransactionStep Step { get; set; }

        public string Document { get; set; }

        public string TriggeredBy { get; set; } // Para el usuario que lanza la transacción

        public long? OperatorId { get; set; }

        public string OntSerialNumber { get; set; }

    }
}
