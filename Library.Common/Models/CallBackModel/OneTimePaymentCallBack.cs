using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Models.CallBackModel
{
    public class OneTimePaymentCallBack
    {
        public  string transaction_id { get; set; }
        public string transaction_state { get; set; }
        public string merchant { get; set; }
        public string operation_reference { get; set; }
        public string consumer_identity { get; set; }
        public Error error { get; set; } 
        public DateTime timestamp { get; set; }
        public OneTimePrice price { get; set; }
        

    }
    public class OneTimePrice
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }
    
}
