using Consumer;
using FluentAssertions;
using PactNet.Matchers.Type;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using System.Collections.Generic;
using System.Net;
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
        public async void ReceivesBodyWithString()
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
            result.Should().NotBeNull();
            result.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void ReceivesTestBodyTrue()
        {
            // Arrange
            _mockProviderService.Given("/Responses/WithTestStatus")
                .UponReceiving("A GET request to /Responses/WithTestStatus")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/Responses/WithTestStatus",
                    Body = new MinTypeMatcher(new
                    {
                        valor = true
                    }, 1),
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 200
                });

            // Act
            var consumer = new ProductClient();
            HttpStatusCode result = await consumer.GetResponse(_mockProviderServiceBaseUri + "/Responses/WithTestStatus");

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public async void ReceivesTestBodyFalse()
        {
            // Arrange
            _mockProviderService.Given("/Responses/WithTestStatus")
                .UponReceiving("A GET request to /Responses/WithTestStatus")
                .With(new ProviderServiceRequest
                {
                    Method = HttpVerb.Get,
                    Path = "/Responses/WithTestStatus",
                    Body = new MinTypeMatcher(new
                    {
                        valor = false
                    }, 1),
                    Headers = new Dictionary<string, object>
                    {
                        { "Content-Type", "application/json" }
                    }
                })
                .WillRespondWith(new ProviderServiceResponse
                {
                    Status = 204
                });

            // Act
            var consumer = new ProductClient();
            HttpStatusCode result = await consumer.GetResponse(_mockProviderServiceBaseUri + "/Responses/WithTestStatus");

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(HttpStatusCode.OK);
        }
    }
}