using Auth0.OidcClient;
using LocationTest.Config;
using LocationTest.Droid.Services;
using LocationTest.Services;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(AuthenticationService))]
namespace LocationTest.Droid.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly Auth0Client _auth0Client;

        public AuthenticationService()
        {
            this._auth0Client = new Auth0Client(new Auth0ClientOptions
            {
                Domain = AuthenticationConfig.Domain,
                ClientId = AuthenticationConfig.ClientId
            });
        }

        public LoginResult AuthenticationResult { get; private set; }

        public async Task<LoginResult> Authenticate()
        {
            IdentityModel.OidcClient.LoginResult response = await this._auth0Client.LoginAsync();

            LoginResult result = new LoginResult
            {
                Username = response.User?.Identity?.Name,
                RefreshToken = response.RefreshToken,
                IdToken = response.IdentityToken,
                AccessToken = response.AccessToken,
                AccessTokenExpiration = response.AccessTokenExpiration,
                Error = response.IsError
            };

            return this.AuthenticationResult = result;
        }
    }
}