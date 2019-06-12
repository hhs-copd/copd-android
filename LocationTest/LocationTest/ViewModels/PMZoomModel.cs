namespace LocationTest.ViewModels
{
    internal class PMZoomModel : IGraphZoomModel

    {
        public string Name => "Particulate Matter";
        public int Max => 30;
        public int Min => 0;
        public string[] GraphItems => new[] { "PM10p0", "PM4p0", "PM2p5", "PM1p0" };
    }
}
