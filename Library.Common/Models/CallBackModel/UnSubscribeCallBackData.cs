using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Models.CallBackModel
{
    public class UnSubscribeCallBackData
    {
        public string subscription_uuid { get; set; }
        public string charging_token { get; set; }
        public string merchant { get; set; }
        public string operation_reference { get; set; }
        public string subscription_status { get; set; }
        public string billing_status { get; set; }
        public DateTime service_starts_at { get; set; }
        public DateTime service_ends_at { get; set; }
        public string consumer_identity { get; set; }
        public ErrorCallBack error { get; set; }
        public string timestamp { get; set; }
        public Price price { get; set; }
        public string action { get; set; }
        public string unsubscribe_url { get; set; }

    }
    public class ErrorCallBack
    {

    }
    public class Price
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }
}
