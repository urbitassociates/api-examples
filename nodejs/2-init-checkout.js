const fetch = require("node-fetch");
const { throwIfNotOk } = require("./response-error");

const initCheckout = async (headers, cartReference) => {
  console.log("Init checkout...");

  const body = {
    cart_reference: cartReference
  };

  const url = "https://sandbox.urb-it.com/v2/checkouts";

  const response = await fetch(url, {
    method: "post",
    body: JSON.stringify(body),
    headers: headers
  });

  throwIfNotOk(response);

  const checkoutId = (await response.json()).id;

  console.log("> Done! Checkout Id", checkoutId, "\n");
  return checkoutId;
};

module.exports = initCheckout;
