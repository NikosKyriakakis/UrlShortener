namespace UrlShortener.Service.Dtos
{
    public record UrlReadDto
    {
        public Guid Id { get; set; }

        public string ShortUrl { get; set; }

        public string LongUrl { get; set; }

        public string ClassifiedAs { get; set; }

        public DateTime ExpirationDate { get; set; }
    }
}
