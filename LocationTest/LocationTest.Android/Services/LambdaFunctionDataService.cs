using LocationTest.Config;
using LocationTest.Droid.Services;
using LocationTest.Services;
using Newtonsoft.Json;
using System.IO;
using System.Net.Http;
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
            this.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            this.HttpClient.DefaultRequestHeaders.Add("X-Value", type);
            HttpResponseMessage response = await this.HttpClient.GetAsync(LambdaConfig.RootUrl + "/graph");

            response.EnsureSuccessStatusCode();

            using (Stream stream = await response.Content.ReadAsStreamAsync())
            using (StreamReader streamReader = new StreamReader(stream))
            using (JsonTextReader reader = new JsonTextReader(streamReader))
            {
                JsonSerializer serializer = new JsonSerializer();
                return serializer.Deserialize<GraphResponse>(reader);
            }
        }

        public async Task PostData(string accessToken, string fileLocation)
        {
            this.HttpClient.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);

            using (FileStream fileStream = File.OpenRead(fileLocation))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string content = await streamReader.ReadToEndAsync();
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(new { content }), System.Text.Encoding.UTF8, "application/json");
                HttpResponseMessage response = await this.HttpClient.PostAsync(LambdaConfig.RootUrl, httpContent);
                var body = await response.Content.ReadAsStringAsync();
                response.EnsureSuccessStatusCode();
            }
        }
    }
}