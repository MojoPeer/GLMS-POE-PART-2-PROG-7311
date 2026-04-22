// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Services;
using Moq;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace GLMS.Tests
{
    public class CurrencyServiceTests
    {
        [Fact]
        public async Task ConvertUsdToZarAsync_ReturnsCorrectConversion()
        {
            // Arrange
            var mockHttp = new Mock<HttpMessageHandler>();
            mockHttp.Setup(m => m.Send(It.IsAny<HttpRequestMessage>()))
                .Returns(new HttpResponseMessage
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent("{ \"rates\": { \"ZAR\": 18.5 } }")
                });

            var httpClient = new HttpClient(mockHttp.Object);
            var service = new CurrencyService(httpClient);

            // Act
            var result = await service.ConvertUsdToZarAsync(10);

            // Assert
            Assert.Equal(185, result);
        }
    }
}