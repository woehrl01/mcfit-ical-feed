using System;
using System.Text.Json.Serialization;

namespace mcfit_ical
{
    public class McFitCourseResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("startdate")]
        public DateTime Startdate { get; set; }

        [JsonPropertyName("enddate")]
        public DateTime Enddate { get; set; }

        [JsonPropertyName("trailer")]
        public string Trailer { get; set; }

        [JsonPropertyName("thumbnail")]
        public string Thumbnail { get; set; }

        [JsonPropertyName("category")]
        public string Category { get; set; }

        [JsonPropertyName("class_filename")]
        public string ClassFilename { get; set; }

        [JsonPropertyName("classtitle")]
        public string Classtitle { get; set; }

        [JsonPropertyName("liveclass")]
        public string Liveclass { get; set; }

        [JsonPropertyName("streaming")]
        public string Streaming { get; set; }

        [JsonPropertyName("instructor")]
        public string Instructor { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
    }

}
