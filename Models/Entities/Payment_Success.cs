using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("Payment_History")]
    public class Payment_History
    {
        [Key]
        public Guid Id { get; set; }
        public string operation_reference { get; set; }
        public string MSISDN { get; set; }
        public string subscription_status { get; set; }
        public string billing_status { get; set; }
        public DateTime service_starts_at { get; set; }
        public DateTime service_ends_at { get; set; }
        public string action { get; set; }
        public string payment_transaction_id { get; set; }
        public DateTime timestamp { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        
    }
}
