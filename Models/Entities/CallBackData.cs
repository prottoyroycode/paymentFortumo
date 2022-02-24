using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("CallBackHistory")]
    public class CallBackHistory 
    {
        
        [Key]
        public Guid Id { get; set; }
        public string subscription_uuid { get; set; }
        public string charging_token { get; set; }
        public string merchant { get; set; }
        public string operation_reference { get; set; }
        public string subscription_status { get; set; }
        public string billing_status { get; set; }
        public DateTime service_starts_at { get; set; }
        public DateTime service_ends_at { get; set; }
        public string consumer_identity { get; set; }
        public string action { get; set; }
        public string unsubscribe_url { get; set; }
        public string payment_transaction_id { get; set; }
        public DateTime timestamp { get; set; }
        public string token { get; set; }
        public string uuid { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string allDataInJson { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //public static implicit operator CallBackHistory(EntityEntry<CallBackHistory> v)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
