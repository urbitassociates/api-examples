using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace DeliveryApiExample
{
    public static class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Starting example ...");

            Console.WriteLine(CallUrbitApi().Result);

            Console.WriteLine("Press enter to close ...");
            Console.ReadLine();
        }

        public static async Task<string> CallUrbitApi()
        {
            var data = new
            {
                items = new[] {
                    new {
                        sku= "sku-cnt1",
                        name= "TestProdcut1",
                        image= "https://picsum.photos/200/200",
                        vat= 2000,
                        price= 3000,
                        quantity= 5
                    }
                }
            };

            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(ConfigurationManager.AppSettings["Urbit.DeliveryApi.BaseUrl"]);
            client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    "Bearer", 
                    ConfigurationManager.AppSettings["Urbit.Retailer.Authorization"]
                );
            client.DefaultRequestHeaders.Add("X-API-Key", ConfigurationManager.AppSettings["Urbit.Retailer.ApiKey"]);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, "v2/carts");
            request.Content = new StringContent(Newtonsoft.Json.JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

            return await client.SendAsync(request)
                .ContinueWith(responseTask =>
                {
                    Console.WriteLine("Response: {0}", responseTask.Result);
                    return responseTask.Result.Content.ReadAsStringAsync().Result.ToString();
                });
        }
    }
}
