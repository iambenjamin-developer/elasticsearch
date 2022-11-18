using System.Collections.Generic;
using System.Transactions;
using System;

namespace API.Models.Tickets
{
    public class FlowOutput
    {
        public long Id { get; set; }

        public string ParentJobId { get; set; }

        public string TraceId { get; set; }

        public string Entity { get; set; }

        public string JobId { get; set; }

        public TransactionStatus Status { get; set; }

        //public TransactionType Type { get; set; }

        public TransactionStep Step { get; set; }

        public long? ParentTransactionId { get; set; }

        public string Data { get; set; }

        public string TriggeredBy { get; set; }

        public long OperatorId { get; set; }

        public string Message { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime UpdatedDate { get; set; }

        public int MessagesCount { get; set; }

        public string LineNumberRelated { get; set; }

        //public Domain.Entities.Models.Services.Service Service { get; set; }

        public long? ServiceId { get; set; }

        public string LastJob { get; set; }

        //public ModificationAction ModificationAction { get; set; }

        //public ModificationType ModificationType { get; set; }

        public virtual ICollection<FlowOutput> ChildTransactions { get; set; }

        public string OntSerialNumber { get; set; }

        public FlowOutput()
        {
        }
    }
}
