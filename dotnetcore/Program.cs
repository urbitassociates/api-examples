using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace UrbitDeliveryAPI
{
    class Program
    {
        private static HttpClient client;

        static async Task Main(string[] args)
        {
            var program = new Program();
            await program.Start();

            Console.ReadKey();
        }

        public Program()
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.local.json", true, true)
                .Build();

            var authorization = configuration.GetValue<string>("authorization");
            var xApiKey = configuration.GetValue<string>("x-api-key");

            client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            client.DefaultRequestHeaders.Add("Authorization", authorization);
            client.DefaultRequestHeaders.Add("X-Api-Key", xApiKey);
        }

        public async Task Start()
        {
            Console.WriteLine("\nCreating delivery:\n");

            var cartReference = await CreateCart();

            var checkoutId = await InitCheckout(cartReference);

            var deliverySlot = await GetFirstAvailableDeliverySlot();

            await SetDelivery(checkoutId, deliverySlot);

            await PrintStatus(cartReference);

            await ShippingLabel(cartReference);

            Console.WriteLine("\nDelivery created!\n");
        }

        private async Task<Tuple<string, string>> GetFirstAvailableDeliverySlot()
        {
            Console.WriteLine("Get first available delivery slot...");

            var response = await client.GetAsync("https://sandbox.urb-it.com/v2/slots");
            var json = await GetJsonObject(response);

            var slots = json.GetValue("items").Value<JArray>();

            foreach (JObject slot in slots)
            {
                var available = slot.GetValue("available").Value<bool>();

                if (available)
                {
                    var slotStart = slot.GetValue("delivery_time").Value<DateTime>().ToString("o"); // Zulu Time
                    var slotEnds = slot.GetValue("max_delivery_time").Value<DateTime>().ToString("o"); // Zulu Time

                    Console.WriteLine($"> Done! Slot: {slotStart} - {slotEnds}\n");

                    return new Tuple<string, string>(slotStart, slotEnds);
                }
            }

            throw new ArgumentException("Error, can't find any slots. Contact Urb-it");
        }

        private async Task<Guid> CreateCart()
        {
            Console.WriteLine("Create cart...");

            var data = new
            {
                items = new[]
                {
                    new
                    {
                        sku = "sku-cnt1",
                        name = "Parcel, 10x50x20, 0.5kg", // Length x Width x Height in cm, weight in kg
                        vat = 0,
                        price = 0,
                        quantity = 1
                    }
                },
                pickup_location = new
                {
                    name = "Partner name",
                    address_1 = "1 Avenue Rapp",
                    address_2 = "Optional extra address information",
                    city = "Paris",
                    postcode = "75007",
                    phone_number = "+330000000",
                    message = "Hints that will make the pickup easier"
                },
                order_reference_id = "optional-partner-reference-to-package"
            };

            var body = GetObjectAsJsonString(data);
            var response = await client.PostAsync("https://sandbox.urb-it.com/v2/carts", body);

            if (!response.IsSuccessStatusCode) throw new ArgumentException("Error! " + response.StatusCode);

            var json = await GetJsonObject(response);
            var cartReference = Guid.Parse(json.GetValue("id").Value<string>());

            Console.WriteLine($"> Done! Cart Reference: {cartReference}\n");
            return cartReference;
        }

        private async Task<Guid> InitCheckout(Guid cartReference)
        {
            Console.WriteLine("Init checkout...");

            var data = new
            {
                cart_reference = cartReference
            };

            var body = GetObjectAsJsonString(data);
            var response = await client.PostAsync("https://sandbox.urb-it.com/v3/checkouts", body);

            if (!response.IsSuccessStatusCode) throw new ArgumentException("Error! " + response.StatusCode);

            var json = await GetJsonObject(response);
            var checkoutId = Guid.Parse(json.GetValue("id").Value<string>());

            Console.WriteLine($"> Done! Checkout Id: {checkoutId}\n");
            return checkoutId;
        }

        private async Task SetDelivery(Guid checkoutId, Tuple<string, string> deliverySlot)
        {
            Console.WriteLine("Set delivery");

            var slotStart = deliverySlot.Item1;
            var slotEnds = deliverySlot.Item2;

            var data = new
            {
                delivery_time = slotStart,
                max_delivery_time = slotEnds,
                message = "This is an example message.",
                recipient = new
                {
                    first_name = "Firstname",
                    last_name = "Lastname",
                    address_1 = "29 Avenue Rapp",
                    city = "Paris",
                    postcode = "75007",
                    phone_number = "+46000000000",
                    email = "no-reply@urbit.com"
                }
            };

            var body = GetObjectAsJsonString(data);
            var response =
                await client.PutAsync($"https://sandbox.urb-it.com/v3/checkouts/{checkoutId}/delivery", body);

            if (!response.IsSuccessStatusCode) throw new ArgumentException("Error! " + response.StatusCode);

            var trackingNumber = response.Headers.GetValues("X-Tracking-Number").FirstOrDefault();
            Console.WriteLine($"  Urb-it Tracking Number: {trackingNumber}\n");

            Console.WriteLine("> Done!\n");
        }

        private async Task PrintStatus(Guid cartReference)
        {
            Console.WriteLine("Get delivery status...");

            var response =
                await client.GetAsync($"https://sandbox.urb-it.com/v3/checkouts/{cartReference}");

            if (!response.IsSuccessStatusCode) throw new ArgumentException("Error! " + response.StatusCode);

            var json = await GetJsonObject(response);
            var status = json.GetValue("status").Value<string>();
            var trackingNumber = json.GetValue("tracking_number").Value<string>();

            Console.WriteLine("> Done!");

            Console.WriteLine($"  Urb-it Tracking Number: {trackingNumber}");
            Console.WriteLine($"  Status: {status}\n");
        }

        private async Task ShippingLabel(Guid cartReference)
        {
            Console.WriteLine("Get shipping label...");
            
            for (int numberOfTries = 0; numberOfTries < 3; numberOfTries++)
            {
                var response =
                    await client.GetAsync($"https://sandbox.urb-it.com/v3/checkouts/{cartReference}/shipping-label");

                if (response.StatusCode == HttpStatusCode.NotFound)
                {
                    // The label process is async, sometimes it's a couple of seconds delay before the delivery is ready
                    Console.WriteLine("> Trying again");
                    Thread.Sleep(2000);
                    continue;
                }

                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Error!" + response.StatusCode);
                }

                string zpl = await response.Content.ReadAsStringAsync();

                Console.WriteLine("> Done!");
                Console.WriteLine($"  ZPL: {zpl.Substring(0, 3)}...");
                return;
            }

            throw new ArgumentException("Error! Can't get shipping label. Contact Urb-it support");
        }

        private static async Task<JObject> GetJsonObject(HttpResponseMessage response)
        {
            string responseBody = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseBody);
        }

        private static StringContent GetObjectAsJsonString(object data)
        {
            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }
    }
}
