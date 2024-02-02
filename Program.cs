using AdsbMudBlazor.Components;
using AdsbMudBlazor.Data;
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
            builder.WebHost.ConfigureKestrel((context, serverOptions) =>
            {
                serverOptions.Listen(IPAddress.Any, 5000);
                //serverOptions.Listen(IPAddress.Any, 5001);
            });

            builder.Logging.AddConsole();
            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();
            builder.Services
                .AddDbContextFactory<FlightDbContext>()
                .AddScoped<IFlightFetcher, FlightFetcher>()
                .AddHostedService<FlightWorker>()
                .AddScoped<ICoordUtils, CoordUtils>()
                .AddScoped<FlightsService>()
                .AddScoped<FeederService>()
                .AddHttpClient();

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
