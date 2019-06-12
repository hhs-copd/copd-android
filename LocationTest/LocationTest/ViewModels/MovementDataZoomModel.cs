namespace LocationTest.ViewModels
{
    internal class MovementDataZoomModel : IGraphZoomModel
    {
        public string Name => "Movement";
        public int Max => 30000;
        public int Min => -30000;
        public string[] GraphItems => new[] { "AccelerometerX", "AccelerometerY", "AccelerometerZ", "GyroX", "GyroY", "GyroZ" };
    }
}
