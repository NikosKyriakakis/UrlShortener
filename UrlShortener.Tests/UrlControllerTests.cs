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

        //[Fact]
        //public async Task GetUrlByIdAsync_WithExistingUrl_ReturnsExpectedUrl()
        //{
        //    var expectedUrl = CreateTestUrl();
        //    var expectedDto = expectedUrl.AsReadDto();

        //    _repositoryStub
        //        .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
        //        .ReturnsAsync(expectedUrl);

        //    var controller = new UrlController(_repositoryStub.Object);

        //    var result = await controller.GetUrlByIdAsync(Guid.NewGuid());

        //    result.Value.Should().BeEquivalentTo(expectedDto, options =>
        //    {
        //        return options.ComparingByMembers<UrlReadDto>();
        //    });
        //}

        //[Fact]
        //public async void GetUrlsAsync_WithExistingUrls_ReturnsExpectedUrls()
        //{
        //    var urls = new List<Url>()
        //    {
        //        CreateTestUrl(),
        //        CreateTestUrl(),
        //        CreateTestUrl()
        //    };

        //    var dtos = new List<UrlReadDto>()
        //    {
        //        urls.First().AsReadDto(),
        //        urls.First().AsReadDto(),
        //        urls.First().AsReadDto()
        //    };

        //    _repositoryStub.Setup(repo => repo.GetAllAsync())
        //        .ReturnsAsync(urls);

        //    var controller = new UrlController(_repositoryStub.Object);

        //    var result = await controller.GetUrlsAsync();

        //    result.Value.Should().BeEquivalentTo(dtos, options =>
        //    {
        //        return options.ComparingByMembers<UrlReadDto>();
        //    });
        //}

        //private static Url CreateTestUrl()
        //{
        //    return new Url
        //    {
        //        Id = Guid.NewGuid(),
        //        ShortUrl = "Test Short URL",
        //        LongUrl = "Test Long URL",
        //        ClassifiedAs = "Phising",
        //        CreationDate = DateTime.UtcNow
        //    };
        //}
    }
}