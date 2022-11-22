using UrlShortener.Service.Dtos;
using UrlShortener.Service.Models;

namespace UrlShortener.Service
{
    public static class Extensions
    {
        public static UrlReadDto AsReadDto(this Url url)
        {
            return new UrlReadDto
            {
                Id = url.Id,
                ShortUrl = url.ShortUrl,
                LongUrl = url.LongUrl,
                ExpirationDate = url.CreationDate.Add(url.ExpiresIn),
                ClassifiedAs= url.ClassifiedAs
            };
        }

        public static Url AsUrl(this UrlCreateDto dto)
        {
            return new Url
            {
                LongUrl = dto.LongUrl,
                ExpiresIn = dto.ExpiresIn
            };
        }

        public static UrlReadDto? ValidateExpiration(this Url url)
        {
            var expirationDate = url.CreationDate.Add(url.ExpiresIn);

            var result = DateTime.Compare(expirationDate, DateTime.UtcNow);
            if (result >= 0)
                return url.AsReadDto();
            else
                return null;
        }
    }
}
