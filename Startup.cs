using Azure.Storage.Blobs;
using DatingWeb.CacheService;
using DatingWeb.CacheService.Interface;
using DatingWeb.Data;
using DatingWeb.Data.DbModel;
using DatingWeb.Helper;
using DatingWeb.Hubs;
using DatingWeb.Repository.Auth;
using DatingWeb.Repository.Auth.Interface;
using DatingWeb.Repository.Chat;
using DatingWeb.Repository.Chat.Interface;
using DatingWeb.Repository.Matches;
using DatingWeb.Repository.Matches.Interface;
using DatingWeb.Repository.PremiumUsers;
using DatingWeb.Repository.PremiumUsers.Interface;
using DatingWeb.Repository.Report;
using DatingWeb.Repository.Report.Interface;
using DatingWeb.Repository.Settings;
using DatingWeb.Repository.Settings.Interface;
using DatingWeb.Repository.User;
using DatingWeb.Repository.User.Interface;
using DatingWeb.Services;
using DatingWeb.Services.Interfaces;
using DatingWeb.Settings;
using DatingWeb.SignalrHelper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.StackExchangeRedis;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Http.Headers;
using System.Text;
using Microsoft.AspNetCore.SignalR;
using System.Net;
using System.Threading.Tasks;
using DatingWeb.Repository.Story;
using DatingWeb.Repository.Story.Interface;
using DatingWeb.Repository.Location.Interface;
using DatingWeb.Repository.Location;
using DatingWeb.Repository.Privacy.Interface;
using DatingWeb.Repository.Privacy;

namespace DatingWeb
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
            System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();


 
            services.AddMemoryCache();
            
            // Redis Configuration
            var redisSettings = Configuration.GetSection("Redis").Get<RedisSettings>();
            services.Configure<RedisSettings>(Configuration.GetSection("Redis"));
            
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = redisSettings.ConnectionString;
                options.InstanceName = "MidpotApp";
            });
            
            services.AddHttpClient();

            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000", "http://localhost:3001")
                        .AllowCredentials();
                });
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreConnection")));

            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 6; //En az ka? karakterli olmas? gerekti?ini belirtiyoruz.
                options.Password.RequireNonAlphanumeric = false; //Alfanumerik zorunlulu?unu kald?r?yoruz.
                options.Password.RequireLowercase = false; //K???k harf zorunlulu?unu kald?r?yoruz.
                options.Password.RequireUppercase = false; //B?y?k harf zorunlulu?unu kald?r?yoruz.
                options.Password.RequireDigit = false; //0-9 aras? say?sal karakter zorunlulu?unu kald?r?yoruz.
                options.User.AllowedUserNameCharacters = null;
            }).AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<ApplicationUser>>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddSignInManager<SignInManager<ApplicationUser>>()
                    .AddDefaultTokenProviders();

            // configure jwt authentication

            services.Configure<DataProtectionTokenProviderOptions>(options => options.TokenLifespan = TimeSpan.FromDays(Configuration.GetValue<int>("Tokens:Lifetime")));

            var key = Encoding.ASCII.GetBytes(Configuration["Tokens:Key"]);

            services.AddAuthorization(auth =>
            {
                auth.AddPolicy("Bearer", new AuthorizationPolicyBuilder()
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build());
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,

                    ValidAudience = this.Configuration["Tokens:Audience"],
                    ValidIssuer = this.Configuration["Tokens:Issuer"],
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Query["access_token"];

                        var path = context.HttpContext.Request.Path;
                        if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                        {
                            context.Token = accessToken;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddSignalR();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<ICache, Cache>();
            services.AddScoped<IRedisCache, DatingWeb.CacheService.RedisCache>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IPremiumUserRepository, PremiumUserRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<IStoryRepository, StoryRepository>();
            services.AddScoped<ILocationRepository, LocationRepository>();
            services.AddScoped<IPrivacyRepository, PrivacyRepository>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Midpot API", Version = "v1" });
                var securityScheme = new OpenApiSecurityScheme()
                {
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                };
                c.AddSecurityDefinition("Bearer", securityScheme);

                c.AddSecurityRequirement(new OpenApiSecurityRequirement { { new OpenApiSecurityScheme { Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" } }, new string[] { } } });
            });

            services.AddScoped(x => new BlobServiceClient(Configuration.GetValue<string>("AzureBlobStorage")));

            services.AddHttpClient("notificationService", (client) =>
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("key", "=" + Configuration.GetValue<string>("FCM:ServerKey"));
                client.BaseAddress = new Uri("https://fcm.googleapis.com/fcm/send");
            });
            var smsServiceSettings = Configuration.GetSection("SMS").Get<SmsServiceSettings>();
            services.AddSingleton<SmsServiceSettings>(smsServiceSettings);
            services.AddHttpClient("smsService", (client) =>
            {
                client.BaseAddress = new Uri(smsServiceSettings.SmsServiceUrl);
            });

            services.AddControllers();

            // Add Health Checks
            services.AddHealthChecks();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var endpointUrl = "/swagger/v1/swagger.json";
                options.SwaggerEndpoint(endpointUrl, "Project API");
            });

            // app.UseCors("CorsPolicy"); // Tanımlı olmayan CORS policy çağrısını kaldır
            app.UseCors("ClientPermission");

            //app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseMiddleware<WebSocketsMiddleware>();
            app.UseMiddleware<ResponseWrapper>();
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseAuthentication();
            app.UseAuthorization();


            app.UseWebSockets();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatHub>("/chat", options =>
                {
                    options.Transports = HttpTransportType.WebSockets;
                });
                
                // Health Check Endpoint
                endpoints.MapHealthChecks("/health").RequireHost("*:*");
            });
            

        }
    }
}
