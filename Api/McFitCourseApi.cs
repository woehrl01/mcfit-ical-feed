using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace McFitCourseFeed.Api
{
    public interface IMcFitCourseApi
    {
        Task<McFitCourseResponse[][]> LoadFromMcFit(string id, DateTime from, DateTime to);
    }

    public class McFitCourseApi : IMcFitCourseApi
    {

        private readonly HttpClient _client;
        public McFitCourseApi(HttpClient client)
        {
            _client = client;
        }

        public async Task<McFitCourseResponse[][]> LoadFromMcFit(string id, DateTime from, DateTime to)
        {
            var request = new McFitCourseRequest
            {
                StudioId = id,
                StartDate = from.ToString("yyyy-MM-dd"),
                EndDate = to.ToString("yyyy-MM-dd")
            };
            var postBody = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using (var responseBody = await _client.PostAsync("https://coursplan-proxy.herokuapp.com/api/classes", postBody))
            using (var stream = await responseBody.Content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<McFitCourseResponse[][]>(stream);
            }
        }
    }
}