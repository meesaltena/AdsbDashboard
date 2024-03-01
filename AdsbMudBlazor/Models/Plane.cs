using System.ComponentModel.DataAnnotations;

namespace AdsbMudBlazor.Models
{
    public class Plane
    {
        // ICAO code registration
        [Key]
        public string ModeS { get; set; }

        public DateTime? LastSeen { get; set; }
    }
}
