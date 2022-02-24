using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models.Entities
{
    [Table("Service_Map")]
    public class Service_Map
    {
        [Key]
        public int Id { get; set; }
        public string ServiceId { get; set; }
        public string ServiceName { get; set; } 
        public decimal PriceOrAmount { get; set; }
        public string Currency { get; set; }
        public string Country_Code { get; set; }
        public string Channel_Code { get; set; }
        public int DurationInDays { get; set; }
        //public Payment_Initiate payment_Initiate { get; set; }
    }
    public static class ServiceUnit
    {
        public static string Monthly { get; set; }
        public static string HalfYearly { get; set; }

    }
}
