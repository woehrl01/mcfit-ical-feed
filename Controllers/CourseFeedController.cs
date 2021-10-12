using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using System.Text.Json.Serialization;

namespace mcfit_ical.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseFeedController : ControllerBase
    {
        private readonly ILogger<CourseFeedController> _logger;
        private readonly HttpClient _client;

        public CourseFeedController(ILogger<CourseFeedController> logger, HttpClient client)
        {
            _logger = logger;
            _client = client;
        }

        private static string BuildTitle(McFitCourseResponse r){
            var builder = new StringBuilder();

            if(r.Streaming != "No"){
                builder.Append("(S)  ");
            }

            if(r.Liveclass != "No"){
                builder.Append("(L) ");
            }

            builder.Append(r.Classtitle);
            
            return builder.ToString();
        }

        [HttpGet("/coursefeed/{clubId}.ical")]
        public async Task<IActionResult> Get([FromRoute]string clubId, [FromQuery]int stream, [FromQuery]int live, [FromQuery]int hideOld)
        {

            var courses = await LoadFromMcFit(clubId);

            string timezone = "Europe/Berlin";

            _logger.LogInformation("hide: {0}", hideOld);

            var events = courses
                .SelectMany(x => x)
                .Where(c => (hideOld == 1 && (c.Streaming != "No" || c.Liveclass != "No")) || hideOld != 1)
                .Where(c => (stream == 1 && c.Streaming != "No") || stream != 1)
                .Where(c => (live == 1 && c.Liveclass != "No") || live != 1)
                .Select(c => new CalendarEvent
                {
                    Summary = BuildTitle(c),
                    Uid = c.Id,
                    Description = c.Description,
                    Start = new CalDateTime(c.Startdate, timezone),
                    End = new CalDateTime(c.Enddate, timezone)
                });

            var calendar = new Calendar();
            calendar.Name = $"McFit {clubId}";
            calendar.AddTimeZone(new VTimeZone(timezone));
            calendar.Events.AddRange(events);
            var iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(calendar);

            return File(Encoding.ASCII.GetBytes(result), "calendar/text", "calendar.ics");
        }
        
        private async Task<McFitCourseResponse[][]> LoadFromMcFit(string id)
        {
            var today = DateTime.Today;
            var future = today.AddMonths(1);

            var request = new Request{
                StudioId = id,
                StartDate = today.ToString("yyyy-MM-dd"),
                EndDate = future.ToString("yyyy-MM-dd")
            };
            var postBody = new StringContent(JsonSerializer.Serialize(request), Encoding.UTF8, "application/json");
            using(var responseBody = await _client.PostAsync("https://coursplan-proxy.herokuapp.com/api/classes", postBody))
            using(var stream = await responseBody.Content.ReadAsStreamAsync())
            {
                return await JsonSerializer.DeserializeAsync<McFitCourseResponse[][]>(stream);
            }
        }   
    }

    public class Request{
        [JsonPropertyName("studioId")]
        public string StudioId {get; set;}

        [JsonPropertyName("startDate")]
        public string StartDate {get; set;}

        [JsonPropertyName("endDate")]
        public string EndDate {get; set;}
    };

}
