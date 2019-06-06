using Xamarin.Forms;
using Xamarin.Forms.Xaml;

[assembly: XamlCompilation(XamlCompilationOptions.Compile)]
namespace LocationTest
{
    public partial class App : Application
    {
        public App()
        {
            this.InitializeComponent();
            this.MainPage = new MainPage();
        }

        public App(ISignInViewModel signInViewModel)
        {
            this.InitializeComponent();

            this.MainPage = new MainPage
            {
                BindingContext = signInViewModel
            };
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
