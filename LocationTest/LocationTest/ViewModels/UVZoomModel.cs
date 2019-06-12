namespace LocationTest.ViewModels
{
    class UVZoomModel : IGraphZoomModel
    {
        public int Max => 100;
        public int Min => 0;
        public string[] GraphItems => new[] { "UVA", "UVB", "UVIndex" };
    }
}
