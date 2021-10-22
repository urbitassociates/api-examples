import fetch from "node-fetch";
import { throwIfNotOk } from "./response-error.js";

const printStatus = async (headers, cartReference) => {
  console.log("Get delivery status...");

  const url = `https://sandbox.urb-it.com/v2/checkouts/${cartReference}`;

  const response = await fetch(url, {
    method: "get",
    headers: headers,
  });

  await throwIfNotOk(response);

  const deliveryInformation = await response.json();

  console.log("> Done!\n");
  console.log("  Delivery time:", deliveryInformation.delivery_time);
  console.log("  Status:", deliveryInformation.status);
  console.log("  Order reference id:", deliveryInformation.order_reference_id);
};

export default printStatus;
