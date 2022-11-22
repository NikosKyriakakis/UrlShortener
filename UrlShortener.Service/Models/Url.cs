using Repository.Pattern.Generic;
using ZstdSharp.Unsafe;

namespace UrlShortener.Service.Models
{
    public class Url : IEntity
    {
        public Guid Id { get; set; }

        public string ShortUrl { get; set; }

        public string LongUrl { get; set; }

        public string ClassifiedAs { get; set; }   

        public DateTime CreationDate { get; set; }
        
        public TimeSpan ExpiresIn { get; set; }
    }
}
