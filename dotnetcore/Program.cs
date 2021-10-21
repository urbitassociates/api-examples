using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

            await SetDelivery(checkoutId);

            await PrintStatus(cartReference);

            Console.WriteLine("\nDelivery created!\n");
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
                        name = "TestProdcut1",
                        image = "https://picsum.photos/200/200",
                        vat = 2000,
                        price = 3000,
                        quantity = 5
                    }
                }
            };

            var body = GetObjectAsJsonString(data);
            var response = await client.PostAsync("https://sandbox.urb-it.com/v2/carts", body);

            var json = await GetJsonObject(response);
            var cartReference = Guid.Parse(json.SelectToken("id").Value<string>());

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
            var response = await client.PostAsync("https://sandbox.urb-it.com/v2/checkouts", body);

            var json = await GetJsonObject(response);
            var checkoutId = Guid.Parse(json.SelectToken("id").Value<string>());

            Console.WriteLine($"> Done! Checkout Id: {checkoutId}\n");
            return checkoutId;
        }

        private async Task SetDelivery(Guid checkoutId)
        {
            Console.WriteLine("Set delivery");

            var slotStart = DateTime.Now.AddDays(7).ToString("yyy-MM-dd") + "T04:00:00Z";
            var slotEnds = DateTime.Now.AddDays(7).ToString("yyy-MM-dd") + "T05:00:00Z";

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
                await client.PutAsync($"https://sandbox.urb-it.com/v2/checkouts/{checkoutId}/delivery", body);

            Console.WriteLine("> Done!\n");
        }

        private async Task PrintStatus(Guid cartReference)
        {
            Console.WriteLine("Get delivery status...");
            
            var response =
                await client.GetAsync($"https://sandbox.urb-it.com/v2/checkouts/{cartReference}");
            if (!response.IsSuccessStatusCode)
            {
                string errorBody = await response.Content.ReadAsStringAsync();
                throw new ArgumentException(errorBody);
            }

            var json = await GetJsonObject(response);
            var deliveryTime = json.SelectToken("delivery_time").Value<string>();
            var status = json.SelectToken("status").Value<string>();
            var orderReferenceId = json.SelectToken("order_reference_id").Value<string>();

            Console.WriteLine("> Done!");

            Console.WriteLine($"  Delivery time: {deliveryTime}");
            Console.WriteLine($"  Status: {status}");
            Console.WriteLine($"  Order reference id: {orderReferenceId}\n");
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
