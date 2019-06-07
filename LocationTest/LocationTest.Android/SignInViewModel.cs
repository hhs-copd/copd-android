using Auth0.OidcClient;
using IdentityModel.OidcClient;
using System.ComponentModel;
using System.Windows.Input;
using Xamarin.Forms;

namespace LocationTest
{
    internal class SignInViewModel : ISignInViewModel
    {
        private readonly Auth0Client _client = new Auth0Client(new Auth0ClientOptions
        {
            Domain = "copd.eu.auth0.com",
            ClientId = "Q0vV85RwoMnKUzaLwGAB8RKoPPh85Oww",
            Scope = "openid profile"
        });

        public event PropertyChangedEventHandler PropertyChanged;

        public ICommand SignInCommand { get; set; }

        public LoginResult LoginResult;

        private bool _isNotLoggedIn = true;
        public bool IsNotLoggedIn
        {
            get => this._isNotLoggedIn;
            set
            {
                this._isNotLoggedIn = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.IsNotLoggedIn)));
            }
        }

        private string _welcomeMessage = "Please sign in to use the COPD Monitoring app.";
        public string WelcomeMessage
        {
            get => this._welcomeMessage;
            set
            {
                this._welcomeMessage = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(this.WelcomeMessage)));
            }
        }

        public SignInViewModel()
        {
            this.SignInCommand = new Command(this.SignIn);
        }

        public async void SignIn()
        {
            this.LoginResult = await this._client.LoginAsync();
            this.IsNotLoggedIn = this.LoginResult.IsError;
            if (this.LoginResult.IsError == false)
            {
                this.WelcomeMessage = $"Welcome to the app, {this.LoginResult.User?.Identity.Name}!";
            }
        }
    }
}
