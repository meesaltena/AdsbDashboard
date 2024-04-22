using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdsbDashboard.Models
{
    public class Flight
    {   
        public int Id { get; set; }

        [MaxLength(12)]
        public string ModeS { get; set; } = string.Empty;
        public string Callsign { get; set; } = string.Empty;
        public string Alt { get; set; } = string.Empty;

        /// <summary>
        /// knots
        /// </summary>
        public int Groundspeed { get; set; }
        public string Squawk { get; set; } = string.Empty;
        public double Lat { get; set; }
        public double Long { get; set; }

        public double? Distance { get; set; }

        //[NotMapped]
        public double DistanceKm => Distance / 1000 ?? Double.MaxValue;
        public DateTime DateTime { get; set; }



        // override object.Equals
#pragma warning disable CS8765
        public override bool Equals(object obj)
#pragma warning restore CS8765 
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            } else
            {
                return this.ModeS == ((Flight)obj).ModeS &&
                       this.Callsign == ((Flight)obj).Callsign &&
                       this.Alt == ((Flight)obj).Alt &&
                       this.Groundspeed== ((Flight)obj).Groundspeed &&
                       this.Squawk == ((Flight)obj).Squawk &&
                       this.Lat == ((Flight)obj).Lat &&
                       this.Long == ((Flight)obj).Long;
            }
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModeS, Callsign, Alt, Groundspeed, Squawk, Lat, Long);
        }
    }
}