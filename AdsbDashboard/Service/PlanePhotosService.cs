using System.Text.Json;

namespace AdsbDashboard.Service
{
    public class PlanePhotosService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PlanePhotosService> _logger;
        public PlanePhotosService(HttpClient httpClient, ILogger<PlanePhotosService> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PlanePhotosResponse> GetPlanePhotosAsync(string hexCode)
        {
            try
            { 
                _logger.LogError("BaseUrl: " + _httpClient.BaseAddress);
                var response = await _httpClient.GetAsync($"https://api.planespotters.net/pub/photos/hex/{hexCode}");

                var c = response.Content;

                response.EnsureSuccessStatusCode();
                _logger.LogInformation($"GetPlanePhotosAsync statuscode: {response.StatusCode}");

                var content = await response.Content.ReadAsStringAsync();

                return JsonSerializer.Deserialize<PlanePhotosResponse>(content);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "GetPlanePhotosAsync Exception: ");
                throw;
            }

        }
    }

    public class PhotoSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class Photo
    {
        public string Id { get; set; }
        public PhotoSize Size { get; set; }
        public string Src { get; set; }
    }

    public class PlanePhoto
    {
        public string Id { get; set; }
        public Photo Thumbnail { get; set; }
        public Photo Thumbnail_Large { get; set; }
        public string Link { get; set; }
        public string Photographer { get; set; }
    }

    public class PlanePhotosResponse
    {
        public List<PlanePhoto> Photos { get; set; }
    }
}
