using LocationTest.Services;
using LocationTest.Views;
using System;
using System.Collections.Generic;
using System.IO;
using Xamarin.Forms;

namespace LocationTest.Pages
{
    public class MainPage : ContentPage
    {
        private readonly StackLayout ConnectedDevicesLayout = new StackLayout();

        private readonly IBluetoothService BluetoothService;

        private readonly ILambdaFunctionDataService LambdaFunctionDataService;

        private readonly LoginResult LoginResult;

        public MainPage(LoginResult loginResult)
        {
            this.Title = "Breeze Home";

            this.LoginResult = loginResult;

            DependencyService.Register<IBluetoothService>();
            DependencyService.Register<ILambdaFunctionDataService>();

            this.BluetoothService = DependencyService.Get<IBluetoothService>();
            this.LambdaFunctionDataService = DependencyService.Get<ILambdaFunctionDataService>();

            this.BluetoothService.Listen(new BluetoothHandler
            {
                OnConnect = _ => this.UpdateDevices(),
                OnDisconnect = _ => this.UpdateDevices()
            });

            this.UpdateDevices();
            this.OnConnect(null, new EventArgs());

            Label connectedDevicesLabel = new Label
            {
                Text = "Connected Devices",
                FontSize = 14,
                Margin = new Thickness(10, 30, 0, 5),
                VerticalOptions = LayoutOptions.Center
            };


            Button buttonConnectBle = new Button
            {
                Text = "Connect to wearable",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(0, 10, 0, 0),
                Padding = new Thickness(20, 5)
            };
            buttonConnectBle.Clicked += this.OnConnect;

            Button buttonPlot = new Button
            {
                Text = "Show data graphs",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            buttonPlot.Clicked += this.OnButtonClicked;

            var buttonGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            buttonGrid.Children.Add(buttonConnectBle);
            buttonGrid.Children.Add(buttonPlot);
            Grid.SetColumn(buttonPlot, 1);

            Button buttonUpload = new Button
            {
                Text = "Upload data",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            buttonUpload.Clicked += ButtonUpload_Clicked;
            this.Padding = new Thickness(5, Device.RuntimePlatform == Device.iOS ? 20 : 0, 5, 0);
            this.Content = new ScrollView
            {
                Content = new StackLayout()
                {
                    Children = {
                        buttonUpload,
                        buttonGrid,
                        connectedDevicesLabel,
                        this.ConnectedDevicesLayout
                    }
                }
            };
        }

        private async void ButtonUpload_Clicked(object sender, EventArgs e)
        {
            string from = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "data.csv");
            string to = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "upload.csv");
            File.Copy(from, to);
            File.Delete(from);
            try{

                await this.LambdaFunctionDataService.PostData(this.LoginResult.AccessToken, to);

                System.IO.File.Delete(to);
            }
            catch (Exception exept)
            {
                //De button is nu helemaal waardeloos, ik kijk er later na
                Console.WriteLine(exept);
            }
        }

        private async void OnConnect(object sender, EventArgs e)
        {
            await this.BluetoothService.Scan();
        }

        private void OnButtonClicked(object sender, EventArgs e)
        {
            this.Navigation.PushAsync(new LinePlotView(this.LoginResult));
        }

        private void UpdateDevices()
        {
            this.ConnectedDevicesLayout.Children.Clear();

            List<string> devices = this.BluetoothService.GetConnectedDevices();

            if (devices.Count == 0)
            {
                this.ConnectedDevicesLayout.Children.Add(new Label
                {
                    Text = "No devices found...",
                    FontSize = 18,
                    Margin = new Thickness(20, 0, 0, 10),
                    FontAttributes = FontAttributes.Bold,
                    TextColor = Color.Gray
                });
            }

            foreach (string device in devices)
            {
                this.ConnectedDevicesLayout.Children.Add(new Label
                {
                    Text = device,
                    FontSize = 18,
                    Margin = new Thickness(20, 0, 0, 10),
                    FontAttributes = FontAttributes.Bold
                });
            }
        }
    }
}
