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
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApp.Controllers
{

    public class TokentestController : BaseApiController
    {
        private readonly EfDbContext _context;
        private readonly IJwtHandler _jwtHandler;
        private static readonly HttpClient client = new HttpClient();
        private readonly ILogger _logger;
        private readonly IFortumoSubscriptionService _fortumoSubscriptionService;
        public TokentestController(IJwtHandler jwtHandler, ILogger<TokentestController> logger,
            IFortumoSubscriptionService fortumoSubscriptionService, EfDbContext context)
        {
            _jwtHandler = jwtHandler;
            _logger = logger;
            _fortumoSubscriptionService = fortumoSubscriptionService;
            _context = context;
        }
        [HttpGet("sub-call")]
        public async Task<IActionResult> CallSubscriptioApi()
        {

            Random random = new Random();
            var value = random.Next(100001, 999999);
            _logger.LogInformation("subapi call-starts");
            #region custom data create
            var claims = new JwtCustomClaims
            {


                sub = "hdcb",
                country_code = "EE",
                channel_code = "sandbox-ee",
                urls =
                {

                    subscription_callback ="https://fortumo.techapi24.com/api/tokentest/callback",
                    redirect="https://fortumo.techapi24.com/api/Tokentest/redirect",
                    unsubscription_redirect="https://fortumo.techapi24.com/api/Tokentest/unsubscripe",
                },
                item_description = "Premium Joy Subscription",
                operation_reference = value.ToString(),
                subscription =
                {
                    price =
                    {
                        amount =3,
                        currency="EUR"
                    },
                    duration=30,
                    unit="days"
                }
            };
            #endregion

            #region token service call
            var jwt = _jwtHandler.CreateToken(claims);
            var tokenValue = jwt.Token;
            #endregion
            //initializing obj
            Response apiResponse = new Response();
            #region fortumo api call
            client.BaseAddress = new Uri("https://fortumo.techapi24.com/");
            // Setting content type.  
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenValue);

            // Initialization.  
            HttpResponseMessage response = new HttpResponseMessage();
            response = await client.GetAsync("https://dcb.fortumo.com/5bc758e17cae03c55d3179b54fe3b486?token=" + tokenValue).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                // Reading Response.  
                string result = response.Content.ReadAsStringAsync().Result;
                // var callBackData = CallbackUrl();
                return Ok(response);
                // apiResponse.Data = JsonConvert.DeserializeObject<Response>(result);
            }
            return Ok(new { status = "error", message = "failed" });
            #endregion
        }
        [HttpPost("token")]
        public IActionResult GenerateTokenAsync()
        {
            var claims = new JwtCustomClaims
            {

                //FirstName = "Sabirul",
                //LastName = "Haque",
                //Email = "content.gakk@gmail.com",
                sub = "hdcb",
                country_code = "EE",
                channel_code = "sandbox-ee",
                urls =
                {

                    subscription_callback ="https://fortumo.techapi24.com/api/tokentest/callback",
                    redirect="https://fortumo.techapi24.com/api/Tokentest/redirect",
                    unsubscription_redirect="https://fortumo.techapi24.com/api/Tokentest/unsubscripe",
                },
                item_description = "Premium Joy Subscription",
                operation_reference = "session001",
                subscription =
                {
                    price =
                    {
                        amount =30,
                        currency="EUR"
                    },
                    duration=30,
                    unit="days"
                }
            };

            var jwt = _jwtHandler.CreateToken(claims);

            return Ok(jwt);
        }
        [AllowAnonymous]
        [HttpPost("onetime-callBack")]
        public async Task<IActionResult> OneTimeCallbackURL([FromBody] OneTimePaymentCallBack oneTimePaymentCallBack)
        {
            var getMSISDN = await _fortumoSubscriptionService.GetPaymentInitDataByOperationRef(oneTimePaymentCallBack.operation_reference);
            var cardResultData = JsonConvert.SerializeObject(oneTimePaymentCallBack).ToString();
            var mapCallBack = new OneTimeCallBackData
            {
                transaction_id = oneTimePaymentCallBack.transaction_id,
                transaction_state = oneTimePaymentCallBack.transaction_state,
                merchant = oneTimePaymentCallBack.merchant,
                operation_reference = oneTimePaymentCallBack.operation_reference,
                consumer_identity = oneTimePaymentCallBack.consumer_identity,
                timestamp = oneTimePaymentCallBack.timestamp,
                amount = oneTimePaymentCallBack.price.amount,
                currency = oneTimePaymentCallBack.price.currency,
                AllData = cardResultData,
                MSISDN = getMSISDN.MSISDN,
                CreateDate = DateTime.Now
            };

            var insertCallBackData = await _fortumoSubscriptionService.InsertOneTimeCallBackDataAsync(mapCallBack);
            var subs_status = new Subscription_Status
            {

                MSISDN = getMSISDN.MSISDN,
                ServiceMap_Id = getMSISDN.Service_MapID,
                Reg_Status = true,
                Reg_Date = DateTime.Now,
                ExpiryDate = DateTime.Now.AddYears(1),
                Operation_reference = oneTimePaymentCallBack.operation_reference,
                Subscription_uuid = oneTimePaymentCallBack.transaction_id
            };
            if (oneTimePaymentCallBack.transaction_state == "failed")
            {
                subs_status.Reg_Status = false;
                subs_status.ExpiryDate = DateTime.Now;
            }

            var insertIntoSubscription_status = await _fortumoSubscriptionService.InsertIntoSubscriptionStatus(subs_status);



            return Ok();
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
            mapCallback.service_starts_at = successfulCallBack.service_starts_at;
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

            var insertCallBackData = await _fortumoSubscriptionService.InsertCallBackDataAsync(mapCallback);
            var getMSISDN = await _fortumoSubscriptionService.GetPaymentInitDataByOperationRef(successfulCallBack.operation_reference);
            if (getMSISDN == null)
            {
                return Ok();
            }
            var mapPaymentHistory = new Payment_History
            {
                operation_reference = mapCallback.operation_reference,
                subscription_status = mapCallback.subscription_status,
                billing_status = mapCallback.billing_status,
                service_ends_at = mapCallback.service_ends_at,
                service_starts_at = mapCallback.service_starts_at,
                action = mapCallback.action,
                timestamp = mapCallback.timestamp,

                amount = mapCallback.amount,
                currency = mapCallback.currency,
                MSISDN = getMSISDN.MSISDN


            };
            var subs_status = new Subscription_Status
            {

                MSISDN = getMSISDN.MSISDN,
                ServiceMap_Id = getMSISDN.Service_MapID,
                Reg_Status = true,
                Reg_Date = DateTime.Now,
                ExpiryDate = successfulCallBack.service_ends_at,
                Operation_reference = successfulCallBack.operation_reference,
                Subscription_uuid = successfulCallBack.subscription_uuid

            };
            if (successfulCallBack.subscription_status == "failed" || successfulCallBack.subscription_status == "cancelled"
                || successfulCallBack.subscription_status == "suspended"
                )
            {
                subs_status.Reg_Status = false;
            }
            if (insertCallBackData.Status == true || insertCallBackData.Status == false)
            {

                if (insertCallBackData.Data.action == "activation")
                {
                    var insertPaymentHistory = await _fortumoSubscriptionService.InsertPaymentStatusAsync(mapPaymentHistory);
                    if (insertPaymentHistory.Status == true && insertPaymentHistory.Data != null)
                    {
                        var insertIntoSubscription_status = await _fortumoSubscriptionService.InsertIntoSubscriptionStatus(subs_status);
                    }
                    _logger.LogInformation(insertCallBackData.Data.action.ToString());
                }
                //  if (insertCallBackData.Data.action == "cancellation" && insertCallBackData.Data.subscription_status == "cancelled")
                if (insertCallBackData.Data.action == "cancellation" || insertCallBackData.Data.action == "rebill" || insertCallBackData.Data.action == "suspension")
                {
                    var getDataByOperation_ref = await _fortumoSubscriptionService.GetPaymentDataByOperationReference(successfulCallBack.operation_reference);
                    getDataByOperation_ref.subscription_status = "cancelled/unsubscriped";
                    getDataByOperation_ref.timestamp = DateTime.Now;
                    var updateData = _context.Update(getDataByOperation_ref);
                    var isUpdated = await _context.SaveChangesAsync();
                    if (isUpdated > 0)
                    {
                        var subscriptionStatusDataToUpdate = await _context.Subscription_Statuses.FirstOrDefaultAsync(p => p.Operation_reference
                        == getDataByOperation_ref.operation_reference);
                        subscriptionStatusDataToUpdate.Reg_Status = false;
                        subscriptionStatusDataToUpdate.Last_Update = DateTime.Now;
                        var updataData = _context.Update(subscriptionStatusDataToUpdate);
                        await _context.SaveChangesAsync();
                    }

                    //update paymentHistory by operation ref or subuuid
                    //then update subscription status by operation reference and susuuid


                }


            }
            _logger.LogInformation("callBack-starts");
            _logger.LogInformation(mapCallback.ToString());
            _logger.LogInformation(cardResultData);


            return Ok();
        }

        [HttpPost("redirect")]
        public IActionResult Redirect([FromBody] object obj)
        {
            _logger.LogInformation("redirect-starts");
            return Ok(new { status = "success", message = "successfull" });
        }

        [HttpPost("unsubscripe")]
        public async Task<IActionResult> Unsubscripe([FromBody] UnSubscribeCallBackData unSubscribeCallBackData)
        {
            var cardResultData = Newtonsoft.Json.JsonConvert.SerializeObject(unSubscribeCallBackData).ToString();
            var mapDataToDb = new UnSubscripeCallBackHistory()
            {
                subscription_uuid = unSubscribeCallBackData.subscription_uuid,
                charging_token = unSubscribeCallBackData.charging_token,
                merchant = unSubscribeCallBackData.merchant,
                operation_reference = unSubscribeCallBackData.operation_reference,
                subscription_status = unSubscribeCallBackData.subscription_status,
                billing_status = unSubscribeCallBackData.billing_status,
                service_starts_at = unSubscribeCallBackData.service_starts_at,
                service_ends_at = unSubscribeCallBackData.service_ends_at,
                consumer_identity = unSubscribeCallBackData.consumer_identity,
                timestamp = unSubscribeCallBackData.timestamp,
                amount = unSubscribeCallBackData.price.amount,
                currency = unSubscribeCallBackData.price.currency,
                action = unSubscribeCallBackData.action,
                unsubscribe_url = unSubscribeCallBackData.unsubscribe_url,
                allDataInJson = cardResultData
            };
            var storeDataIntoDb = await _fortumoSubscriptionService.InsertUnSubscripeDataIntoDb(mapDataToDb);
            _logger.LogInformation("redirect-starts");
            _logger.LogInformation(unSubscribeCallBackData.ToString());
            return Ok(storeDataIntoDb);
        }



    }
}
