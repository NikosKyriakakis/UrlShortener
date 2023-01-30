namespace UrlShortener.Service.Dtos
{
    public record UrlReadDto
    {
        public Guid Id { get; set; }

        public string ShortUrl { get; set; } = string.Empty;

        public string LongUrl { get; set; } = string.Empty;

        public string ClassifiedAs { get; set; } = string.Empty;

        public DateTime ExpirationDate { get; set; }
    }
}
