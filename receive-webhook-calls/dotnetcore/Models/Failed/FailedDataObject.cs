using System.Text.Json.Serialization;

namespace SampleWebhookService.Models.Failed;

public class FailedDataObject
{
    [JsonPropertyName("comment")] public string Comment { get; set; }

    [JsonPropertyName("reason")] public string Reason { get; set; }
}
