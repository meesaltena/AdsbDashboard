using AdsbMudBlazor.Service;

namespace AdsbMudBlazor.Models
{
    public class PlanespottersImage : IPlaneImage
    {
        public string Src { get; set; } = string.Empty;
        public int Width { get; set; }
        public int Height { get; set; }
        public string? Id { get; set; }
        public string? Link { get; set; }
        public string Photographer { get; set; }
    }
}
