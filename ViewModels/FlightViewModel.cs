using AdsbMudBlazor.Models;

namespace AdsbMudBlazor.ViewModels
{
    public class FlightViewModel
    {
        private Flight Flight { get; set; }

        public FlightViewModel(Flight flight)
        {
            Flight = flight;
        }
    }
    
}
