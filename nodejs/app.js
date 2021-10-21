const createCart = require("./1-create-cart");
const initCheckout = require("./2-init-checkout");
const setDelivery = require("./3-set-delivery");
const printStatus = require("./4-print-status");

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
  require("dotenv").config();

  if (!process.env.X_API_KEY || !process.env.AUTHORIZATION) {
    console.error(
      "Missing .env file, please make sure you have follow the README"
    );
    return;
  }

  await createDelivery();
})();
