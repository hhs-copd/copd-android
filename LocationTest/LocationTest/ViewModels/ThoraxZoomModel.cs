namespace LocationTest.ViewModels
{
    internal class ThoraxZoomModel : IGraphZoomModel
    {
        public string Name => "Thorax Circumference";
        public int Max => 400;
        public int Min => 0;
        public string[] GraphItems => new[] { "ThoraxCircumference" };
    }
}
