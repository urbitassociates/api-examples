using System.Text.Json.Serialization;

namespace SampleWebhookService.Models.Generic;

public class EventObject
{
    [JsonPropertyName("event_process")] public string EventProcess { get; set; }

    [JsonPropertyName("event_type")] public string EventType { get; set; }

    [JsonPropertyName("tracking_number")] public string TrackingNumber { get; set; }

    [JsonPropertyName("barcode")] public string Barcode { get; set; }

    [JsonPropertyName("reference_id")] public ReferenceId ReferenceId { get; set; }

    [JsonPropertyName("reference_id2")] public ReferenceId ReferenceId2 { get; set; }

    [JsonPropertyName("timestamp")] public DateTime Timestamp { get; set; }

    [JsonPropertyName("shipment")] public ShipmentObject Shipment { get; set; }
}