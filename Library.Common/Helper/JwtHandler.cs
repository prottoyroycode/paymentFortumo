using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Library.Core.Helper
{

    public class JwtCustomClaims
    {
        public JwtCustomClaims()
        {
            this.urls = new Url() { redirect = "https://testmusic.shadhinmusic.com/subscription", subscription_callback = "https://fortumo.techapi24.com/api/tokentest/callback", unsubscription_redirect = "https://testmusic.shadhinmusic.com" };
            this.subscription = new Subscription() { duration=30,unit="days",price =new Price { amount=30,currency="EUR"} };
        }
        public string FirstName { get; set; }
        public string nbf { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string sub { get; set; } = "hdcb";
        public string country_code { get; set; } = "EE";
        public string channel_code { get; set; } = "sandbox-ee";
        public Url urls { get; set; } = new Url();
        public string item_description { get; set; } = "Premium Joy Subscription";
        public string operation_reference { get; set; } = "session001";
        public Subscription subscription { get; set; } = new Subscription();



    }
    public class Url
    {
        public string subscription_callback { get; set; } = "https://fortumo.techapi24.com/api/api/Tokentest/callback";
        public string redirect { get; set; } = "http://testmusic.shadhinmusic.com/subscription";
        public string unsubscription_redirect { get; set; } = "https://testmusic.shadhinmusic.com";

    }
    public class Subscription
    {
        public Price price { get; set; }
        public int duration { get; set; } = 30;
        public string unit { get; set; } = "days";
    }
    public class Price
    {
        public decimal amount { get; set; } = 30;
        public string currency { get; set; } = "EUR";
    }

    public class JwtResponse
    {
        public string Token { get; set; }
        public long ExpiresAt { get; set; }
    }

    public interface IJwtHandler
    {
        JwtResponse CreateToken(JwtCustomClaims claims);
        bool ValidateToken(string token);
        string GenerateLink(string token);
    }

    public static class TypeConverterExtension
    {
        public static byte[] ToByteArray(this string value) =>
         Convert.FromBase64String(value);
    }
    public class JwtHandler : IJwtHandler
    {
        private readonly ExternalClientJsonConfiguration _settings;
        public JwtHandler(IOptions<ExternalClientJsonConfiguration> setting)
        {
            _settings = setting.Value;
        }

        public JwtResponse CreateToken(JwtCustomClaims claims)
       {
           
            var rsaVar = @"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";
           // var exp = rsaVar.Split("\n\n");
           
           // var privateKey = exp[0].Split("\n-----")[0];

            var privateKey = _settings.RsaPrivateKey;
            var binaryEncoding = Convert.FromBase64String(string.Join(null, privateKey));

            using (RSA rsa = RSA.Create(4096))
            {
               
                rsa.ImportRSAPrivateKey(binaryEncoding, out _);
               
                

                var signingCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
                {
                    CryptoProviderFactory = new CryptoProviderFactory { CacheSignatureProviders = false }
                };



                var now = DateTime.Now;
                var unixTimeSeconds = new DateTimeOffset(now).ToUnixTimeSeconds();

                var jwt = new JwtSecurityToken(
                    audience:"Fortumo",
                    issuer: "5bc758e17cae03c55d3179b54fe3b486",
                   // notBefore: now,
                    expires: now.AddMinutes(30),
                    claims: new Claim[]{
                   
                    //new Claim(nameof(claims.FirstName), claims.FirstName),
                    //new Claim(nameof(claims.LastName), claims.LastName),
                    //new Claim(nameof(claims.Email), claims.Email),
                    new Claim(nameof(claims.nbf),unixTimeSeconds.ToString()),
                    new Claim(nameof(claims.sub), claims.sub),
                    new Claim(nameof(claims.country_code), claims.country_code),
                    new Claim(nameof(claims.channel_code), claims.channel_code),
                    //new Claim(nameof(claims.urls.subscription_callback), claims.urls.subscription_callback),
                    //new Claim(nameof(claims.urls.redirect), claims.urls.redirect),
                    //new Claim(nameof(claims.urls.unsubscription_redirect), claims.urls.unsubscription_redirect),
                    new Claim(nameof(claims.item_description), claims.item_description),
                    new Claim(nameof(claims.operation_reference), claims.operation_reference),
                    //new Claim("amount",claims.subscription.price.amount.ToString()),
                    //new Claim( "urls",claims.urls.ToString()),
                    //new Claim(nameof(claims.subscription.price.currency), claims.subscription.price.currency),
                    //new Claim("duration",claims.subscription.duration.ToString()),
                    //new Claim(nameof(claims.subscription.unit), claims.subscription.unit)
                     new Claim(JwtRegisteredClaimNames.Iat, unixTimeSeconds.ToString(), ClaimValueTypes.Integer64),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),



                    },
                    
                    
                    signingCredentials: signingCredentials
                );;; ; ;
                jwt.Payload["urls"] = new { subscription_callback = "https://fortumo.techapi24.com/api/tokentest/callback", 
                    redirect = "https://testmusic.shadhinmusic.com/subscription/payment-history", 
                    unsubscription_redirect = "https://testmusic.shadhinmusic.com/subscription"
                };
                jwt.Payload["subscription"] = new
                {
                    price = new
                    {
                        amount = claims.subscription.price.amount,
                        currency = claims.subscription.price.currency
                    },
                    duration = claims.subscription.duration,
                    unit = "days"
                };

                string token = new JwtSecurityTokenHandler().WriteToken(jwt);

                return new JwtResponse
                {
                    Token = token,
                    ExpiresAt = unixTimeSeconds,
                };
            }
        }

        public bool ValidateToken(string token)
        {
            var t = @"MIICIjANBgkqhkiG9w0BAQEFAAOCAg8AMIICCgKCAgEAzMS+G+DY5jadDQ0My/IZ
tXO0VWoLx8uW0Ba8x2fA/xXfNsib5HmFqWoezhrMprpGseaEMz01j56hxLjvJIOs
/7eATkb5ASKForv2o6xxkTzIZg76QDHC9KeKOTPYT8c7DWiPx4SeM1+WYVB3HmLS
34KYve88SNj64suByLJYyyJCPAEFzkTQejTJEAZA9xYb47T7OIIJ60ThE/cKwAzB
6QbfmRUlgDK3eIAsOALYvxMIcPR4DAH+rfq5I4UZC0/SHLdsHElXBuJ0rC/q2CXt
MpTVELqCqiDQhBP6STs0iI9BqWjN+IisDa660Rc5kT5Mbiugn5hPXi/NAtr4bRuq
rSofsH+pvBlDuYjDwBZs7G2OY7KmZIZA9q2ceRZo3pLKFHkC6EWhwQqlEn3/aKPy
WU0ylvVzrXVF13qv4LdFY7CC1QQOpRXP1g2Pii4/L9g7YZuyce7aFXe58c8jCKtm
KT29kzvjdW2KPt7dc/UK9j427dr9XDHDCeUtON4CCXL43Se1YMsweQLns6I3W+rA
ctVroHCptFiQsgT0ux4cRSl8Qxa8IJhtiI0yRCDWGPRwKttbn16irlKikfsOislS
T5VSJkACBnZ7PwHV7bUYQYlMHqC0W9FZK8OL94EQyFSmGIkLU4wkXHkVRcwMfy8y
iBSLe3mi+NgfY/ZPy2WAr4UCAwEAAQ==p ";

           // var publicKey = _settings.RsaPublicKey.ToByteArray();
            var exp = t.Split("\n\n");
            //  var privateKey = exp[1].Split("\n-----")[0];

            // var privateKey = _settings.RsaPrivateKey;
            var binaryEncoding = Convert.FromBase64String(string.Join(null, exp));
            using (RSA rsa = RSA.Create())
            {


                rsa.ImportRSAPublicKey(binaryEncoding, out _);

                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = new RsaSecurityKey(rsa),

                    CryptoProviderFactory = new CryptoProviderFactory()
                    {
                        CacheSignatureProviders = false
                    }
                };

                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    handler.ValidateToken(token, validationParameters, out var validatedSecurityToken);
                }
                catch
                {
                    return false;
                }

                return true;
            }
        }

        public string GenerateLink(string token) =>
             $"{_settings.ReferralUrl}/{_settings.ReferralId}/foo?token={token}";

    
}
}
