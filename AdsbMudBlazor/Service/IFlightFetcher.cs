using AdsbMudBlazor.Models;

namespace AdsbMudBlazor.Service
{
    public interface IFlightFetcher
    {
        public Task<IEnumerable<Flight>> GetFlightsFromFeederAsync(CancellationToken token = default);
        public Task<int> GetCurrentlyTrackedFlightsCount(CancellationToken token = default);
    }
}
