# Urb-it Delivery examples:

API docs: [https://urb-it.dev/docs/v4/](https://urb-it.dev/docs/v4/)

**Important:**
You need to get `authorization` and `clientId` from urb-it.

The sample code will:

1. Create a shipment with one delivery
2. Fetch the shipping label for the created shipment

## [Postman](https://www.postman.com/) collection

The [Urb-it Delivery API](./postman/Urb-it_Delivery_API.json) postman collection has sample requests for the following actions:

- Create a shipment
- Retrieve a shipment
- Get shipping label
- Cancel a shipment
- Create a webhook configuration
- Retrieve the webhook configuration
- Validate a postcode

![Postman Collection](https://user-images.githubusercontent.com/351045/225658246-294d68e0-ad4c-47dc-9149-db574eab5f7d.png)

Before making requests to any of the above endpoints, make sure to set the variables `Authorization` and `ClientId` in Postman [Environment](https://learning.postman.com/docs/sending-requests/managing-environments/).
