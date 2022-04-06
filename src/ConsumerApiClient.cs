using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Consumer
{
    public class ProductClient
    {
        #nullable enable
        public async Task<HttpStatusCode> GetResponse(string baseUrl, HttpClient? httpClient = null)
        {
            using var client = httpClient ?? new HttpClient();

            var response = await client.GetAsync(baseUrl);
            response.EnsureSuccessStatusCode();

            HttpStatusCode resp = response.StatusCode;
            //var resp = await response.Content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<List<Product>>(resp);
            return resp;
        }
    }
}