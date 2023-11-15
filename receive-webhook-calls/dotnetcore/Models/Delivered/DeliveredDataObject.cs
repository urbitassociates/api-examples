using System.Text.Json.Serialization;
using SampleWebhookService.Models.Generic;

namespace SampleWebhookService.Models.Delivered;

public class DeliveredDataObject
{
    [JsonPropertyName("location")] public LocationObject Location { get; set; }

    [JsonPropertyName("with_issues")] public bool WithIssues { get; set; }

    [JsonPropertyName("comment")] public string Comment { get; set; }

    [JsonPropertyName("method")] public string Method { get; set; }
}