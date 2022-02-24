using Library.Core.Models.CallBackModel;
using Library.Core.ViewModels;
using Microsoft.EntityFrameworkCore;
using Models.DataContext;
using Models.Entities;
using Services.CustomRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ViewModels;

namespace Services.CustomRepository.Implementations
{
    public class AvailablePackage
    {
        public bool Monthly { get; set; }
        public bool halfYearly { get; set; }
    }
    public class FortumoSubscriptionService : IFortumoSubscriptionService
    {
        public readonly EfDbContext _context;
        public FortumoSubscriptionService(EfDbContext context)
        {
            _context = context;
        }

        public async Task<bool>CheckIfAlreadySubscriped(SubscriptionVM subscriptionVM)
        {
            //before business change
            //var checkStatus = await _context.Subscription_Statuses.Where(s => s.MSISDN == subscriptionVM.Mobile
            //&& s.ServiceMap_Id == subscriptionVM.Service_MapId && s.Reg_Status == true).OrderByDescending(o =>o.Reg_Date).ToListAsync();
            //var takeLastStatus = checkStatus.First();
            //var reqDate = DateTime.Now;
            //var expiryDate = takeLastStatus.ExpiryDate;
            //var difference = expiryDate - reqDate;
            //var days = difference.TotalDays;
            //if (days > 0)
            //{
            //    return true;
            //   // Console.WriteLine("exist");
            //}
            var checkStatus = await _context.Subscription_Statuses.FirstOrDefaultAsync(s => s.MSISDN == subscriptionVM.Mobile && s.Reg_Status==true);
            if(checkStatus is null || checkStatus ==null)
            {
                return false;
            }
            if (checkStatus !=null)
            {
                var reqDate = DateTime.Now;
                var expiryDate = checkStatus.ExpiryDate;
                var difference = expiryDate - reqDate;
                var days = difference.TotalDays;
                if (days > 0)
                {
                    return true;
                    // Console.WriteLine("exist");
                }
            }
            

            return false;
        }

        public async Task<Response> CheckPackageStatus(SubscriptionVM subscriptionVM)
        {
            var availablePackage = new List<Service_Map>();
            var response = new Response();
            var available = new Service_Map();
            try
            {
                var data = await _context.Subscription_Statuses.Where(s => s.Reg_Status == true && s.MSISDN == subscriptionVM.Mobile &&
               s.ServiceMap_Id == subscriptionVM.Service_MapId ).OrderByDescending(o => o.Reg_Date).ToListAsync();
               
                    var takeLastStatus = data.First();
                    var durationInDays = (takeLastStatus.ExpiryDate - takeLastStatus.Reg_Date).TotalDays;
                    //if (durationInDays <= 30 && durationInDays > 0)
                    //{
                    //    available.ServiceName = "Monthly";
                    //}
                    if (durationInDays> 0)
                {
                    available.ServiceId = takeLastStatus.ServiceMap_Id;

                    availablePackage.Add(available);

                    response.Data = availablePackage;
                    response.Message = "available packages ";
                    response.Status = true;
                    response.StatusCode = StatusCode.Success;
                    response.TotalRecords = 1;
                }
                else
                {
                    response.Data = null;
                    response.Message = "no available packages ";
                    response.Status = false;
                    response.StatusCode = StatusCode.NoDataAvailable;
                    response.TotalRecords = 0;
                }
                    
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                response.StatusCode = StatusCode.InternalServerError;
                response.Data = null;
            }

            return response;
        }

        public async Task<bool> CheckSubscriptionValidity(Service_Map service_Map,int dayCount)
        {

            var data = await _context.Payment_Initiates.SingleOrDefaultAsync(s => s.Service_MapID == service_Map.ServiceId);
            if (data == null)
            {
                return false;
            }

            if (data != null)
            {
                var createdDateTime = data.CreatedOn;
                var requestdateTime = DateTime.Now;
                var difference = requestdateTime - createdDateTime;
                var days = difference.TotalDays;
                if (days <= dayCount)
                    return true;

            }

            return false;
        }

        //  private static readonly HttpClient client = new HttpClient();
        public async Task<Response> CreateSubscriptionAsync(string token)
            {
            var client = new HttpClient();
            var serviceResponse = new Response();
            client.BaseAddress = new Uri("https://fortumo.techapi24.com/");
            // Setting content type.  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Initialization.  
            HttpResponseMessage response = new HttpResponseMessage();
            response = await client.GetAsync("https://dcb.fortumo.com/5bc758e17cae03c55d3179b54fe3b486?token=" + token).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                // Reading Response.  
                string result = response.Content.ReadAsStringAsync().Result;
                // var callBackData = CallbackUrl();
                return new Response
                {
                    Status=true,
                    Message="success",
                    Data=response.RequestMessage.RequestUri,
                    StatusCode= StatusCode.Success
                };
                // apiResponse.Data = JsonConvert.DeserializeObject<Response>(result);
            }
            return new Response
            {
                Status=false,
                Message="failed",
                StatusCode=StatusCode.BadRequest,
                Data=null
            };
        }

