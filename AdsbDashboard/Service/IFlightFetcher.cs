using AdsbDashboard.Models;

namespace AdsbDashboard.Service
{
    public interface IFlightFetcher
    {
        public Task<IEnumerable<Flight>> GetFlightsFromFeederAsync(CancellationToken token = default);
        public Task<int> GetCurrentlyTrackedFlightsCount(CancellationToken token = default);
    }
}
