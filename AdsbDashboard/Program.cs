using AdsbDashboard.Components;
using AdsbDashboard.Components.Account;
using AdsbDashboard.Data;
using AdsbDashboard.Models;
using AdsbDashboard.Service;
using AdsbDashboard.Utility;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using MudBlazor.Services;

namespace AdsbDashboard
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var loggerFactory = LoggerFactory.Create(b => b
                .AddConsole()
                .AddDebug()
                .SetMinimumLevel(LogLevel.Debug));
            var logger = loggerFactory.CreateLogger<Program>();

            builder.Logging.AddConsole();

            string _ENVIRONMENT = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? builder.Configuration["ENVIRONMENT"] ?? "Development";
            bool isDevelopment = _ENVIRONMENT == "Development";


            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            // --- setup auth 

            builder.Services.AddCascadingAuthenticationState();
            builder.Services.AddScoped<IdentityUserAccessor>();
            builder.Services.AddScoped<IdentityRedirectManager>();
            builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultScheme = IdentityConstants.ApplicationScheme;
                    options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
                })
                .AddIdentityCookies();

            var authConnectionString = builder.Configuration.GetConnectionString("AuthDbConnection") ?? throw new InvalidOperationException("Connection string 'AuthDbConnection' not found.");
      
            var serverVersion = new MySqlServerVersion(ServerVersion.AutoDetect(authConnectionString));
            builder.Services.AddDbContext<AuthDbContext>(options =>
            {
                options.UseMySql(authConnectionString, serverVersion);
                //options.UseSqliste(flightDbConnectionString);
                if (isDevelopment)
                {
                    options
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }
            });

            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();


            builder.Configuration.AddJsonFile($"appsettings.json");
            builder.Configuration.AddJsonFile($"appsettings.{_ENVIRONMENT}.json", true);

            var feederBaseurl = "";

            if (!string.IsNullOrEmpty(builder.Configuration["FeederOptions:FeederUrl"]))
                feederBaseurl = builder.Configuration["FeederOptions:FeederUrl"];
            else if (!string.IsNullOrEmpty(builder.Configuration["FeederUrl"]))
                feederBaseurl = builder.Configuration["FeederOptions:FeederUrl"];
            Uri feederBaseUri = new Uri(feederBaseurl ?? throw new ArgumentNullException("AddHttpClient Error: FeederOptions:FeederUrl or FeederUrl not set"));


            builder.Services.Configure<FeederOptions>(builder.Configuration.GetSection(FeederOptions.Position));
            builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.Position));

            //string connectionString = $"server=mysqladsb; userid=adsb; pwd=8vJzIjW3qAO5;port=3306; database=adsb;SslMode=none;allowpublickeyretrieval=True;";
            var flightDbConnectionString = builder.Configuration.GetConnectionString("FlightDbConnection") ?? 
                throw new InvalidOperationException("Connection string 'flightDbConnectionString' not found.");

            var flightDbServerVersion = new MySqlServerVersion(ServerVersion.AutoDetect(flightDbConnectionString));

            builder.Services.AddDbContextFactory<FlightDbContext>(options =>
            {
                options.UseMySql(flightDbConnectionString, flightDbServerVersion);
                //options.UseSqliste(flightDbConnectionString);
                if (isDevelopment)
                {
                    options
                        .EnableSensitiveDataLogging()
                        .EnableDetailedErrors();
                }
            });

            builder.Services.AddMemoryCache();
            builder.Services.AddHttpClient();
            builder.Services.AddTransient<PlanePhotosCacheService>();
            builder.Services
                .AddScoped<IFlightFetcher, FlightFetcher>()
                //.AddScoped<PlanePhotosService>()
                //.AddScoped<IPlaneImageService, PlanespottersService>()
                .AddHostedService<FlightWorker>()
                .AddScoped<ICoordUtils, CoordUtils>()
                .AddScoped<FlightsService>()
                .AddScoped<FeederService>();

            AppDomain currDomain = AppDomain.CurrentDomain;
            currDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionHandler);

            builder.Services.AddMudServices();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAntiforgery();

            app.MapRazorComponents<App>()
                .AddInteractiveServerRenderMode();

            // Add additional endpoints required by the Identity /Account Razor components.
            app.MapAdditionalIdentityEndpoints();
            app.Run();
        }

        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("MyHandler caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }
    }
}
