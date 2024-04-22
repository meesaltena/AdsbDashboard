using System.ComponentModel.DataAnnotations;

namespace AdsbDashboard.Models
{
    public class Plane
    {
        // ICAO code registration
        [Key]
        public string ModeS { get; set; }

        public DateTime? LastSeen { get; set; }

        public override bool Equals(object? obj)
        {
            return (obj is Plane) ?
                 ((obj is Plane plane) && ModeS == plane.ModeS) :
                 ((obj is string splane) && ModeS == splane);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ModeS);
        }

        public static bool operator ==(Plane? left, Plane? right)
        {
            return EqualityComparer<Plane>.Default.Equals(left, right);
        }

        public static bool operator !=(Plane? left, Plane? right)
        {
            return !(left == right);
        }
    }
}
