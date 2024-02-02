using AdsbMudBlazor.Models;

namespace AdsbMudBlazor.Service
{
    public interface IFlightFetcher
    {
        public IEnumerable<Flight> GetFlightsFromFeeder();

        public Task<IEnumerable<Flight>> GetFlightsFromFeederAsync();
    }
}
