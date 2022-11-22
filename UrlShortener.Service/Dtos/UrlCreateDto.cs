using System.ComponentModel.DataAnnotations;

namespace UrlShortener.Service.Dtos
{
    public record UrlCreateDto
    {
        [Required]
        public string LongUrl { get; set; }

        [Required]
        public TimeSpan ExpiresIn { get; set; }
    }
}
