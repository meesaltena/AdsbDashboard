using AdsbDashboard.Models;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace AdsbDashboard.Service
{
    public class PlanePhotosCacheService
    {
        private readonly IMemoryCache _cache;
        private readonly HttpClient _httpClient;

        public PlanePhotosCacheService(IMemoryCache cache, HttpClient httpClient)
        {
            _cache = cache;
            _httpClient = httpClient;
        }

        public PlanePhoto SetImageByHex(string hexCode, PlanePhoto planePhoto)
        {
            return _cache.Set(hexCode, planePhoto, TimeSpan.FromMinutes(720));
        }

        public async Task<PlanePhoto> GetImageByHex(string hexCode)
        {
            // Check the cache for the plane photo
            if (_cache.TryGetValue(hexCode, out PlanePhoto planePhoto))
            {
                // If we have the plane photo, return it
                return planePhoto;
            }
            return null;

        }
    }
}
