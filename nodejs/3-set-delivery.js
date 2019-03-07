const fetch = require("node-fetch");
const { throwIfNotOk } = require("./response-error");

const setDelivery = async (headers, checkoutReference) => {
  console.log("Set delivery");

  const time = addMinutes(new Date(), 180).toISOString();

  const body = {
    delivery_time: time,
    message: "This is an example message.",
    recipient: {
      first_name: "Firstname",
      last_name: "Lastname",
      address_1: "29 Avenue Rapp",
      city: "Paris",
      postcode: "75007",
      phone_number: "+46000000000",
      email: "no-reply@urbit.com"
    }
  };

  const url = `https://sandbox.urb-it.com/v2/checkouts/${checkoutReference}/delivery`;

  const response = await fetch(url, {
    method: "put",
    body: JSON.stringify(body),
    headers: headers
  });

  throwIfNotOk(response);

  console.log("> Done!\n");

  return body;
};

const addMinutes = (date, minutes) =>
  new Date(date.getTime() + minutes * 60000);

module.exports = setDelivery;
