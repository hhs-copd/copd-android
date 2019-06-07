using Xamarin.Forms;

namespace LocationTest
{
    public class MainPage : ContentPage
    {
        public MainPage()
        {
            // Create the Button and attach Clicked handler.
            Label label = new Label
            {
                Text = "Welcome to the app!",
                VerticalOptions = LayoutOptions.Center
            };

            this.Padding = new Thickness(5, Device.RuntimePlatform == Device.iOS ? 20 : 0, 5, 0);

            this.Content = new StackLayout
            {
                Children =
                {
                    label
                }
            };
        }
    }
}
