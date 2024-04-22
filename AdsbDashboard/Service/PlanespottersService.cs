using System.Globalization;
using System.Net.Http;
using System.Text.Json;
using AdsbDashboard.Components.Pages;
using AdsbDashboard.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AdsbDashboard.Service
{
    public class PlanespottersService : IPlaneImageService
    {

        private readonly HttpClient _httpClient;
        private readonly ILogger<PlanespottersService> _logger;
        private readonly string baseAddress = "https://api.planespotters.net/pub/photos/hex/";
        public PlanespottersService(HttpClient httpClient, ILogger<PlanespottersService> logger)
        {
            _httpClient = httpClient;
            //httpClient.BaseAddress = new Uri(baseAddress);
            _logger = logger;
        }

        public IPlaneImage? GetImage(Flight flight)
        {
            return GetImageByHex(flight.ModeS).Result;
        }

        public async Task<IPlaneImage?> GetImageAsync(Flight flight)
        {
            return await GetImageByHex(flight.ModeS);
        }

        public async Task<IPlaneImage?> GetImageByHex(string hex)
        {
            try
            {

                //var url = _httpClient.BaseAddress.AbsolutePath + hex;

                //var response = await _httpClient.GetAsync($"https://api.planespotters.net/pub/photos/hex/{hex}");
                //response,.
                //_logger.LogDebug(response.ToString(), $"GetImageByHex {hex}");
                ////TODO deserialize properly
                //var document = await JsonDocument.ParseAsync(response);
                //foreach (var property in document.RootElement.EnumerateObject())
                //{
                //    var photos = property.Value;

                //    //var planespotters = new PlanespottersPhoto
                //    //{
                //    //    Src 
                //    //    ModeS = flightData[0].GetString()!,
                //    //    Callsign = flightData[16].GetString()!,
                //    //    Lat = flightData[1].GetDouble(),
                //    //    Long = flightData[2].GetDouble(),
                //    //    Alt = flightData[4].GetInt32().ToString(),
                //    //    Squawk = flightData[6].GetString()!,
                //    //    DateTime = DateTime.UtcNow,
                //    //};

                //}

            }
            catch (Exception e)
            {
                _logger.LogError(e, "PlanespottersService GetImage: ");
            }
            return new PlanespottersImage()
            {
            };
        }
    }
}
