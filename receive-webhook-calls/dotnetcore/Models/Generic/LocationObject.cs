using System.Text.Json.Serialization;

namespace SampleWebhookService.Models.Generic;

public class LocationObject
{
    [JsonPropertyName("latitude")] public double Latitude { get; set; }

    [JsonPropertyName("longitude")] public double Longitude { get; set; }
}