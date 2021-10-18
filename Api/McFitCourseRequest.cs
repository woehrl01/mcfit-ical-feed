namespace McFitCourseFeed.Api;

public class McFitCourseRequest
{
    [JsonPropertyName("studioId")]
    public string StudioId { get; set; }

    [JsonPropertyName("startDate")]
    public string StartDate { get; set; }

    [JsonPropertyName("endDate")]
    public string EndDate { get; set; }
};
