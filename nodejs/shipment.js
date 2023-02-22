import fetch from "node-fetch";

const createShipment = async (authorization, clientId) => {
  var payload = {
    client_id: clientId,
    service_type: "NEXT_DAY_DELIVERY",
    reference_id: {
      description: "Order Id",
      data: "1653920631",
    },

    deliveries: [
      {
        weight: { unit: "g", value: 550 },
        dimensions: {
          height: { unit: "cm", value: 10 },
          width: { unit: "cm", value: 15 },
          length: { unit: "cm", value: 20 },
        },
        reference_id: {
          description: "Parcel Id",
          data: "132213213213",
        },
      },
    ],

    origin: {
      address: {
        address_1: "132 Commercial Street",
        postcode: "E1 6AZ",
        city: "London",
        country_code: "GB",
      },
      contact: {
        name: "John Doe",
        phone_number: "+46700000000",
        email: "John.Doe@example.org",
      },
    },

    destination: {
      address: {
        name: "Acme Corp",
        address_1: "6 Fairclough Street",
        postcode: "E1 1PW",
        city: "London",
        country_code: "GB",
      },
      contact: {
        name: "Jane Doe",
        phone_number: "+46700000000",
        email: "Jane.Doe@example.org",
      },
      instructions: {
        notes: "Lipsum",
        door_code: "1234",
      },
    },
  };

  const url = `https://sandbox.urb-it.com/v4/shipments`;

  const headers = {
    Authorization: authorization,
    "Content-Type": "application/json",
  };

  const response = await fetch(url, {
    method: "POST",
    body: JSON.stringify(payload),
    headers: headers,
  });

  if (!response.ok) throw "Noooo!!!";

  return await response.json();
};

export default createShipment;
