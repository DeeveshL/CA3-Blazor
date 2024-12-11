using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Moq.Protected;
using SteamAchievementApp.Services;
using System.Text.Json;

namespace SteamAchievementApp.Tests
{
    public class AchievementServiceTests
    {
        [Fact]
        public async Task Response_ReturnsAchievements()
        {
            // Arrange
            var jsonResponse = JsonSerializer.Serialize(new GlobalAchievementResponse
            {
                achievementpercentages = new AchievementPercentages
                {
                    achievements = new System.Collections.Generic.List<Achievement>
                    {
                        new Achievement { name = "Achievement1", percent = 50.5 },
                        new Achievement { name = "Achievement2", percent = 10.0 }
                    }
                }
            });

            // Mock HttpMessageHandler to return the above JSON
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    return new HttpResponseMessage(HttpStatusCode.OK)
                    {
                        Content = new StringContent(jsonResponse)
                    };
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AchievementService(httpClient);

            // Act
            var result = await service.GetGlobalAchievementPercentagesAsync("730");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.achievementpercentages);
            Assert.Equal(2, result.achievementpercentages.achievements.Count);
            Assert.Equal("Achievement1", result.achievementpercentages.achievements[0].name);
            Assert.Equal(50.5, result.achievementpercentages.achievements[0].percent);
        }

        [Fact]
        public async Task NonSuccessResponse_ThrowsException()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync((HttpRequestMessage request, CancellationToken token) =>
                {
                    return new HttpResponseMessage(HttpStatusCode.InternalServerError);
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AchievementService(httpClient);

            // Act & Assert
            await Assert.ThrowsAsync<HttpRequestException>(() => service.GetGlobalAchievementPercentagesAsync("730"));
        }

        [Fact]
        public async Task EmptyBody_ReturnsNull()
        {
            // Arrange
            var handlerMock = new Mock<HttpMessageHandler>();
            handlerMock
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StringContent("{}")
                });

            var httpClient = new HttpClient(handlerMock.Object);
            var service = new AchievementService(httpClient);

            // Act
            var result = await service.GetGlobalAchievementPercentagesAsync("730");

            // Assert
            Assert.NotNull(result);
            Assert.Null(result.achievementpercentages); // Or handle differently if needed.
        }
    }
}
