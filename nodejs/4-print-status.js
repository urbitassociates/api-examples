const fetch = require("node-fetch");
const { throwIfNotOk } = require("./response-error");

const printStatus = async (headers, cartReference) => {
  console.log("Get delivery status...");

  const url = `https://sandbox.urb-it.com/v2/checkouts/${cartReference}`;

  const response = await fetch(url, {
    method: "get",
    headers: headers
  });

  throwIfNotOk(response);

  const deliveryInformation = await response.json();

  console.log("> Done!\n");
  console.log("  Delivery time:", deliveryInformation.delivery_time);
  console.log("  Status:", deliveryInformation.status);
  console.log("  Order reference id:", deliveryInformation.order_reference_id);

  return cartReference;
};

module.exports = printStatus;
