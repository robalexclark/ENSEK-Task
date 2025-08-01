using Microsoft.AspNetCore.Http;

namespace MeterReadingsApi.Interfaces
{
    public interface IMeterReadingUploadService
    {
        Task<(int Successful, int Failed)> UploadAsync(IFormFile file);
    }
}