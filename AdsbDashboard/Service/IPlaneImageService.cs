using AdsbDashboard.Models;

namespace AdsbDashboard.Service
{
    public interface IPlaneImageService
    {
        public IPlaneImage? GetImage(Flight flight);
        public Task<IPlaneImage?> GetImageAsync(Flight flight);
        public Task<IPlaneImage?> GetImageByHex(string hex);
    }

    public interface IPlaneImage
    {
        public string? Id { get; set; }
        public string Src { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Link { get; set; }
        public string Photographer { get; set; }
    }
}
