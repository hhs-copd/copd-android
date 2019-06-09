using LocationTest.Services;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocationTest.Pages
{
    public class MainPage : ContentPage
    {
        private List<string> ConnectedDevices { get; } = new List<string>();

        private readonly StackLayout devicesStackLayout;

        public MainPage()
        {
            // Create the Button and attach Clicked handler.
            Label label = new Label
            {
                Text = "Connected devices:",
                FontSize = 14,
                Margin = new Thickness(0, 30, 0, 5),
                VerticalOptions = LayoutOptions.Center
            };


            this.devicesStackLayout = new StackLayout();

            this.Padding = new Thickness(5, Device.RuntimePlatform == Device.iOS ? 20 : 0, 5, 0);

            this.Content = new ScrollView
            {
                Content = new StackLayout()
                {
                    Children = {
                        label,
                        this.devicesStackLayout
                    }
                }
            };

            DependencyService.Register<IBluetoothService>();
            DependencyService.Get<IBluetoothService>().ConnectAndWrite(new BluetoothHandler
            {
                OnConnect = (name) => {
                    this.ConnectedDevices.Add(name);
                    this.UpdateDevices();
                },
                OnDisconnect = (name) => {
                    this.ConnectedDevices.Remove(name);
                    this.UpdateDevices();
                }
            });
        }

        private void UpdateDevices()
        {
            this.devicesStackLayout.Children.Clear();
            foreach (string device in this.ConnectedDevices)
            {
                this.devicesStackLayout.Children.Add(new Label
                {
                    Text = device,
                    FontSize = 18,
                    Margin = new Thickness(5, 10),
                    FontAttributes = FontAttributes.Bold
                });
            }
        }
    }
}
