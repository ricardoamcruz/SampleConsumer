using System;
using Xunit;
using PactNet.Mocks.MockHttpService;
using PactNet.Mocks.MockHttpService.Models;
using Consumer;
using System.Collections.Generic;
using PactNet.Matchers.Type;
using FluentAssertions;
using System.Net;

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
            _mockProviderService.UponReceiving("A GET request to /Responses/WithStatusCode200")
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
            HttpStatusCode result = await consumer.GetResponse(_mockProviderServiceBaseUri);

            // Assert
            result.Should().NotBeNull();
            result.Should().Be(HttpStatusCode.OK);
        }
    }
}
