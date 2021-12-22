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
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.Net.Http.Headers;
using System.Text;

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

            services.AddSignalR(options =>
            {
                options.EnableDetailedErrors = true;
            });
            //services.AddSignalR();
            //Burayi geri actim
            //services.AddCors(o => o.AddPolicy("CorsPolicy", builder =>
            //{
            //    builder
            //        .AllowAnyMethod()
            //        .AllowAnyHeader()
            //        .AllowAnyOrigin();
            //}));
            services.AddMemoryCache();
            services.AddHttpClient();
            services.AddCors(options =>
            {
                options.AddPolicy("ClientPermission", policy =>
                {
                    policy.AllowAnyHeader()
                        .AllowAnyMethod()
                        .WithOrigins("http://localhost:3000")
                        .AllowCredentials();
                });
            });

            services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(Configuration.GetConnectionString("PostgreConnection")));

            //services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
            //{
            //    //options.SignIn.RequireConfirmedAccount = true;
            //    options.Password.RequiredLength = 6; //En az ka? karakterli olmas? gerekti?ini belirtiyoruz.
            //    options.Password.RequireNonAlphanumeric = false; //Alfanumerik zorunlulu?unu kald?r?yoruz.
            //    options.Password.RequireLowercase = false; //K???k harf zorunlulu?unu kald?r?yoruz.
            //    options.Password.RequireUppercase = false; //B?y?k harf zorunlulu?unu kald?r?yoruz.
            //    options.Password.RequireDigit = false; //0-9 aras? say?sal karakter zorunlulu?unu kald?r?yoruz.
            //    //options.User.RequireUniqueEmail = true; //Email adreslerini tekille?tiriyoruz.
            //    //_.User.AllowedUserNameCharacters = "abc?defghi?jklmno?pqrs?tu?vwxyzABC?DEFGHI?JKLMNO?PQRS?TU?VWXYZ0123456789-._@+"; //Kullan?c? ad?nda ge?erli olan karakterleri belirtiyoruz.
            //    options.ClaimsIdentity.SecurityStampClaimType = "AspNet.Identity.MobileCore.SecurityStamp";
            //}).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            //services.ConfigureApplicationCookie(options =>
            //{
            //    // Cookie settings
            //    options.AccessDeniedPath = "/accessdenied";
            //    options.Cookie.Name = "mobiapicookie";
            //    options.Cookie.HttpOnly = true;
            //    options.ExpireTimeSpan = TimeSpan.FromHours(Configuration.GetValue<int>("Tokens:Lifetime"));
            //    options.LoginPath = "/login";
            //    // ReturnUrlParameter requires 
            //    //using Microsoft.AspNetCore.Authentication.Cookies;
            //    options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
            //    options.SlidingExpiration = true;
            //});

            //services.Configure<SecurityStampValidatorOptions>(options =>
            //{
            //    // enables immediate logout, after updating the user's stat.
            //    options.ValidationInterval = TimeSpan.FromSeconds(1);
            //});

            //var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Tokens:Key"]));
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddJwtBearer(config =>
            //{
            //    config.RequireHttpsMetadata = false;
            //    config.SaveToken = false; // TODO: Check later! Ara?t?r
            //    config.TokenValidationParameters = new TokenValidationParameters()
            //    {
            //        IssuerSigningKey = signingKey,
            //        ValidateAudience = true,
            //        ValidAudience = this.Configuration["Tokens:Audience"],
            //        ValidateIssuer = true,
            //        ValidIssuer = this.Configuration["Tokens:Issuer"],
            //        ValidateLifetime = true,
            //        ValidateIssuerSigningKey = true,
            //        ClockSkew = TimeSpan.Zero
            //    };
            //});

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Project API", Version = "v1" });
            //});


            services.AddIdentityCore<ApplicationUser>(options =>
            {
                options.Password.RequiredLength = 6; //En az ka? karakterli olmas? gerekti?ini belirtiyoruz.
                options.Password.RequireNonAlphanumeric = false; //Alfanumerik zorunlulu?unu kald?r?yoruz.
                options.Password.RequireLowercase = false; //K???k harf zorunlulu?unu kald?r?yoruz.
                options.Password.RequireUppercase = false; //B?y?k harf zorunlulu?unu kald?r?yoruz.
                options.Password.RequireDigit = false; //0-9 aras? say?sal karakter zorunlulu?unu kald?r?yoruz.
                options.User.AllowedUserNameCharacters = null;
                //options.User.RequireUniqueEmail = true; //Email adreslerini tekille?tiriyoruz.
                //options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789=*-._@+"; //Kullan?c? ad?nda ge?erli olan karakterleri belirtiyoruz.
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
            });

            //var key = Encoding.ASCII.GetBytes(Configuration["Tokens:Key"]);
            //services.AddAuthentication(x =>
            //{
            //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //}).AddIdentityCookies();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddScoped<IMatchRepository, MatchRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IBlobService, BlobService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<ICache, Cache>();
            services.AddScoped<ISettingRepository, SettingRepository>();
            services.AddScoped<IPremiumUserRepository, PremiumUserRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();

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
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //}

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                var endpointUrl = "/swagger/v1/swagger.json";
                options.SwaggerEndpoint(endpointUrl, "Project API");
            });

            //bunu tekrar actim
            app.UseCors("CorsPolicy");
            app.UseCors("ClientPermission");

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseMiddleware<WebSocketsMiddleware>();
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
            });
        }
    }
}
