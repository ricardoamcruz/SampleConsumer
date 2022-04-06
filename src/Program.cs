using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace Consumer
{
    class Program
    {
        static void Main(string[] args)
        {
            string baseUri = "http://localhost:9000";

            Console.WriteLine("Fetching response");
            var consumer = new ProductClient();
            HttpStatusCode result = consumer.GetResponse(baseUri + "/Responses/WithStatusCode200").GetAwaiter().GetResult();
            Console.WriteLine(result);
        }
    }
}
