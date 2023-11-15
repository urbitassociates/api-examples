using System.Text.Json.Serialization;
using SampleWebhookService.Models.Generic;

namespace SampleWebhookService.Models.Failed;

public class FailedEvent
{
    [JsonPropertyName("data")] public FailedDataObject Data { get; set; }

    [JsonPropertyName("event")] public EventObject Event { get; set; }
}
