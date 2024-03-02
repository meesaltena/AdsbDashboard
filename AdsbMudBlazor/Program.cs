using AdsbMudBlazor.Components;
using AdsbMudBlazor.Data;
using AdsbMudBlazor.Models;
using AdsbMudBlazor.Service;
using AdsbMudBlazor.Utility;
using MudBlazor.Services;
using System.Net;

namespace AdsbMudBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.AddConsole();
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Configuration.AddJsonFile($"appsettings.json");
            builder.Configuration.AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", true);

            var feederBaseurl = "";

            if (!string.IsNullOrEmpty(builder.Configuration["FeederOptions:FeederUrl"]))
                feederBaseurl = builder.Configuration["FeederOptions:FeederUrl"];
            else if (!string.IsNullOrEmpty(builder.Configuration["FeederUrl"]))
                feederBaseurl = builder.Configuration["FeederOptions:FeederUrl"];
            Uri feederBaseUri = new Uri(feederBaseurl ?? throw new ArgumentNullException("AddHttpClient Error: FeederOptions:FeederUrl or FeederUrl not set"));
    
                builder.Services.Configure<FeederOptions>(builder.Configuration.GetSection(FeederOptions.Position));
            builder.Services.Configure<DbOptions>(builder.Configuration.GetSection(DbOptions.Position));
            builder.Services
                .AddDbContextFactory<FlightDbContext>()
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
