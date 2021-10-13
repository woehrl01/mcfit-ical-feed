using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace McFitCourseFeed.Api
{
    public interface IMcFitCourseApi
    {
        Task<McFitCourseResponse[][]> LoadFromMcFit(string id);
    }

    public class McFitCourseApi : IMcFitCourseApi
    {

        private readonly HttpClient _client;
        public McFitCourseApi(HttpClient client)
        {
            _client = client;
        }

        public async Task<McFitCourseResponse[][]> LoadFromMcFit(string id)
        {
            /* 
             * it looks like that the API requires some special 
             * treatment for StartDate and EndDate.
             * Therfore we always send the beginning of this week as the StartDate
             * The EndDate is 30 Days in the future based on the StartDate
             */
            var from = DateTime.Today.StartOfWeek(DayOfWeek.Monday);
            var to = from.AddDays(30);

            var request = new McFitCourseRequest
            {
                StudioId = id,
                StartDate = from.ToString("yyyy-MM-dd"),
                EndDate = to.ToString("yyyy-MM-dd")
            };
            var requestStr = JsonSerializer.Serialize(request);
            var postBody = new StringContent(requestStr, Encoding.UTF8, "application/json");
            using (var responseBody = await _client.PostAsync("https://coursplan-proxy.herokuapp.com/api/classes", postBody))
            {
                var result = await responseBody.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<McFitCourseResponse[][]>(result);
            }
        }
    }
}