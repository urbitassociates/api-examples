# Urb-it Delivery examples:

API docs: [https://urb-it.dev/docs/v4/](https://urb-it.dev/docs/v4/)

**Important:**
You need to get `authorization` and `clientId` from urb-it.

The sample code will:

1. Create a shipment with one delivery
2. Fetch the shipping label for the created shipment

## Postman collection

The [Urb-it Delivery API](./postman/Urb-it_Delivery_API.json) collection can be used to:

- Create a Shipment
- Retrieve a Shipment
- Cancel a Shipment
- Create the webhook configuration
- Retrieve the webhook configuration
- Validate a postcode

Before executing any of the above endpoints, make sure to set the variables `Authorization` and `ClientId` on Postman.
