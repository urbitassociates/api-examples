using System.Text.Json.Serialization;
using SampleWebhookService.Models.Generic;

namespace SampleWebhookService.Models.Delivered;

public class DeliveredEvent
{
    [JsonPropertyName("data")] public DeliveredDataObject DeliveredData { get; set; }

    [JsonPropertyName("event")] public EventObject Event { get; set; }
}
