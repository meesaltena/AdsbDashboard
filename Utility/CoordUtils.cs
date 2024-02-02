namespace AdsbMudBlazor.Utility
{
    public class CoordUtils: ICoordUtils
    {
        private readonly IConfiguration _configuration;

        public CoordUtils(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public double GetDistance(double lat1, double lon1, double lat2, double long2)
        {
            var oD = Math.PI / 180.0;
            var d1 = lat1 * oD;
            var d2 = lat2 * oD;
            var d3 = Math.Pow(Math.Sin((d2 - d1) / 2.0), 2.0) 
                     + Math.Cos(d1) * Math.Cos(d2) * 
                     Math.Pow(Math.Sin((long2 * oD - (lon1 * oD)) / 2.0), 2.0);
            
            return 6376500.0 * (2.0 * Math.Atan2(Math.Sqrt(d3), Math.Sqrt(1.0 - d3)));
        }
    }
}
