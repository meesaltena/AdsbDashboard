namespace AdsbDashboard.Utility
{
    public interface ICoordUtils
    {
        public double GetDistance(double lat1, double lon1, double lat2, double long2);
        public double GetDistance(double planeLat, double planeLong);
        public double GetDistanceOrZero(double planeLat, double planeLong);
    }
}