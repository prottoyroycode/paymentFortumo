using System;
using System.Collections.Generic;
using System.Text;

namespace Library.Core.Models.CallBackModel
{
    public class SuccessfulCallBack
    {
        public SuccessfulCallBack()
        {
            this.metadata = new Metadata();
            this.error = new Error();
            this.price = new PriceModel();
        }
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
        public Error error { get; set; }
        public Metadata metadata { get; set; }
        public PriceModel price { get; set; }
        public PricePromotionModel promotion_price { get; set; }
       

    }
    public class Error
    {

    }
    public class Metadata
    {
        public string token { get; set; }
        public string uuid { get; set; }
        public string fortumo_hdcb_session_uuid { get; set; }
        public string merchant_redirect_url { get; set; }
        public string subscription_origin { get; set; }
    }
    public class PriceModel
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }
    public class PricePromotionModel
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
    }
}
