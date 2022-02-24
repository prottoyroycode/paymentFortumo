using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Models
{
    public class SubscriptionVM
    {
        [Required(ErrorMessage ="please provide a valid service id")]
        public string Service_MapId { get; set; }
        [Required(ErrorMessage ="please insert mobile number")]
        public string Mobile { get; set; }
        [Required(ErrorMessage = "please select channel")]
        public string Channel { get; set; }
        //[Required(ErrorMessage = "please enter a valid mssisdn")]
        //public string MSISDN { get; set; }

        ////  public Guid Service_MapID { get; set; }
        //[Required(ErrorMessage = "mandatory field")]
        //public string Country_Code { get; set; }

        //[Required(ErrorMessage = "mandatory field")]
        //public double Price { get; set; }
        //[Required(ErrorMessage = "mandatory field")]
        //public string Currency { get; set; }
        //[Required(ErrorMessage = "mandatory field")]
        //public string Duration { get; set; }
    }
}
