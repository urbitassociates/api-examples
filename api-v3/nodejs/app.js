import createCart from "./1-create-cart.js";
import initCheckout from "./2-init-checkout.js";
import setDelivery from "./3-set-delivery.js";
import printStatus from "./4-print-status.js";
import dotenv from "dotenv";

const createDelivery = async () => {
  console.log("\nCreating delivery 📦:\n");

  const headers = {
    "X-Api-Key": process.env.X_API_KEY,
    Authorization: process.env.AUTHORIZATION,
    "Content-Type": "application/json",
  };

  console.log("Headers", headers, "\n");

  // 1. Create cart - https://developer.urb-it.com/#operation/createCart
  const cartReference = await createCart(headers);

  // 2. Initiate checkout - https://developer.urb-it.com/#operation/createCheckout
  const checkoutReference = await initCheckout(headers, cartReference);

  // 3. Set delivery - https://developer.urb-it.com/#operation/updateDelivery
  await setDelivery(headers, checkoutReference);

  // Optional. Get status - https://developer.urb-it.com/#operation/getCheckout
  await printStatus(headers, checkoutReference);

  console.log("\nDelivery created 🌟!\n");
};

(async () => {
  dotenv.config();

  if (!process.env.X_API_KEY || !process.env.AUTHORIZATION) {
    console.error(
      "Missing .env file, please make sure you have follow the README"
    );
    return;
  }

  await createDelivery();
})();
