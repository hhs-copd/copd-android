namespace LocationTest.ViewModels
{
    internal class UVZoomModel : IGraphZoomModel
    {
        public string Name => "UV";
        public int Max => 100;
        public int Min => 0;
        public string[] GraphItems => new[] { "UVA", "UVB", "UVIndex" };
    }
}
