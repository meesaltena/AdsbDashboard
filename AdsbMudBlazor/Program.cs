using AdsbMudBlazor.Components;
using AdsbMudBlazor.Components.Account;
using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using AdsbMudBlazor.Service;
using AdsbMudBlazor.Utility;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using MudBlazor.Services;
using System.Net;

namespace AdsbMudBlazor
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

            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseSqlite(authConnectionString));
            builder.Services.AddDatabaseDeveloperPageExceptionFilter();

            builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
                .AddEntityFrameworkStores<AuthDbContext>()
                .AddSignInManager()
                .AddDefaultTokenProviders();

            builder.Services.AddSingleton<IEmailSender<ApplicationUser>, IdentityNoOpEmailSender>();

        
    
            builder.Configuration.AddJsonFile($"appsettings.json");
            builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true);

            var feederBaseurl = "";

            if (!string.IsNullOrEmpty(builder.Configuration["FeederOptions:FeederUrl"]))
                feederBaseurl = builder.Configuration["FeederOptions:FeederUrl"];
            else if (!string.IsNullOrEmpty(builder.Configuration["FeederUrl"]))
                feederBaseurl = builder.Configuration["FeederOptions:FeederUrl"];
            Uri feederBaseUri = new Uri(feederBaseurl ?? throw new ArgumentNullException("AddHttpClient Error: FeederOptions:FeederUrl or FeederUrl not set"));


            var flightDbConnectionString = builder.Configuration.GetConnectionString("FlightDbConnection") ?? throw new InvalidOperationException("Connection string 'flightDbConnectionString' not found.");


            builder.Services.Configure<FeederOptions>(builder.Configuration.GetSection(FeederOptions.Position));
            builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.Position));
            builder.Services.AddDbContextFactory<FlightDbContext>(options =>
                options.UseSqlite(flightDbConnectionString))
                .AddScoped<IFlightFetcher, FlightFetcher>()
                .AddHostedService<FlightWorker>()
                .AddScoped<ICoordUtils, CoordUtils>()
                .AddScoped<FlightsService>()
                .AddScoped<FeederService>()
                .AddHttpClient<IFlightFetcher, FlightFetcher>(client =>
                {
                    client.BaseAddress = feederBaseUri;
                });

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


            // run pending migrations
            RunMigrationsMaybe(app.Services, logger, builder.Configuration);

            app.Run();
        }

        private static void RunMigrationsMaybe(IServiceProvider serviceProvider, ILogger logger, ConfigurationManager Configuration)
        {
            if (Configuration.GetValue<bool>("RunMigrations") == true)
            {
                using (var scope = serviceProvider.CreateScope())
                {
                    var flightDbContext = scope.ServiceProvider.GetRequiredService<FlightDbContext>();
                    var pendingF = flightDbContext.Database.GetPendingMigrations();
                    flightDbContext.Database.Migrate();

                    if (pendingF.Any())
                    {
                        logger.LogInformation($"Running {pendingF.Count()} pending FlightDbContext migrations.");
                        flightDbContext.Database.Migrate();
                    }

                    var authDbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
                    var pendingA = authDbContext.Database.GetPendingMigrations();
                    if (pendingA.Any())
                    {
                        logger.LogInformation($"Running {pendingA.Count()} pending AuthDbContext migrations.");
                        authDbContext.Database.Migrate();
                    }
                }
            }
            else
            {
                logger.LogInformation("RunMigrations false. Not running migrations.");
            }
        }


        static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception e = (Exception)args.ExceptionObject;
            Console.WriteLine("MyHandler caught : " + e.Message);
            Console.WriteLine("Runtime terminating: {0}", args.IsTerminating);
        }
    }
}
