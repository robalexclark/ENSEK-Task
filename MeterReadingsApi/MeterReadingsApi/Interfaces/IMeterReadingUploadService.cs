namespace MeterReadingsApi.Interfaces
{
    public interface IMeterReadingUploadService
    {
        Task<Models.MeterReadingUploadResult> UploadAsync(IFormFile file);
    }
}