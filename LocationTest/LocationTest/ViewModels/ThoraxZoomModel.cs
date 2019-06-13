namespace LocationTest.ViewModels
{
    internal class ThoraxZoomModel : IGraphZoomModel
    {
        public string Name => "Thorax Circumference";
        public int Max => 65;
        public int Min => 0;
        public string[] GraphItems => new[] { "ThoraxCircumference" };
    }
}