        public async Task<Response> GetAllServicesAsync()
        {
            var response = new Response();
            try
            {
                var data = await _context.Service_Maps.ToListAsync();
                if(data !=null)
                {
                    response.Status = true;
                    response.StatusCode = StatusCode.Success;
                    response.Message = "All services ";
                    response.Data = data;
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = StatusCode.NoDataAvailable;
                    response.Message = "no data found ";
                    response.Data = data;
                }
                
            }
            catch(Exception ex)
            {
                response.Status = false;
                response.StatusCode = StatusCode.InternalServerError;
                response.Message = "Some thing went wrong please try again";
                response.Data = ex.Message;
            }
            return response;
            
            
        }

        public async Task<Payment_History> GetPaymentDataByOperationReference(string operation_Ref)
        {
            var getData = await _context.Payment_Histories.FirstOrDefaultAsync(s=>s.operation_reference==operation_Ref);
            return getData;
        }

        public async Task<Payment_Initiate> GetPaymentInitDataByOperationRef(string operation_ref)
        {
           
            var data = await _context.Payment_Initiates.SingleOrDefaultAsync(s => s.Operation_Reference == operation_ref);
            //if(data == null)
            //{
            //    var data2 = new Payment_Initiate
            //    {
            //        MSISDN = "01760000000",
            //        Operation_Reference = "test",
            //        Service_MapID = "sand-box",
            //        Channel = "sand-box"
            //    };
            //    return data2;
                
            //}
            return data;
            
        }

