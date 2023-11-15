using System.Text.Json.Serialization;

namespace SampleWebhookService.Models.Generic;

public class ReferenceId
{
    [JsonPropertyName("data")] public string Data { get; set; }

    [JsonPropertyName("description")] public string Description { get; set; }
}
