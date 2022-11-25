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
        private readonly Mock<IRepository<Url>> _repositoryStub = new();

        [Fact]
        public async Task GetUrlByIdAsync_WithUnexistingUrl_ReturnsNotFound()
        {
            _repositoryStub
                .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Url)null);

            var controller = new UrlController(_repositoryStub.Object);

            var result = await controller.GetUrlByIdAsync(Guid.NewGuid());

            result.Result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetUrlByIdAsync_WithExistingUrl_ReturnsExpectedUrl()
        {
            var expectedUrl = CreateTestUrl();

            _repositoryStub
                .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expectedUrl);

            var controller = new UrlController(_repositoryStub.Object);

            var result = await controller.GetUrlByIdAsync(Guid.NewGuid());
            var okObject = (OkObjectResult)result.Value;
            result.Should().BeEquivalentTo (
                expectedUrl, 
                options => options.ComparingByMembers<Url>()
            );
        }

        [Fact]
        public async void GetUrlsAsync_WithExistingUrls_ReturnsAllUrls()
        {
            var urls = new List<Url>()
            {
                CreateTestUrl(),
                CreateTestUrl(),
                CreateTestUrl()
            };

            _repositoryStub
                .Setup(_ => _.GetAllAsync())
                .ReturnsAsync(urls);

            var controller = new UrlController(_repositoryStub.Object);

            var result = await controller.GetUrlsAsync();

            result.Value.Should().BeEquivalentTo(urls, options =>
            {
                return options.ComparingByMembers<Url>();
            });
        }

        //[Fact]
        //public async Task PostAsync_WithUrlToCreate_ReturnsCreatedAtAction()
        //{
        //    var urlToCreate = new UrlCreateDto()
        //    {
        //        LongUrl = "Fake Long URL"
        //    };

        //    var controller = new UrlController(_repositoryStub.Object);

        //    var result = await controller.PostAsync(urlToCreate);

        //    var createdUrl = (result.Result as CreatedAtActionResult).Value as dto
        //}

        private static Url CreateTestUrl()
        {
            return new Url
            {
                Id = Guid.NewGuid(),
                ShortUrl = "Test Short URL",
                LongUrl = "Test Long URL",
                ClassifiedAs = "Phising",
                CreationDate = DateTime.UtcNow
            };
        }
    }
}