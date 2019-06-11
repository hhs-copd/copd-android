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
        public async Task<GraphResponse> GetGraph(string accessToken, string type)
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("X-Value", type);
            HttpResponseMessage response = await client.GetAsync(LambdaConfig.RootUrl + "/graph");

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
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Add("Content-Type", "application/json");

            using (FileStream fileStream = File.OpenRead(fileLocation))
            using (StreamReader streamReader = new StreamReader(fileStream))
            {
                string content = await streamReader.ReadToEndAsync();
                StringContent httpContent = new StringContent(JsonConvert.SerializeObject(new { content }));
                HttpResponseMessage response = await client.PostAsync(LambdaConfig.RootUrl, httpContent);
                response.EnsureSuccessStatusCode();
            }
        }
    }
}