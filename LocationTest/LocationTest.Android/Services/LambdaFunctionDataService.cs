using LocationTest.Config;
using LocationTest.Droid.Services;
using LocationTest.Services;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

[assembly: Dependency(typeof(LambdaFunctionDataService))]
namespace LocationTest.Droid.Services
{
    public class LambdaFunctionDataService : ILambdaFunctionDataService
    {
        private readonly HttpClient HttpClient = new HttpClient();

        public async Task<GraphResponse> GetGraph(string accessToken, string type)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Get, LambdaConfig.RootUrl + "/graph"))
            {
                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                requestMessage.Headers.Add("X-Value", type);
                HttpResponseMessage response = await this.HttpClient.SendAsync(requestMessage);

                var body = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();

                using (Stream stream = await response.Content.ReadAsStreamAsync())
                using (StreamReader streamReader = new StreamReader(stream))
                using (JsonTextReader reader = new JsonTextReader(streamReader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<GraphResponse>(reader);
                }
            }
        }

        public async Task PostData(string accessToken, string fileLocation)
        {
            using (var requestMessage = new HttpRequestMessage(HttpMethod.Post, LambdaConfig.RootUrl))
            using (FileStream fileStream = File.OpenRead(fileLocation))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string content = await streamReader.ReadToEndAsync();

                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(new { content }), System.Text.Encoding.UTF8, "application/json");
                requestMessage.Content = httpContent;

                requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                var response = await this.HttpClient.SendAsync(requestMessage);

                var body = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
        }
    }
}