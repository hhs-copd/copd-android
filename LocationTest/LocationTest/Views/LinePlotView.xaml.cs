using LocationTest.Services;
using LocationTest.ViewModels;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace LocationTest.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LinePlotView : ContentPage
    {
        public LinePlotView(LoginResult loginResult)
        {
            this.InitializeComponent();

            var model = App.Locator.LineViewModel;
            model.Token = loginResult.AccessToken;

            this.BindingContext = model;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            if (this.BindingContext is LinePlotViewModel linePlotViewModel)
            {
                linePlotViewModel.OnAppearing(null);
            }
        }
    }
}
