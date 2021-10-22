const fetch = require("node-fetch");
const { throwIfNotOk } = require("./response-error");

const createCart = async (headers) => {
  console.log("Create cart...");

  const body = {
    items: [
      {
        sku: "SKU001",
        name: "Red T-Shirt Large",
        vat: 0,
        quantity: 1,
        price: 19900,
      },
    ],
  };

  const url = "https://sandbox.urb-it.com/v2/carts";

  const response = await fetch(url, {
    method: "post",
    body: JSON.stringify(body),
    headers: headers,
  });

  await throwIfNotOk(response);

  const cartReference = (await response.json()).id;

  console.log("> Done! Cart Reference:", cartReference, "\n");
  return cartReference;
};

module.exports = createCart;
