using LocationTest.Services;
using LocationTest.ViewModels;
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

        private readonly Button UploadButton;

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

            Label graphLabel = new Label
            {
                Text = "Show Graphs",
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


            this.UploadButton = new Button
            {
                Text = "Upload data",
                HorizontalOptions = LayoutOptions.Center,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            this.UploadButton.Clicked += this.ButtonUpload_Clicked;


            Button buttonThorax = new Button
            {
                Text = "Thorax Graph",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            buttonThorax.Clicked += (_, __) => this.OnButtonClicked(new ThoraxZoomModel());
            Button buttonUV = new Button
            {
                Text = "UV Graph",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            buttonUV.Clicked += (_, __) => this.OnButtonClicked(new UVZoomModel());

            Button buttonPM = new Button
            {
                Text = "PM Graph",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            buttonPM.Clicked += (_, __) => this.OnButtonClicked(new PMZoomModel());
            Button buttonMovement = new Button
            {
                Text = "Movement Graph",
                HorizontalOptions = LayoutOptions.CenterAndExpand,
                IsEnabled = true,
                IsVisible = true,
                Margin = new Thickness(00, 10, 0, 0),
                Padding = new Thickness(32, 5)
            };
            buttonMovement.Clicked += (_, __) => this.OnButtonClicked(new MovementDataZoomModel());
            Grid buttonGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            buttonGrid.Children.Add(buttonConnectBle);
            buttonGrid.Children.Add(this.UploadButton);
            Grid.SetColumn(this.UploadButton, 1);

            Grid plotGrid = new Grid
            {
                ColumnDefinitions = new ColumnDefinitionCollection
                {
                    new ColumnDefinition { Width = GridLength.Star },
                    new ColumnDefinition { Width = GridLength.Star }
                }
            };
            plotGrid.Children.Add(buttonThorax);
            plotGrid.Children.Add(buttonMovement);
            plotGrid.Children.Add(buttonPM);
            plotGrid.Children.Add(buttonUV);
            Grid.SetRow(buttonPM, 1);
            Grid.SetRow(buttonUV, 1);
            Grid.SetColumn(buttonMovement, 1);
            Grid.SetColumn(buttonUV, 1);

            this.Padding = new Thickness(5, Device.RuntimePlatform == Device.iOS ? 20 : 0, 5, 0);
            this.Content = new ScrollView
            {
                Content = new StackLayout()
                {
                    Children = {
                        buttonGrid,
                        connectedDevicesLabel,
                        this.ConnectedDevicesLayout,
                        graphLabel,
                        plotGrid
                    }
                }
            };
        }

        private async void ButtonUpload_Clicked(object sender, EventArgs e)
        {
            this.UploadButton.IsEnabled = false;
            this.UploadButton.Text = "Uploading...";

            try
            {
                string from = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "dataE24DDB60C06B.csv");
                string to = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "uploadE24DDB60C06B.csv");
                if (File.Exists(from))
                {
                    File.Copy(from, to);
                    File.Delete(from);

                    await this.LambdaFunctionDataService.PostData(this.LoginResult.AccessToken, to);

                    File.Delete(to);
                }
            }
            catch (Exception exept)
            {
                //De button is nu helemaal waardeloos, ik kijk er later na
                Console.WriteLine(exept);
            }

            try
            {
                string from = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "dataCA81BA4BDC02.csv");
                string to = Path.Combine("storage", "emulated", "0", "Android", "data", "com.copd.COPDMonitor.Android", "files", "uploadCA81BA4BDC02.csv");
                if (File.Exists(from))
                {
                    File.Copy(from, to);
                    File.Delete(from);

                    await this.LambdaFunctionDataService.PostData(this.LoginResult.AccessToken, to);

                    File.Delete(to);
                }
            }
            catch (Exception exept)
            {
                //De button is nu helemaal waardeloos, ik kijk er later na
                Console.WriteLine(exept);
            }

            this.UploadButton.IsEnabled = true;
            this.UploadButton.Text = "Upload data";
        }

        private async void OnConnect(object sender, EventArgs e)
        {
            await this.BluetoothService.Scan();
        }

        private void OnButtonClicked(IGraphZoomModel model)
        {
            this.Navigation.PushAsync(new LinePlotView(this.LoginResult, model));
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
