namespace LocationTest.ViewModels
{
    public interface IGraphZoomModel
    {
        string Name { get; }

        int Max { get; }

        int Min { get; }

        string[] GraphItems { get; }

    }
}
