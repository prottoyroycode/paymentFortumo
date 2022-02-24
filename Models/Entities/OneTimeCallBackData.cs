using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("OneTimeCallBackData")]
    public class OneTimeCallBackData 
    {
        [Key]
        public int Id { get; set; }
        public string transaction_id { get; set; }
        public string transaction_state { get; set; }
        public string merchant { get; set; }
        public string operation_reference { get; set; }
        public string consumer_identity { get; set; }
        public DateTime timestamp { get; set; }
        public decimal amount { get; set; }
        public string currency { get; set; }
        public string AllData { get; set; }
        public string MSISDN { get; set; }
        public DateTime CreateDate { get; set; } = DateTime.Now;
        
    }
}
