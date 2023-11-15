using Microsoft.AspNetCore.Mvc;
using SampleWebhookService.Models.Delivered;
using SampleWebhookService.Models.Failed;

namespace SampleWebhookService.Controllers;

[ApiController]
public class UrbitWebhooksController : ControllerBase
{
    [HttpGet]
    [Route("/")]
    public ContentResult Index()
    {
        return Content("hello world");
    }

    [HttpPost]
    [Route("delivered")]
    public ActionResult DeliveredEvent([FromBody] DeliveredEvent deliveredEvent)
    {
        var trackingNumber = deliveredEvent.Event.TrackingNumber;
        var method = deliveredEvent.DeliveredData.Method;
        var time = deliveredEvent.Event.Timestamp;
        var latitude = deliveredEvent.DeliveredData.Location.Latitude;
        var longitude = deliveredEvent.DeliveredData.Location.Longitude;

        Console.WriteLine(
            $"{trackingNumber} was delivered at {time} using {method} (location: {latitude}, {longitude})");

        return Ok();
    }

    [HttpPost]
    [Route("failed")]
    public ActionResult FailedEvent([FromBody] FailedEvent failedEvent)
    {
        var trackingNumber = failedEvent.Event.TrackingNumber;
        var reason = failedEvent.Data.Reason;
        var time = failedEvent.Event.Timestamp;

        Console.WriteLine($"{trackingNumber} was failed on {time} due to {reason}");

        return Ok();
    }
}