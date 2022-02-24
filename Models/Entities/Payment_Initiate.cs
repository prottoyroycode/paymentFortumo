using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("Payment_Initiate")]
    public class Payment_Initiate:AuditTable
    {

       // public Guid Id { get; set; }
       
        public string MSISDN { get; set;}
        
        public string Service_MapID { get; set; }
       
        
        public string Operation_Reference { get; set; }
        public string Channel { get; set; }
        public string tokenValue { get; set; }
    }
}
