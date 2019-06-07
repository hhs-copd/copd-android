using System.Threading.Tasks;

namespace LocationTest.Services
{
    public interface IAuthenticationService
    {
        Task<LoginResult> Authenticate();

        LoginResult AuthenticationResult { get; }
    }
}