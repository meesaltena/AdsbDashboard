
using AdsbMudBlazor.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace AdsbMudBlazor.Service
{
    public class PlanePhotosCacheHandler : DelegatingHandler
    {
        private readonly IMemoryCache _cache;

        public PlanePhotosCacheHandler(IMemoryCache cache) : base()
        {
            _cache = cache;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // If the request is not for api.planespotters.net, just forward it
            if (request.RequestUri.Host != "api.planespotters.net")
            {
                return await base.SendAsync(request, cancellationToken);
            }
            
            // Extract the hex code from the URL
            var hexCode = request.RequestUri.AbsolutePath.Split('/').Last();

            // Check the cache for the plane photo
            if (_cache.TryGetValue(hexCode, out PlanePhoto planePhoto))
            {
                // If we have the plane photo, return it
                return new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent(JsonSerializer.Serialize(planePhoto), Encoding.UTF8, "application/json")
                };
            }

            try
            {
                //Go get the PlanePhoto from the http endpoint
                var response = await base.SendAsync(request, cancellationToken);
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                _cache.Set(hexCode, JsonSerializer.Deserialize<PlanePhoto>(content), TimeSpan.FromMinutes(5));
                return response;
            }
            catch (Exception)
            {

                throw;
            }
   
        }
    }
}
