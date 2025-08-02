using MeterReadingsApi.Controllers;
using MeterReadingsApi.Interfaces;
using MeterReadingsApi.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace MeterReadingsApi.UnitTests
{
    [ExcludeFromCodeCoverage]
    public class MeterReadingsControllerTests
    {
        private static IFormFile CreateFile()
        {
            MemoryStream stream = new MemoryStream(new byte[] { 1 });
            return new FormFile(stream, 0, stream.Length, "file", "test.csv");
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_BadRequest_When_File_Is_Null()
        {
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            MeterReadingsController controller = new MeterReadingsController(service.Object);

            ActionResult result = await controller.MeterReadingUploads(null);

            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is null or empty.", badRequest.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_BadRequest_When_File_Is_Empty()
        {
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            MeterReadingsController controller = new MeterReadingsController(service.Object);
            Mock<IFormFile> file = new Mock<IFormFile>();
            file.Setup(f => f.Length).Returns(0);

            ActionResult result = await controller.MeterReadingUploads(file.Object);

            BadRequestObjectResult badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("File is null or empty.", badRequest.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Never);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_Created_When_All_Successful()
        {
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            MeterReadingUploadResult uploadResult = new MeterReadingUploadResult(1, 0);
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            MeterReadingsController controller = new MeterReadingsController(service.Object);
            IFormFile file = CreateFile();

            ActionResult result = await controller.MeterReadingUploads(file);

            CreatedResult created = Assert.IsType<CreatedResult>(result);
            Assert.Equal(uploadResult, created.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_MultiStatus_When_Some_Fail()
        {
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            MeterReadingUploadResult uploadResult = new MeterReadingUploadResult(1, 1);
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            MeterReadingsController controller = new MeterReadingsController(service.Object);
            IFormFile file = CreateFile();

            ActionResult result = await controller.MeterReadingUploads(file);

            ObjectResult objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(StatusCodes.Status207MultiStatus, objectResult.StatusCode);
            Assert.Equal(uploadResult, objectResult.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public async Task MeterReadingUploads_Returns_UnprocessableEntity_When_All_Fail()
        {
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            MeterReadingUploadResult uploadResult = new MeterReadingUploadResult(0, 1);
            service.Setup(s => s.UploadAsync(It.IsAny<IFormFile>())).ReturnsAsync(uploadResult);

            MeterReadingsController controller = new MeterReadingsController(service.Object);
            IFormFile file = CreateFile();

            ActionResult result = await controller.MeterReadingUploads(file);

            UnprocessableEntityObjectResult unprocessable = Assert.IsType<UnprocessableEntityObjectResult>(result);
            Assert.Equal(uploadResult, unprocessable.Value);
            service.Verify(s => s.UploadAsync(It.IsAny<IFormFile>()), Times.Once);
        }

        [Fact]
        public void GetByAccountId_Returns_Ok()
        {
            Mock<IMeterReadingUploadService> service = new Mock<IMeterReadingUploadService>();
            MeterReadingsController controller = new MeterReadingsController(service.Object);

            ActionResult result = controller.GetByAccountId(1);

            Assert.IsType<OkResult>(result);
        }
    }
}
