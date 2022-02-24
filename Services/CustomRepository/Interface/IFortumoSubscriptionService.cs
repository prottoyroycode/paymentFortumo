using Library.Core.Models.CallBackModel;
using Library.Core.ViewModels;
using Models.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services.CustomRepository.Interface
{
    public interface IFortumoSubscriptionService
    {
        Task<Response> CreateSubscriptionAsync(string token);
        Task<Response> PaymentInitiate(Payment_Initiate payment_Initiate);
        Task<Response> GetAllServicesAsync();
        Task<Response> GetSingleServiceAsync(string serviceId);
        Task<Service_Map> GetSingleAsync(string serviceId);
        Task<bool> CheckSubscriptionValidity(Service_Map service_Map,int days);
        Task<CustomResponse<CallBackHistory>> InsertCallBackDataAsync(CallBackHistory callBackHistory);
        Task<CustomResponse<OneTimeCallBackData>> InsertOneTimeCallBackDataAsync(OneTimeCallBackData oneTimeCallBackData);
        Task<Response> InsertPaymentStatusAsync(Payment_History payment_History);
        Task<Payment_Initiate> GetPaymentInitDataByOperationRef(string operation_ref);
        Task<Response> InsertIntoSubscriptionStatus(Subscription_Status subscription_Status);
        Task<bool> CheckIfAlreadySubscriped(SubscriptionVM subscriptionVM);
        Task<Response> CheckPackageStatus(SubscriptionVM subscriptionVM);
        Task<Response> InsertUnSubscripeDataIntoDb(UnSubscripeCallBackHistory unSubscripeCallBackHistory);
        Task<Payment_History> GetPaymentDataByOperationReference(string operation_Ref);



    }
}
