using AdsbMudBlazor.Components;
using AdsbMudBlazor.Data;
using AdsbMudBlazor.Service;
using MudBlazor.Services;

namespace AdsbMudBlazor
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services
                .AddDbContextFactory<FlightDbContext>()
                .AddHostedService<FlightWorker>()
                .AddScoped<FlightsService>()
                .AddScoped<FeederService>()
                .AddHttpClient();


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
    }
}
