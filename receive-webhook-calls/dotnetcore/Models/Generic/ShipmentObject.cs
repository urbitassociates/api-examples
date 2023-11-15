using System.Text.Json.Serialization;

namespace SampleWebhookService.Models.Generic;

public class ShipmentObject
{
    [JsonPropertyName("shipment_number")] public string ShipmentNumber { get; set; }

    [JsonPropertyName("client_id")] public string ClientId { get; set; }

    [JsonPropertyName("reference_id")] public ReferenceId ReferenceId { get; set; }

    [JsonPropertyName("reference_id2")] public ReferenceId ReferenceId2 { get; set; }
}
