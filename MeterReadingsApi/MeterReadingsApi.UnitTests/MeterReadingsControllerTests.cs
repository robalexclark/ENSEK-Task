using MeterReadingsApi.Controllers;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingsControllerTests
    {
        private static IFormFile CreateFile()
        {
            var stream = new MemoryStream(new byte[] { 1 });
            return new FormFile(stream, 0, stream.Length, "file", "test.csv");
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_BadRequest_When_File_Is_Null()
        {
            var service = new Mock<IMeterReadingUploadService>();
            var controller = new MeterReadingsController(service.Object);

            var result = await controller.MeterReadingUploads(null);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is null or empty.", badRequest.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_BadRequest_When_File_Is_Empty()
        {
            var service = new Mock<IMeterReadingUploadService>();
            var controller = new MeterReadingsController(service.Object);
            var file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(0);

            var result = await controller.MeterReadingUploads(file.Object);

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is null or empty.", badRequest.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_Created_When_All_Successful()
        {
            var service = new Mock<IMeterReadingUploadService>();
            var uploadResult = new MeterReadingUploadResult(1, 0);
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            var controller = new MeterReadingsController(service.Object);
            var file = CreateFile();

            var result = await controller.MeterReadingUploads(file);

            var created = Assert.IsType<CreatedResult>(result);
            Assert.Equal(uploadResult, created.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_MultiStatus_When_Some_Fail()
        {
            var service = new Mock<IMeterReadingUploadService>();
            var uploadResult = new MeterReadingUploadResult(1, 1);
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            var controller = new MeterReadingsController(service.Object);
            var file = CreateFile();

            var result = await controller.MeterReadingUploads(file);

            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status207MultiStatus, objectResult.StatusCode);
            Assert.Equal(uploadResult, objectResult.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_UnprocessableEntity_When_All_Fail()
        {
            var service = new Mock<IMeterReadingUploadService>();
            var uploadResult = new MeterReadingUploadResult(0, 1);
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            var controller = new MeterReadingsController(service.Object);
            var file = CreateFile();

            var result = await controller.MeterReadingUploads(file);

            var unprocessable = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(uploadResult, unprocessable.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public void GetByAccountId_Returns_Ok()
        {
            var service = new Mock<IMeterReadingUploadService>();
            var controller = new MeterReadingsController(service.Object);

            var result = controller.GetByAccountId(1);

            Assert.IsType<OkResult>(result);
        }
    }
}
