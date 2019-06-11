using System.Threading.Tasks;

namespace LocationTest.Services
{
    public interface ILambdaFunctionDataService
    {
        Task<GraphResponse> GetGraph(string accessToken, string type);

        Task PostData(string accessToken, string fileLocation);
    }
}