        public async Task<Service_Map> GetSingleAsync(string serviceId)
        {
            var data = await _context.Service_Maps.SingleOrDefaultAsync(s => s.ServiceId == serviceId);
            return data;
        }
        public async Task<Response> GetSingleServiceAsync(string serviceId)
        {
            var response = new Response();
            try
            {
                var data = await _context.Service_Maps.SingleOrDefaultAsync(s => s.ServiceId == serviceId);
                if(data !=null)
                {
                    response.Status = true;
                    response.StatusCode = StatusCode.Success;
                    response.Message = "service found";
                    response.Data = data;
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = StatusCode.NoDataAvailable;
                    response.Message = "sorry no data found";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = StatusCode.InternalServerError;
                response.Message = "Some thing went wrong please try again";
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<CustomResponse<CallBackHistory>> InsertCallBackDataAsync(CallBackHistory callBackHistory)
        {
            var response = new CustomResponse<CallBackHistory>();
            try
            {
                var insertData = await _context.CallBackHistories.AddAsync(callBackHistory);
                var isInserted = await _context.SaveChangesAsync();
                if(isInserted>0)
                {
                    response.Status = true;
                    response.StatusCode = StatusCode.Success;
                    response.Message = "data inserted";
                    response.Data = callBackHistory;
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = StatusCode.InternalServerError;
                    response.Message = "failed to insert data";
                    response.Data = null;
                }
            }
            catch(Exception ex)
            {
                response.Status = false;
                response.StatusCode = StatusCode.InternalServerError;
                response.Message = "some thing went wrong please try again";
                response.Data = null;
            }
            return response;
        }

        public async Task<Response> InsertIntoSubscriptionStatus(Subscription_Status subscription_Status)
        {
            var response = new Response();
            try
            {
                var checkNumberExist = await _context.Subscription_Statuses.FirstOrDefaultAsync(s => s.Reg_Status == true && s.MSISDN == subscription_Status.MSISDN);
                if(checkNumberExist is null || checkNumberExist ==null)
                {
                    var insertData = await _context.Subscription_Statuses.AddAsync(subscription_Status);
                    var isInserted = await _context.SaveChangesAsync();
                    if (isInserted > 0)
                    {
                        response.Status = true;
                        response.StatusCode = StatusCode.Success;
                        response.Message = "data inserted";
                        response.Data = subscription_Status.MSISDN;
                    }
                    else
                    {
                        response.Status = false;
                        response.StatusCode = StatusCode.InternalServerError;
                        response.Message = "failed to insert data";
                        response.Data = null;
                    }

                }
                if(checkNumberExist !=null )
                {
                    var checkNumber = await _context.Subscription_Statuses.FirstOrDefaultAsync(s => s.Reg_Status == true && s.MSISDN == subscription_Status.MSISDN);
                    checkNumber.Reg_Status = true;
                    checkNumber.Last_Update = DateTime.Now;
                    checkNumber.ExpiryDate = subscription_Status.ExpiryDate;

                   var updateData = _context.Update(checkNumber);
                   var isInserted =await  _context.SaveChangesAsync();
                    if (isInserted > 0)
                    {
                        response.Status = true;
                        response.StatusCode = StatusCode.Success;
                        response.Message = "data inserted";
                        response.Data = subscription_Status.MSISDN;
                    }
                    else
                    {
                        response.Status = false;
                        response.StatusCode = StatusCode.InternalServerError;
                        response.Message = "failed to insert data";
                        response.Data = null;
                    }
                }
                
                //var isInserted = await _context.SaveChangesAsync();
                //if (isInserted > 0)
                //{
                //    response.Status = true;
                //    response.StatusCode = StatusCode.Success;
                //    response.Message = "data inserted";
                //    response.Data = subscription_Status.MSISDN;
                //}
                //else
                //{
                //    response.Status = false;
                //    response.StatusCode = StatusCode.InternalServerError;
                //    response.Message = "failed to insert data";
                //    response.Data = null;
                //}
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = StatusCode.InternalServerError;
                response.Message = "some thing went wrong please try again";
                response.Data = ex.Message;
            }
            return response;
            
        }

        public async Task<CustomResponse<OneTimeCallBackData>> InsertOneTimeCallBackDataAsync(OneTimeCallBackData oneTimeCallBackData)
        {
            var response = new CustomResponse<OneTimeCallBackData>();
            try
            {
                var insertData = await _context.OneTimeCallBackDatas.AddAsync(oneTimeCallBackData);
                var isInserted = await _context.SaveChangesAsync();
                if (isInserted > 0)
                {
                    response.Status = true;
                    response.StatusCode = StatusCode.Success;
                    response.Message = "data inserted";
                    response.Data = oneTimeCallBackData;
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = StatusCode.InternalServerError;
                    response.Message = "failed to insert data";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = StatusCode.InternalServerError;
                response.Message = ex.Message;
                response.Data = null;
            }
            return response;
            
        }

        public async Task<Response> InsertPaymentStatusAsync(Payment_History payment_History)
        {
            var response = new Response();
            try
            {
                var insertData = await _context.Payment_Histories.AddAsync(payment_History);
                var isInserted = await _context.SaveChangesAsync();
                if (isInserted > 0)
                {
                    response.Status = true;
                    response.StatusCode = StatusCode.Success;
                    response.Message = "data inserted";
                    response.Data = insertData;
                }
                else
                {
                    response.Status = false;
                    response.StatusCode = StatusCode.InternalServerError;
                    response.Message = "failed to insert data";
                    response.Data = null;
                }
            }
            catch (Exception ex)
            {
                response.Status = false;
                response.StatusCode = StatusCode.InternalServerError;
                response.Message = "some thing went wrong please try again";
                response.Data = ex.Message;
            }
            return response;
        }

        public async Task<Response> InsertUnSubscripeDataIntoDb(UnSubscripeCallBackHistory unSubscripeCallBackHistory)
        {
            var response = new Response();
            try
            {
                var dataInsert = await _context.UnSubscripeCallBackHistories.AddAsync(unSubscripeCallBackHistory);
                var isInserted = await _context.SaveChangesAsync();
                if(isInserted>0)
                {
                    var updateSubscriptionStatusToFalse = await _context.Subscription_Statuses.FirstOrDefaultAsync(s => s.Operation_reference
                     == unSubscripeCallBackHistory.operation_reference && s.Subscription_uuid==unSubscripeCallBackHistory.subscription_uuid);
                    updateSubscriptionStatusToFalse.Reg_Status = false;
                    updateSubscriptionStatusToFalse.Last_Update = DateTime.Now;

                    _context.Update(updateSubscriptionStatusToFalse);
                    _context.SaveChanges();
                    response.Status = true;
                    response.Message = "success";
                    response.StatusCode = StatusCode.Success;
                    response.Data = unSubscripeCallBackHistory;
                }
                else
                {
                    response.Status = false;
                    response.Message = "failed";
                    response.StatusCode = StatusCode.BadRequest;
                    response.Data = null;
                }
            }
            catch(Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
                response.Data = null;
                response.StatusCode = StatusCode.InternalServerError;
            }
            return response;
            
        }

        public async Task<Response> PaymentInitiate(Payment_Initiate payment_Initiate)
        {
            var response = new Response();
            try
            {
                var dataInsert = await _context.Payment_Initiates.AddAsync(payment_Initiate);
                var isInserted = await _context.SaveChangesAsync();
                if(isInserted>1)
                {
                    response.Status = true;
                    response.Message = "success";
                    response.StatusCode = StatusCode.Success;
                }

            }
            catch(Exception ex)
            {
                response.Status = false;
                response.Message = ex.Message;
            }
            return response;
        }
    }
}
