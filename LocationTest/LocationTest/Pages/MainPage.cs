using LocationTest.Services;
using LocationTest.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace LocationTest.Pages
{
    public class MainPage : ContentPage
    {
        private List<string> ConnectedDevices { get; } = new List<string>();

        private readonly StackLayout devicesStackLayout;

        private readonly LoginResult LoginResult;

        public MainPage(LoginResult loginResult)
        {
            this.LoginResult = loginResult;

            // Create the Button and attach Clicked handler.
            Label label = new Label
            {
                Text = "Connected devices:",
                FontSize = 14,
                Margin = new Thickness(0, 30, 0, 5),
                VerticalOptions = LayoutOptions.Center
            };

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
            DependencyService.Register<ILambdaFunctionDataService>();
        }

        private void OnConnect(object sender, EventArgs e)
        {
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

        private void OnButtonClicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new LinePlotView(this.LoginResult));
        }

        private void UpdateDevices()
        {
            this.devicesStackLayout.Children.Clear();

            foreach (var device in this.ConnectedDevices)
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
