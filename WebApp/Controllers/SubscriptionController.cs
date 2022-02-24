using Library.Core.Helper;
using Library.Core.Models.CallBackModel;
using Library.Core.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Models.DataContext;
using Models.Entities;
using Newtonsoft.Json;
using Services.CustomRepository.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ViewModels;

namespace WebApp.Controllers
{
    public class SubscriptionController:BaseApiController
    {
        private readonly IJwtHandler _jwtHandler;
        private readonly IJwtHandlerOneTime _jwtHandlerOneTime;
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger _logger;
        private readonly EfDbContext _context;
        private readonly IFortumoSubscriptionService _fortumoSubscriptionService;

        public SubscriptionController(IJwtHandler jwtHandler, ILogger<TokentestController> logger, 
            IFortumoSubscriptionService fortumoSubscriptionService ,EfDbContext context , IJwtHandlerOneTime jwtHandlerOneTime)
        {
            _jwtHandler = jwtHandler;
            _jwtHandlerOneTime= jwtHandlerOneTime;
            _logger = logger;
            _fortumoSubscriptionService = fortumoSubscriptionService;
            _context = context;
        }
        [HttpPost("unsubscribe")]
        public async Task<IActionResult>UnSubscripe([FromBody] SubscriptionVM subscriptionVM)
        {

            var response = new Response();
            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.StatusCode = Library.Core.ViewModels.StatusCode.BadRequest;
                response.Message = string.Join(" and ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)); ;
                response.Data = null;
                return Ok(response);
                // return BadRequest(ModelState);
            }
            //var service_MapId = await _fortumoSubscriptionService.GetSingleAsync(subscriptionVM.Service_MapId);
            //if (service_MapId == null)
            //    return BadRequest();
            var checkServiceStatus = await _fortumoSubscriptionService.CheckIfAlreadySubscriped(subscriptionVM);
            if (checkServiceStatus)
            {
                var linkToUnSubscripe = await _context.Subscription_Statuses.FirstOrDefaultAsync(s => s.Reg_Status == true && s.MSISDN == subscriptionVM.Mobile);
                if(linkToUnSubscripe!=null)
                {
                    var getUnsubUrl = await _context.CallBackHistories.FirstOrDefaultAsync(p => p.subscription_uuid == linkToUnSubscripe.Subscription_uuid &&
                    p.operation_reference==linkToUnSubscripe.Operation_reference);
                    if(getUnsubUrl.unsubscribe_url !=null)
                    {
                        response.Data = getUnsubUrl.unsubscribe_url;
                        response.Message = "click this link to unsubscribe";
                        response.Status = true;
                        response.StatusCode = Library.Core.ViewModels.StatusCode.Success;
                    }
                }
            }
            else
            {
                response.Data = new { 
                    error = "This number is not subscribed "   ,
                    description ="subscription is needed "
                };
                response.Status = false;
                response.Message = "you are not subscribed ";
                response.StatusCode = Library.Core.ViewModels.StatusCode.BadRequest;
            }
            
            return Ok(response);
        }
        [AllowAnonymous]
        [HttpPost("callback")]
        public async Task<IActionResult> CallbackUrl([FromBody] SuccessfulCallBack successfulCallBack)
        {
            
            var cardResultData = Newtonsoft.Json.JsonConvert.SerializeObject(successfulCallBack).ToString();
            var mapCallback = new CallBackHistory();
            mapCallback.subscription_uuid = successfulCallBack.subscription_uuid;
            mapCallback.charging_token = successfulCallBack.charging_token;
            mapCallback.merchant = successfulCallBack.merchant;
            mapCallback.operation_reference = successfulCallBack.operation_reference;
            mapCallback.subscription_status = successfulCallBack.subscription_status;
            mapCallback.billing_status = successfulCallBack.billing_status;
            mapCallback.service_starts_at = mapCallback.service_starts_at;
            mapCallback.service_ends_at = successfulCallBack.service_ends_at;
            mapCallback.consumer_identity = successfulCallBack.consumer_identity;
            mapCallback.action = successfulCallBack.action;
            mapCallback.unsubscribe_url = successfulCallBack.unsubscribe_url;
            mapCallback.payment_transaction_id = successfulCallBack.payment_transaction_id;
            mapCallback.timestamp = successfulCallBack.timestamp;
            mapCallback.token = successfulCallBack.metadata.token;
            mapCallback.uuid = successfulCallBack.metadata.uuid;
            mapCallback.amount = successfulCallBack.price.amount;
            mapCallback.currency = successfulCallBack.price.currency;
            mapCallback.allDataInJson = cardResultData;
            mapCallback.CreatedDate = DateTime.Now;
            var insertCallBackData = await _fortumoSubscriptionService.InsertCallBackDataAsync(mapCallback);
            _logger.LogInformation("callBack-starts");
            _logger.LogInformation(successfulCallBack.ToString());
            _logger.LogInformation(cardResultData);


            return Ok();
        }
        [HttpGet("all-services")]
        public async Task<IActionResult>GetAllServices()
        {
            var result = await _fortumoSubscriptionService.GetAllServicesAsync();
            return Ok(result);
        }
        [HttpPost("availablePackage")]
        public async Task<IActionResult> CheckAvailablePackage(SubscriptionVM subscriptionVM)
        {
            var data = await _fortumoSubscriptionService.CheckPackageStatus(subscriptionVM);
            return Ok(data);
        }
        [HttpGet("single-service/{serviceId}")]
        public async Task<IActionResult> GetSingleService(string serviceId)
        {
              
            var result = await _fortumoSubscriptionService.GetSingleServiceAsync(serviceId);
            return Ok(result);
        }
        [HttpPost("subscription")]
        public async Task<IActionResult>SubscriptionApiCall([FromBody]SubscriptionVM subscriptionVM)
        {
            var response = new Response();
            if (!ModelState.IsValid)
            {
                response.Status = false;
                response.StatusCode = Library.Core.ViewModels.StatusCode.BadRequest;
                response.Message = string.Join(" and ", ModelState.Values
                                            .SelectMany(v => v.Errors)
                                            .Select(e => e.ErrorMessage)); ;
                response.Data = null;
                return Ok(response);
                // return BadRequest(ModelState);
            }
            var service_MapId = await _fortumoSubscriptionService.GetSingleAsync(subscriptionVM.Service_MapId);        
            if (service_MapId ==null)
                return BadRequest();
            var checkServiceStatus = await _fortumoSubscriptionService.CheckIfAlreadySubscriped(subscriptionVM);
            if (checkServiceStatus)
                return Ok(new Response
                { 
                   Status=false,
                   StatusCode=Library.Core.ViewModels.StatusCode.BadRequest,
                   Message="already subscribed",
                   Data=null
                   


                });
            

            var operation_reference = Guid.NewGuid();
            var or = Guid.Parse(operation_reference.ToString());

            _logger.LogInformation("subscriptio api- call-starts");
            #region claims for one time 
            var claimsForOneTime = new JwtCustomClaimsOneTime();
            if (service_MapId.DurationInDays == 365)
            {
                 claimsForOneTime = new JwtCustomClaimsOneTime
                {
                    sub = "hdcb",
                    country_code = service_MapId.Country_Code,
                    channel_code = service_MapId.Channel_Code,
                    urls =
                {
                    payment_callback ="https://fortumo.techapi24.com/api/tokentest/onetime-callBack",
                    redirect ="https://testmusic.shadhinmusic.com/subscription"
                },
                    item_description = "Premium Shadhin Subscription",
                    operation_reference = operation_reference.ToString(),
                    price =
                {
                    amount =service_MapId.PriceOrAmount,
                    currency =service_MapId.Currency
                }
                };
            }
           
            #endregion
            #region claims
            var claims = new JwtCustomClaims();
            if (service_MapId.DurationInDays == 30)
            {
                
                claims = new JwtCustomClaims
                {


                    sub = "hdcb",
                    country_code = service_MapId.Country_Code,
                    channel_code = service_MapId.Channel_Code,
                    //channel_code = "sandbox-ee",
                    urls =
                {

                    subscription_callback = "https://fortumo.techapi24.com/api/tokentest/callback",
                    redirect = "https://testmusic.shadhinmusic.com/subscription",
                    unsubscription_redirect = "https://testmusic.shadhinmusic.com",
                },
                    item_description = "Premium Shadhin Subscription",
                    operation_reference = operation_reference.ToString(),
                    // operation_reference = Guid.NewGuid().ToString(),
                    subscription =
                {
                    price =
                    {
                        amount =service_MapId.PriceOrAmount,
                        currency=service_MapId.Currency
                    },
                    duration=Convert.ToInt32(service_MapId.DurationInDays),
                    unit ="days"
                }
                };
            }
            

            #endregion
            var jwt = new JwtResponse();
            #region token service call
            if (service_MapId.DurationInDays == 365)
            {
                jwt = _jwtHandlerOneTime.CreateToken(claimsForOneTime);
                
            }
            if (service_MapId.DurationInDays == 30)
            {
                jwt = _jwtHandler.CreateToken(claims);
            }
           
            
            #endregion
            #region paymentinit
            var tokenValue = jwt.Token;
            var paymentInitObj = new Payment_Initiate();
            paymentInitObj.MSISDN = subscriptionVM.Mobile;
            paymentInitObj.Operation_Reference = operation_reference.ToString();
            paymentInitObj.Service_MapID = service_MapId.ServiceId;
            paymentInitObj.CreatedOn = DateTime.Now;
            paymentInitObj.Channel = subscriptionVM.Channel;
            paymentInitObj.tokenValue = tokenValue;
            var paymentInitiate = await _fortumoSubscriptionService.PaymentInitiate(paymentInitObj);
            if(paymentInitiate.Status==true)
            {
                #region httpcall
                var callHttpApi = await _fortumoSubscriptionService.CreateSubscriptionAsync(tokenValue);
                
                return Ok(callHttpApi);
                #endregion
            }

            #endregion


            return Ok(new 
            { 
               status =false ,
               message ="some thing went wrong ",
               statusCode =400
            });;;
        }
    }
}
