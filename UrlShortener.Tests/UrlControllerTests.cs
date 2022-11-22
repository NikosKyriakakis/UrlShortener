using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Pattern.Generic;
using UrlShortener.Service;
using UrlShortener.Service.Controllers;
using UrlShortener.Service.Dtos;
using UrlShortener.Service.Models;

namespace UrlShortener.Tests
{
    public class UrlControllerTests
    {
        private readonly Mock<IRepository<Url>> _repositoryStub = new(MockBehavior.Strict);

        [Fact]
        public async Task GetUrlByIdAsync_WithUnexistingUrl_ReturnsNotFound()
        {
            _repositoryStub.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Url)null);

            var controller = new UrlController(_repositoryStub.Object);

            var result = await controller.GetUrlByIdAsync(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetUrlByIdAsync_WithExistingUrl_ReturnsExpectedUrl()
        {
            Url expectedUrl = CreateTestUrl();

            _repositoryStub.Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync(expectedUrl);

            UrlController controller = new(_repositoryStub.Object);

            ActionResult<UrlReadDto> result = await controller.GetUrlByIdAsync(expectedUrl.Id);
            UrlReadDto dto = result.Value;

            result.Value.Should().BeEquivalentTo(expectedUrl, options =>
            {
                return options.ComparingByMembers<Url>();
            });
        }

        [Fact]
        public async void GetUrlsAsync_WithExistingUrls_ReturnsExpectedUrls()
        {
            IReadOnlyCollection<Url> urls = new List<Url>()
            {
                CreateTestUrl(),
                CreateTestUrl(),
                CreateTestUrl()
            };

            IEnumerable<UrlReadDto> dtos = new List<UrlReadDto>()
            {
                urls.First().AsReadDto(),
                urls.First().AsReadDto(),
                urls.First().AsReadDto()
            };

            _repositoryStub.Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(urls);

            var controller = new UrlController(_repositoryStub.Object);

            var result = await controller.GetUrlsAsync();

            result.Value.Should().BeEquivalentTo(dtos, options =>
            {
                return options.ComparingByMembers<UrlReadDto>();
            });
        }

        private static Url CreateTestUrl()
        {
            return new Url
            {
                Id = Guid.NewGuid(),
                ShortUrl = "Test Short URL",
                LongUrl = "Test Long URL"
            };
        }
    }
}