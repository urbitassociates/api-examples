import createShipment from "./shipment.js";
import getShippingLabel from "./labels.js";
import dotenv from "dotenv";

const init = async () => {
  const authorization = process.env.AUTHORIZATION;
  const clientId = process.env.CLIENT_ID;

  console.log("\nCreate delivery 📦");
  var shipment = await createShipment(authorization, clientId);
  var shipmentNumber = shipment.shipment_number;
  var trackingNumber = shipment.deliveries[0].tracking_number;
  console.log(">> Done! 🌟");
  console.log(`>> shipment_number: ${shipmentNumber}`);
  console.log(`>> tracking_number: ${trackingNumber}`);

  console.log("\nGet shipping label 🏷️");
  await getShippingLabel(authorization, trackingNumber);
  console.log(">> Done! 🌟");
  console.log(">> zpl saved in current folder");

  console.log("\nBye bye");
};

(async () => {
  dotenv.config();

  if (!process.env.CLIENT_ID || !process.env.AUTHORIZATION) {
    console.error(
      "Missing .env file, please make sure you have follow the README"
    );
    return;
  }

  await init();
})();
