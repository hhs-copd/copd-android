using LocationTest.Services;
using LocationTest.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Net.Http;
using System.Threading.Tasks;

namespace LocationTest.Pages
{
    public class MainPage : ContentPage
    {
        private static readonly HttpClient client = new HttpClient();
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
            Button buttonSendData = new Button
            {
                Text = "Send Data",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(20, 5)
            };
            //buttonSendData.Clicked += ButtonSendData_Clicked;
            Button buttonConnectBle = new Button
            {
                Text = "Connect to device",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(0, 20, 0, 0),
                Padding = new Thickness(20, 5)
            };
            buttonConnectBle.Clicked += this.OnConnect;
            Button buttonPlot = new Button
            {
                Text = "Generate Plot",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(0, 20,0, 0),
                Padding = new Thickness(20, 5)
            };
            buttonPlot.Clicked += this.OnButtonClicked;

            this.devicesStackLayout = new StackLayout();

            this.Padding = new Thickness(5, Device.RuntimePlatform == Device.iOS ? 20 : 0, 5, 0);

            this.Content = new ScrollView
            {
                Content = new StackLayout()
                {
                    Children = {
                        label,
                        buttonConnectBle,
                        buttonPlot,
                        this.devicesStackLayout
                    }
                }
            };

            DependencyService.Register<IBluetoothService>();
            DependencyService.Get<IBluetoothService>().ConnectAndWrite(new BluetoothHandler
            {
                OnConnect = (name) =>
                {
                    this.ConnectedDevices.Add(name);
                    this.UpdateDevices();
                },
                OnDisconnect = (name) =>
                {
                    this.ConnectedDevices.Remove(name);
                    this.UpdateDevices();
                }
            });
        }

        private async Task ButtonSendData_Clicked(object sender, EventArgs e)
        {
            var values = new Dictionary<string, string>
{
   { "thing1", "hello" },
   { "thing2", "world" }
};

            var content = new FormUrlEncodedContent(values);

            var response = await client.PostAsync("http://www.example.com/recepticle.aspx", content);

            var responseString = await response.Content.ReadAsStringAsync();
        }

        private void OnConnect(object sender, EventArgs e)
        {
            //DependencyService.Register<IBluetoothService>();
            //DependencyService.Get<IBluetoothService>().ConnectAndWrite(new BluetoothHandler
            //{
            //    OnConnect = (name) =>
            //    {
            //        this.ConnectedDevices.Add(name);
            //        this.UpdateDevices();
            //    },
            //    OnDisconnect = (name) =>
            //    {
            //        this.ConnectedDevices.Remove(name);
            //        this.UpdateDevices();
            //    }
            //});
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {

            Application.Current.MainPage = new NavigationPage(new LinePlotView(null));
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
