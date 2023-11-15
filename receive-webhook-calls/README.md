# Sample project on how to receive webhook calls from Urb-it

Please see [https://urb-it.dev/docs/v4/](https://urb-it.dev/docs/v4/) for more details on events and payloads

URLs:

- https://localhost:8081/delivered
- https://localhost:8081/failed

Byt using Postman, Insomnia or similar tools, you can easily push test payload agains your server, pretending that it's Urb-it sending the payload.

## Guide to Receiving Webhook Calls from Urb-it

For an in-depth understanding of Urb-it's events and payloads, refer to their official documentation at [Urb-it Developer Docs](https://urb-it.dev/docs/v4/).

### URLs:

- To handle the "DELIVERED_EVENT": `https://localhost:8081/delivered`
- To handle the "FAILURE_EVENT":: `https://localhost:8081/failed`

### Testing:

To simulate receiving webhook calls from Urb-it, you can use tools like Postman or Insomnia with they payload found in API documentation. These tools allow you to send test payloads to your server, emulating the behavior of Urb-it's webhook service.

1. Update Postman/Insomnia with the URL and payload.
1. Set the request method to `POST`.
1. Observe how the test service.

| Delivered                                                                                                    | Failed                                                                                                       |
| ------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------------------------------------------ |
| ![image](https://github.com/urbitassociates/api-examples/assets/351045/bdce8e54-46ed-458b-b4da-c7c1f8fa5a99) | ![image](https://github.com/urbitassociates/api-examples/assets/351045/01194bd9-6bc6-4e3f-a55f-717386d99fdc) |

Output
![image](https://github.com/urbitassociates/api-examples/assets/351045/3c1c44c3-4f4b-4ac4-8411-7b3877a5e4d4)
