using System.Net.Http.Headers;
using System.Text;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Sample
{
    class Program
    {
        private static HttpClient httpClient;
        private readonly string clientId;

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

            // This token you will get from Urb-it
            var authorization = configuration.GetValue<string>("authorization");

            httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            httpClient.DefaultRequestHeaders.Add("Authorization", authorization);
            
            // This id you will get from Urb-it
            clientId = configuration.GetValue<string>("clientId");
        }

        public async Task Start()
        {
            Console.WriteLine("\nCreate delivery");
            var shipmentInformation = await CreateShipment();
            var shipmentNumber = shipmentInformation.Item1;
            var trackingNumber = shipmentInformation.Item2[0];
            Console.WriteLine(">> Done!");
            Console.WriteLine($">> shipment_number: {shipmentNumber}");
            Console.WriteLine($">> tracking_number: {trackingNumber}");

            Console.WriteLine("\nGet shipping label...");
            await ShippingLabel(trackingNumber);
            Console.WriteLine(">> Done!");
            Console.WriteLine(">> Pdf saved in current folder");

            Console.WriteLine("\nDone");
        }

        private async Task<Tuple<string, string[]>> CreateShipment()
        {
            // https://urb-it.dev/docs/v4/7d4fb4abf29e2-create-shipment-with-deliveries
            var data = new
            {
                client_id = clientId,
                service_type = "NEXT_DAY_DELIVERY",
                reference_id = new
                {
                    description = "Order Id",
                    data = "1653920631",
                },

                deliveries = new[]
                {
                    new
                    {
                        weight = new { unit = "g", value = 550 },
                        dimensions = new
                        {
                            height = new { unit = "cm", value = 10 },
                            width = new { unit = "cm", value = 15 },
                            length = new { unit = "cm", value = 20 }
                        },
                        reference_id = new
                        {
                            description = "Parcel Id",
                            data = "132213213213"
                        }
                    }
                },

                origin = new
                {
                    address = new
                    {
                        address_1 = "132 Commercial Street",
                        postcode = "E1 6AZ",
                        city = "London",
                        country_code = "GB",
                    },
                    contact = new
                    {
                        name = "John Doe",
                        phone_number = "+46700000000",
                        email = "John.Doe@example.org"
                    }
                },

                destination = new
                {
                    address = new
                    {
                        name = "Acme Corp",
                        address_1 = "6 Fairclough Street",
                        postcode = "E1 1PW",
                        city = "London",
                        country_code = "GB",
                    },
                    contact = new
                    {
                        name = "Jane Doe",
                        phone_number = "+46700000000",
                        email = "Jane.Doe@example.org"
                    },
                    instructions = new
                    {
                        notes = "Lipsum",
                        door_code = "1234"
                    }
                },
            };
            
            var body = GetObjectAsJsonString(data);
            var response = await httpClient.PostAsync("https://sandbox.urb-it.com/v4/shipments", body);

            if (!response.IsSuccessStatusCode) throw new ArgumentException("Error! " + response.StatusCode);

            // Note: Instead of JObject one could easy deserialize into classes/objects
            var json = await GetJsonObject(response);
            var shipmentNumber = json.GetValue("shipment_number").Value<string>();
            var deliveries = json.GetValue("deliveries").Value<JArray>();
            var trackingNumber = deliveries[0].Value<JObject>().GetValue("tracking_number").Value<string>();

            return new (shipmentNumber, new[] { trackingNumber });
        }

        private async Task ShippingLabel(string trackingNumber)
        {
            // https://urb-it.dev/docs/v4/51a1dcc0a0888-fetch-shipping-label-for-delivery
            var response =
                await httpClient.GetAsync($"https://sandbox.urb-it.com/v4/deliveries/{trackingNumber}/shipping-label");

            if (!response.IsSuccessStatusCode) throw new ArgumentException("Error! " + response.StatusCode);

            var output = await response.Content.ReadAsByteArrayAsync();
            var path = trackingNumber + ".pdf";
            await File.WriteAllBytesAsync(path, output);
        }

        private static async Task<JObject> GetJsonObject(HttpResponseMessage response)
        {
            var responseBody = await response.Content.ReadAsStringAsync();
            return JObject.Parse(responseBody);
        }

        private static StringContent GetObjectAsJsonString(object data)
        {
            var payload = Newtonsoft.Json.JsonConvert.SerializeObject(data);
            return new StringContent(payload, Encoding.UTF8, "application/json");
        }
    }
}
