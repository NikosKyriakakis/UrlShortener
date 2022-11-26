using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Repository.Pattern.Generic;
using System.Linq.Expressions;
using UrlShortener.Service;
using UrlShortener.Service.Controllers;
using UrlShortener.Service.Dtos;
using UrlShortener.Service.Models;

namespace UrlShortener.Tests
{
    public class UrlControllerTests
    {
        private readonly Mock<IRepository<Url>> _mockRepository = new();

        [Fact]
        public async Task GetByIdAsync_WithoutExistingObject_ReturnsNotFound()
        {
            _mockRepository
                .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Url)null);

            var controller = new UrlController(_mockRepository.Object);

            var result = (NotFoundResult)await controller.GetUrlByIdAsync(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetByIdAsync_WithExistingObject_ReturnsExpectedObject()
        {
            var expected = CreateFakeUrl();

            _mockRepository
                .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expected);

            var controller = new UrlController(_mockRepository.Object);

            var result = (OkObjectResult)await controller.GetUrlByIdAsync(Guid.NewGuid());
            result.Value.Should()
                .BeEquivalentTo(expected.AsReadDto(), options => options.ComparingByMembers<UrlReadDto>());
        }

        [Fact]
        public async Task GetUrlsAsync_WithExistingObjects_ReturnsAllObjects()
        {
            var expectedUrls = new List<Url>()
            {
                CreateFakeUrl(),
                CreateFakeUrl(),
                CreateFakeUrl()
            };

            var expectedDtos = expectedUrls
                .Select(e => e.AsReadDto())
                .ToList();

            _mockRepository
                .Setup(_ => _.GetAllAsync())
                .ReturnsAsync(expectedUrls);

            var controller = new UrlController(_mockRepository.Object);

            var result = (OkObjectResult)await controller.GetUrlsAsync();

            result.Value.Should()
                .BeEquivalentTo(expectedDtos, options => options.ComparingByMembers<UrlReadDto>());
        }

        [Fact]
        public async Task GetUrlByShort_WithValidObject_ReturnsExpectedObject()
        {
            var expectedUrls = new List<Url>()
            {
                 CreateFakeUrl(),
                 CreateFakeUrl()
            };

            _mockRepository
                .Setup(_ => _.GetAllAsync(It.IsAny<Expression<Func<Url, bool>>>()))
                .ReturnsAsync((IReadOnlyCollection<Url>)expectedUrls);

            var controller = new UrlController(_mockRepository.Object);

            var result = (OkObjectResult)await controller.GetUrlByShort("test");

            var expectedDto = expectedUrls.First().AsReadDto();

            result.Value.Should().BeEquivalentTo(expectedDto, options => options.ComparingByMembers<UrlReadDto>());
        }

        [Fact]
        public async Task GetUrlByShort_WithoutExpiredObject_ReturnsNotFound()
        {
            var expectedUrls = new List<Url>()
            {
                 CreateFakeUrl(IsExpired: true),
                 CreateFakeUrl(IsExpired: true)
            };

            _mockRepository
                .Setup(_ => _.GetAllAsync(It.IsAny<Expression<Func<Url, bool>>>()))
                .ReturnsAsync((IReadOnlyCollection<Url>)expectedUrls);

            var controller = new UrlController(_mockRepository.Object);

            var result = (NotFoundResult)await controller.GetUrlByShort("test");

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task GetUrlByShort_WithoutExistingObject_ReturnsNotFound()
        {
            _mockRepository
                .Setup(_ => _.GetAllAsync(It.IsAny<Expression<Func<Url, bool>>>()))
                .ReturnsAsync((IReadOnlyCollection<Url>)null);

            var controller = new UrlController(_mockRepository.Object);

            var result = (NotFoundResult)await controller.GetUrlByShort("test");

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task PostAsync_ValidObjectPassed_ReturnsCreatedAtAction()
        {
            var createDto = new UrlCreateDto()
            {
                LongUrl = "https://timeseriesreasoning.com/contents/poisson-process/"
            };

            var controller = new UrlController(_mockRepository.Object);

            var result = (CreatedAtActionResult)await controller.PostAsync(createDto);

            result.Should().BeOfType<CreatedAtActionResult>();
        }

        [Fact]
        public async Task PostAsync_InvalidObjectPassed_ReturnsBadRequest()
        {
            var createDto = new UrlCreateDto()
            {
                LongUrl = "Bad URL"
            };

            var controller = new UrlController(_mockRepository.Object);

            var result = (BadRequestObjectResult)await controller.PostAsync(createDto);

            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task PostAsync_ExistingObjectPassed_ReturnsExistingUrl()
        {
            var createDto = new UrlCreateDto()
            {
                LongUrl = "https://timeseriesreasoning.com/contents/poisson-process/"
            };

            var expectedUrls = new List<Url>()
            {
                createDto.AsUrl(),
                createDto.AsUrl()
            };

            _mockRepository
                .Setup(_ => _.GetAllAsync(It.IsAny<Expression<Func<Url, bool>>>()))
                .ReturnsAsync((IReadOnlyCollection<Url>)expectedUrls);

            var controller = new UrlController(_mockRepository.Object);

            var result = (OkObjectResult)await controller.PostAsync(createDto);

            var expectedDto = expectedUrls.First().AsReadDto();

            result.Value.Should().BeEquivalentTo(expectedDto, options => options.ComparingByMembers<UrlReadDto>());
        }

        [Fact]
        public async Task DeleteAsync_WithoutExistingUrl_ReturnsNotFound()
        {
            _mockRepository
                .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Url)null);

            var controller = new UrlController(_mockRepository.Object);

            var result = (NotFoundResult)await controller.DeleteAsync(Guid.NewGuid());

            result.Should().BeOfType<NotFoundResult>();
        }

        [Fact]
        public async Task DeleteAsync_WithExistingUrl_ReturnsExpectedUrl()
        {
            var expected = CreateFakeUrl();

            _mockRepository
                .Setup(_ => _.GetByIdAsync(It.IsAny<Guid>()))
                .ReturnsAsync(expected);

            var controller = new UrlController(_mockRepository.Object);

            var result = (NoContentResult)await controller.DeleteAsync(Guid.NewGuid());

            result.Should().BeOfType<NoContentResult>();
        }

        private static Url CreateFakeUrl(bool IsExpired = false)
        {
            var creationDate = new DateTime(2029, 05, 09, 9, 15, 0);
            if (IsExpired)
                creationDate = new DateTime(2019, 05, 09, 9, 15, 0);

            return new Url()
            {
                Id = Guid.NewGuid(),
                ShortUrl = "1st fake short url",
                LongUrl = "1st fake long url",
                CreationDate = creationDate,
                ClassifiedAs = "Bening",
                ExpiresIn = default
            };
        }
    }
}