using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text;
using Ical.Net;
using Ical.Net.CalendarComponents;
using Ical.Net.DataTypes;
using Ical.Net.Serialization;
using McFitCourseFeed.Api;

namespace McFitCourseFeed.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CourseFeedController : ControllerBase
    {
        private const string Timezone = "Europe/Berlin";
        private readonly ILogger<CourseFeedController> logger;
        private readonly IMcFitCourseApi api;

        public CourseFeedController(ILogger<CourseFeedController> logger, IMcFitCourseApi api)
        {
            this.logger = logger;
            this.api = api;
        }

        private static string BuildTitle(McFitCourseResponse r)
        {
            var builder = new StringBuilder();

            if (r.Streaming != "No")
            {
                //builder.Append("🎥 ");
                builder.Append("(S) ");
            }

            if (r.Liveclass != "No")
            {
                //builder.Append("👨 ");
                builder.Append("(L) ");
            }

            builder.Append(r.Classtitle);

            return builder.ToString();
        }

        [HttpGet("/")]
        public string Status()
        {
            return "looks good!";
        }

        [HttpGet("/coursefeed/{clubId}.ical")]
        public async Task<IActionResult> Get([FromRoute] string clubId, [FromQuery] int stream, [FromQuery] int live, [FromQuery] int hideOld)
        {


            var courses = await api.LoadFromMcFit(clubId);

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
                    Start = new CalDateTime(c.Startdate, Timezone),
                    End = new CalDateTime(c.Enddate, Timezone)
                });

            var calendar = new Calendar();
            calendar.AddTimeZone(new VTimeZone(Timezone));
            calendar.Events.AddRange(events);
            var iCalSerializer = new CalendarSerializer();
            string result = iCalSerializer.SerializeToString(calendar);

            return File(Encoding.UTF8.GetBytes(result), "text/calendar", "calendar.ics");
        }

    }

}
