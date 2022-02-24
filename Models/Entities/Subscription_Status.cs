using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("Subscription_Status")]
    public class Subscription_Status
    {
        [Key]
        public int Id { get; set; }
        public string MSISDN { get; set; }
        public bool Reg_Status { get; set; }
        public DateTime Reg_Date { get; set; }
        public DateTime ExpiryDate { get; set; }
        public DateTime Last_Update { get; set; }
        public string ServiceMap_Id { get; set; }
        public string Operation_reference { get; set; }
       
        public string Subscription_uuid { get; set; }


    }
}
