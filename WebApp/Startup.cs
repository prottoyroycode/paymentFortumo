using Library.Core.Helper;
using Library.Core.IoC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Models.DataContext;
using Services.CustomRepository.Implementations;
using Services.CustomRepository.Interface;
using System;
using System.Security.Cryptography;
using System.Text;

namespace WebApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region CORS policy
            services.AddCors();
            services.AddControllers();
            //services.AddCors(options =>
            //{
            //    // default policy
            //    options.AddDefaultPolicy(builder => builder
            //                                .AllowAnyOrigin()
            //                                .AllowAnyMethod()
            //                                .AllowAnyHeader()
            //                            );
            //    // named policy
            //    options.AddPolicy("CorsPolicy", builder => builder.WithOrigins("http://localhost").AllowAnyHeader().AllowAnyMethod());
            //});

            //prottoy
            

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Fortumo",
                    Version = "v1",
                    Description = "An API to perform Fortumo Operations",
                    Contact = new OpenApiContact
                    {
                        Name = "prottoy roy",
                        Email = "prottoy@gakkmediabd.com"
                    }

                });
                //var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                //var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                //c.IncludeXmlComments(xmlPath);

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
             {
               new OpenApiSecurityScheme
               {
                 Reference = new OpenApiReference
                 {
                   Type = ReferenceType.SecurityScheme,
                   Id = "Bearer"
                 }
                },
                new string[] { }
              }
  });

                // var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                // var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // c.IncludeXmlComments(xmlPath);


            });
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(o =>
            {
                
                var rsaVar = @"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";
                var binaryEncoding = Convert.FromBase64String(string.Join(null, rsaVar));
                var rsa = RSA.Create();
                
                    rsa.ImportRSAPrivateKey(binaryEncoding, out _);
               
                

                o.RequireHttpsMetadata = false;
                o.SaveToken = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero,
                    // ValidIssuer = false,
                    //ValidAudience = false,

                    // ValidIssuer = _config["TokenKey"],
                    // ValidAudience = _config["TokenKey"],
                    IssuerSigningKey = new RsaSecurityKey(rsa)
                };
            });

            services.Configure<ExternalClientJsonConfiguration>(Configuration.GetSection("ExternalClientServer"));
            services.AddTransient<IJwtHandler, JwtHandler>();
            services.AddTransient<IJwtHandlerOneTime, JwtHandlerOneTime>();
            services.AddTransient<IFortumoSubscriptionService, FortumoSubscriptionService>();
            //prottoy

            #endregion

            services.AddControllersWithViews().AddNewtonsoftJson();

            services.AddMvc();

            #region DbContext

            services.AddDbContext<EfDbContext>(opts => opts.UseSqlServer(Configuration.GetConnectionString("DefaultDBContextConnection"), options => options.MigrationsAssembly("WebApp")));

            #endregion

            #region data protection service
        
            services.Configure<DataProtectionTokenProviderOptions>(opt => opt.TokenLifespan = TimeSpan.FromMinutes(2));

            #endregion

            // TODO : New Add
            //commenting
            // services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            // RegisterJwtBearer(services);
            //commenting
            services.Configure<ApiBehaviorOptions>(opt =>
            {
                opt.SuppressModelStateInvalidFilter = true;
            });

            #region Application Service Dependency
           
            //services.AddScoped<IUserService, UserService>();

            #endregion
        }
        //commenting
        //private void RegisterJwtBearer(IServiceCollection services)
        //{
        //    DependencyContainer.RegisterJwtBearer(services);
        //}
        //commenting
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
           
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();
            
            else app.UseExceptionHandler("/Home/Error");
            loggerFactory.AddFile("wwwroot/Logs/mylog-{Date}.txt");
            app.UseSwagger();
            app.UseSwaggerUI(d => d.SwaggerEndpoint("/swagger/v1/swagger.json", "ECardApi v1"));

            app.UseStaticFiles();
            app.UseHttpsRedirection(); 
            app.UseRouting();
            // global cors policy
            app.UseCors(x => x
                .AllowAnyMethod()
                .AllowAnyHeader()
                .SetIsOriginAllowed(origin => true) // allow any origin
                .AllowCredentials()); // allow credentials
            //app.UseCors();
            //app.UseCors("CorsPolicy");

            // Linux with Nginx
            //app.UseForwardedHeaders(new ForwardedHeadersOptions
            //{
            //    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            //});

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
