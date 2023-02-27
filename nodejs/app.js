import createShipment from "./shipment.js";
import saveShippingLabel from "./labels.js";
import dotenv from "dotenv";

dotenv.config();

if (!process.env.CLIENT_ID || !process.env.AUTHORIZATION) {
  throw new Error(
    "Missing .env file, please make sure you have follow the README"
  );
}

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
await saveShippingLabel(authorization, trackingNumber);
console.log(">> Done! 🌟");
console.log(">> zpl saved in current folder");

console.log("\nBye bye");
