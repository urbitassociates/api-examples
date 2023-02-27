import fetch from "node-fetch";
import fs from "fs";

const saveShippingLabel = async (authorization, trackingNumber) => {
  const url = `https://sandbox.urb-it.com/v4/deliveries/${trackingNumber}/shipping-label`;

  const headers = {
    Authorization: authorization,
  };

  const response = await fetch(url, {
    method: "GET",
    headers: headers,
  });

  if (!response.ok) throw "Noooo!!!";

  var zpl = await response.text();
  fs.writeFileSync(`./${trackingNumber}.zpl`, zpl);
};

export default saveShippingLabel;
