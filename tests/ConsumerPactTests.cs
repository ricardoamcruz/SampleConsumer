using Consumer;
using FluentAssertions;
using Newtonsoft.Json;
using PactNet.Matchers.Type;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace tests
{
    public class ConsumerPactTests : IClassFixture<ConsumerPactClassFixture>
    {
        private IMockProviderService _mockProviderService;
        private string _mockProviderServiceBaseUri;

        public ConsumerPactTests(ConsumerPactClassFixture fixture)
        {
            _mockProviderService = fixture.MockProviderService;
            _mockProviderService.ClearInteractions(); //NOTE: Clears any previously registered interactions before the test is run
            _mockProviderServiceBaseUri = fixture.MockProviderServiceBaseUri;
        }

        [Fact]
        public async void ReceivesStatusCode200()
        {
            // Arrange
            _mockProviderService.Given("/Responses/WithStatusCode200")
                .UponReceiving("A GET request to /Responses/WithStatusCode200")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/Responses/WithStatusCode200"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200
                });

            // Act
            var consumer = new ProductClient();
            HttpStatusCode result = await consumer.GetResponse(_mockProviderServiceBaseUri + "/Responses/WithStatusCode200");

            // Assert
            result.Should().NotBe(null);
            result.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void ReceivesTestQueryTrue()
        {
            // Arrange
            _mockProviderService//.Given("/Responses/WithTestStatus true")
                .UponReceiving("A GET request to /Responses/WithTestStatus with query")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/Responses/WithTestStatus",
                    Query = "valor=true"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200
                });

            // Act
            var consumer = new ProductClient();
            HttpStatusCode result = await consumer.GetResponse(_mockProviderServiceBaseUri + "/Responses/WithTestStatus?valor=true");

            // Assert
            result.Should().NotBe(null);
            result.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void ReceivesTestQueryFalse()
        {
            // Arrange
            _mockProviderService//.Given("/Responses/WithTestStatus false")
                .UponReceiving("A GET request to /Responses/WithTestStatus")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/Responses/WithTestStatus"
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 204
                });

            // Act
            var consumer = new ProductClient();
            HttpStatusCode result = await consumer.GetResponse(_mockProviderServiceBaseUri + "/Responses/WithTestStatus");

            // Assert
            result.Should().NotBe(null);
            result.Should().Be(HttpStatusCode.NoContent);
        }

        //[Fact]
        public async void ReceivesBodyWithString()
        {
            var content = new StringContent("StringContent", Encoding.UTF8, "application/json");
            var jsonString = JsonConvert.SerializeObject(content);

            // Arrange
            _mockProviderService
                .UponReceiving("A POST request to /Body/WithString")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = "/Body/WithString",
                    Body = jsonString,
                    Headers = new Dictionary<string, object>()
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Body = JsonConvert.SerializeObject("Received body parameter: AQUI"),
                    Headers = new Dictionary<string, object>()
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    }
                });

            var consumer = new ProductClient();

            //Act
            var response = await consumer.PostRequest(_mockProviderServiceBaseUri + "/Body/WithString", content);
            //var response = await consumer.PostRequest(_mockProviderServiceBaseUri + "/Body/WithString", content);

            var statusCode = response.StatusCode;
            var responseMessage = await response.Content.ReadAsStringAsync();
            //var teste = JsonConvert.DeserializeObject<object>(responseMessage);

            // Assert
            response.Should().NotBeNull();
            statusCode.Should().Be(HttpStatusCode.OK);
            responseMessage.Should().Be("\"Received body parameter: StringContent\"");
        }

        //[Fact]
        public async void ReceivesBodyWithModel()
        {
            var model = new DummyBody()
            {
                Id = 13,
                Name = "MyName"
            };
            var json = JsonConvert.SerializeObject(model);

            // Arrange
            _mockProviderService
                .UponReceiving("A POST request to /Body/WithModel")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Post,
                    Path = "/Body/WithModel",
                    Body = model,
                    Headers = new Dictionary<string, object>()
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200,
                    Body = model,
                    Headers = new Dictionary<string, object>()
                    {
                        { "Content-Type", "application/json; charset=utf-8" }
                    }
                });

            var consumer = new ProductClient();

            //Act
            //_mockProviderServiceBaseUri = "http://localhost:9000";
            var response = await consumer.PostRequest(_mockProviderServiceBaseUri + "/Body/WithModel", model);

            var statusCode = response.StatusCode;
            var responseMessage = await response.Content.ReadAsStringAsync();

            // Assert
            response.Should().NotBeNull();
            statusCode.Should().Be(HttpStatusCode.OK);
        }
    }


    public class DummyBody
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string[] Tags { get; set; }
        public object Inner { get; set; }
        public List<string> StringList { get; set; }
        public List<object> ObjectList { get; set; }
        public Dictionary<string, int> Dict { get; set; }
    }
}