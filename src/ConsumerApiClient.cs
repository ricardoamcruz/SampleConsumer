using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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
            //response.EnsureSuccessStatusCode();

            HttpStatusCode resp = response.StatusCode;
            //var resp = await response.Content.ReadAsStringAsync();
            //return JsonConvert.DeserializeObject<List<Product>>(resp);
            return resp;
        }

        public async Task<HttpResponseMessage> PostRequest(string baseUrl, HttpContent content, HttpClient? httpClient = null)
        {
            using var client = httpClient ?? new HttpClient();

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");//;odata=verbose

            return await client.PostAsync(baseUrl, content);

        }

        public async Task<HttpResponseMessage> PostRequest(string baseUrl, object content, HttpClient? httpClient = null)
        {
            using var client = httpClient ?? new HttpClient();
            using (var request = new HttpRequestMessage(HttpMethod.Post, baseUrl))
            {
                var json = JsonConvert.SerializeObject(content);
                using (var stringContent = new StringContent(json, Encoding.UTF8, "application/json"))
                {
                    request.Content = stringContent;

                    return await client
                        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead)
                        .ConfigureAwait(true);
                }
            }
        }
    }
}