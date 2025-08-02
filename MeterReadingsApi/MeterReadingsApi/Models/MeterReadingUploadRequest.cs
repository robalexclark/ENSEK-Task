using Microsoft.AspNetCore.Http;

namespace MeterReadingsApi.Models
{
    public class MeterReadingUploadRequest
    {
        public IFormFile? File { get; set; }
    }
}
