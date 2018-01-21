using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System;
using System.IO;
using System.Text;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.ResponseCompression;
using Newtonsoft;
using FormeraMeasure.Options;
using FormeraMeasure.Models;

namespace FormeraMeasure
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }
        private SymmetricSecurityKey _signingKey;

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add framework services.
            services.AddOptions();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
            });

            var mapconfig = new AutoMapper.MapperConfiguration(cfg =>
            {
                cfg.AddProfile(new AutoMapperProfileConfiguration());
            });

            var mapper = mapconfig.CreateMapper();
            services.AddSingleton(mapper);

            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });

            // Make authentication compulsory across the board (i.e. shut
            // down EVERYTHING unless explicitly opened up).
            services.AddMvc(config =>
            {
                var policy = new AuthorizationPolicyBuilder()
                           .RequireAuthenticatedUser()
                           .Build();
                config.Filters.Add(new AuthorizeFilter(policy));
            });

            services.Configure<GzipCompressionProviderOptions>(options => options.Level = System.IO.Compression.CompressionLevel.Optimal);
            services.AddResponseCompression();

            // Use policy auth.
            services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly",
                            policy => policy.RequireClaim("UserType", "Admin"));
                options.AddPolicy("PostData",
                            policy => policy.RequireClaim("type", "device"));
            });

            //environment
            //var secret = Configuration["JWT_SIGNING_SECRET"];
            var secret = "aaaaaaaaaaaaxxxxxxxxxxxxxxxxxxxxx";
            _signingKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));

            // Get options from app settings
            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));

            // Configure JwtIssuerOptions


            services.Configure<JwtIssuerOptions>(options =>
            {
                options.ValidFor = TimeSpan.FromHours(24);
                options.Issuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)];
                options.Audience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)];
                options.SigningCredentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);
            });

            services.Configure<MongoSettings>(options =>
            {
                options.ConnectionString = Configuration.GetSection("MongoConnection:ConnectionString").Value;
                options.Database = Configuration.GetSection("MongoConnection:Database").Value;
            });

            services.Configure<SlackSettings>(options =>
            {
                options.WebHookUrl = Configuration.GetSection("SlackSettings:WebHookUrl").Value;
            });

            services.Configure<MailGunSettings>(options =>
            {
                options.Domain = Configuration.GetSection("MailGunSettings:Domain").Value;
                options.API_Key = Configuration.GetSection("MailGunSettings:API_Key").Value;
            });

            // Get options from app settings
            services.Configure<PasswordSettings>(options =>
            {
                options.HashLength = int.Parse(Configuration.GetSection("PasswordSettings:HashLength").Value);
                options.Iterations = int.Parse(Configuration.GetSection("PasswordSettings:Iterations").Value);
                options.SaltLength = int.Parse(Configuration.GetSection("PasswordSettings:SaltLength").Value);
            });

            services.AddScoped(typeof(MongoContext));
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IClientRepository, ClientRepository>();
            services.AddTransient<IDeviceRepository, DeviceRepository>();
            services.AddTransient<IAlertRepository, AlertRepository>();
            services.AddTransient<IDataRepository, DataRepository>();
            services.AddTransient<IMailService, MailGunService>();
            services.AddTransient<ISlackService, SlackNotificationService>();
            services.AddTransient<IEventProcessorService, AlertProcessorService>();
            //services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            var jwtAppSettingOptions = Configuration.GetSection(nameof(JwtIssuerOptions));
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = jwtAppSettingOptions[nameof(JwtIssuerOptions.Issuer)],

                ValidateAudience = true,
                ValidAudience = jwtAppSettingOptions[nameof(JwtIssuerOptions.Audience)],

                ValidateIssuerSigningKey = true,
                IssuerSigningKey = _signingKey,

                RequireExpirationTime = true,
                ValidateLifetime = true,
                NameClaimType = "sub",
                ClockSkew = TimeSpan.Zero
            };

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationParameters
            });

            app.UseCors("CorsPolicy");

            //middleware compression is 28% slower than IIS compression (added by Azure)
            //https://www.softfluent.com/blog/dev/2017/01/13/Enabling-gzip-compression-with-ASP-NET-Core
            //app.UseResponseCompression();

            app.Use(async (context, next) =>
            {
                await next();

                if (context.Response.StatusCode == 404 &&
                    !Path.HasExtension(context.Request.Path.Value) &&
                    !context.Request.Path.Value.StartsWith("/api/"))
                {
                    context.Request.Path = "/index.html";
                    await next();
                }
            });
            /*
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = 500; // or another Status accordingly to Exception Type
                    context.Response.ContentType = "application/json";

                    var error = context.Features.Get<IExceptionHandlerFeature>();
                    if (error != null)
                    {
                        var ex = error.Error;

                        await context.Response.WriteAsync(new Error()
                        {
                            Code = < your custom code based on Exception Type >,
                            Message = ex.Message // or your custom message
                            // other custom data
                        }.ToString(), Encoding.UTF8);
                    }
                });
            });
            */
            app.UseMvc();
            app.UseStaticFiles();
        }
    }
}