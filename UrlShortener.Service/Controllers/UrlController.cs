using Microsoft.AspNetCore.Mvc;
using Repository.Pattern.Generic;
using shortid;
using System.ComponentModel.DataAnnotations;
using UrlClassifier_ML;
using UrlShortener.Service.Dtos;
using UrlShortener.Service.Models;


namespace UrlShortener.Service.Controllers
{
    [ApiController]
    [Route("api/urls")]
    public class UrlController : ControllerBase
    {
        private readonly IRepository<Url> _repository;
        private static readonly string BaseUrl = "shortened.at/";

        public UrlController(IRepository<Url> repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetUrlsAsync()
        {
            var urls = await _repository.GetAllAsync();
            var validUrls = new List<UrlReadDto>();

            foreach (var url in urls)
            {
                var validUrl = url.ValidateExpiration();
                if (validUrl != null)
                    validUrls.Add(validUrl);
            }

            return Ok(validUrls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUrlByIdAsync(Guid id)
        {
            var url = await _repository.GetByIdAsync(id);
            if (url == null)
                return NotFound();

            var validUrl = url.ValidateExpiration();
            if (validUrl == null)
                return NotFound();

            return Ok(validUrl);
        }

        [HttpGet("[action]")]
        public async Task<IActionResult> GetUrlByShort([Required][FromQuery] string shortUrl)
        {
            var urls = await _repository.GetAllAsync(x => x.ShortUrl == shortUrl);
            if (urls == null || !urls.Any())
                return NotFound();

            var url = urls.First();

            var validUrl = url.ValidateExpiration();
            if (validUrl == null)
                return NotFound();

            return Ok(validUrl);
        }

        [HttpPost]
        public async Task<IActionResult> PostAsync(UrlCreateDto createDto)
        {
            if (!Uri.IsWellFormedUriString(createDto.LongUrl, UriKind.Absolute))
                return BadRequest("Invalid URL provided ...");

            UrlClassifier.ModelInput sampleData = new()
            {
                Url = createDto.LongUrl
            };

            var predictionResult = UrlClassifier.Predict(sampleData);

            var url = createDto.AsUrl();
            url.ClassifiedAs = predictionResult.Prediction;

            var existingUrls = await _repository.GetAllAsync(x => x.LongUrl == url.LongUrl);
            if (existingUrls != null)
                return Ok(existingUrls.First().AsReadDto());

            url.ShortUrl = BaseUrl + ShortId.Generate();
            url.CreationDate = DateTime.UtcNow;

            await _repository.PostAsync(url);

            var readDto = url.AsReadDto();

            return CreatedAtAction(nameof(GetUrlByIdAsync), new { url.Id }, readDto);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync(Guid id)
        {
            var url = await _repository.GetByIdAsync(id);
            if (url == null)
                return NotFound();

            await _repository.DeleteAsync(id);

            return NoContent();
        }
    }
}
