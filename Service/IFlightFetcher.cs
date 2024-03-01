using AdsbMudBlazor.Models;

namespace AdsbMudBlazor.Service
{
    public interface IFlightFetcher
    {
        public Task<IEnumerable<Flight>> GetFlightsFromFeederAsync();
        public Task<int> GetCurrentlyTrackedFlightsCount();
    }
}
