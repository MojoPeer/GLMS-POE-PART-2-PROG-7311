// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace GLMS.Tests
{
    public class FileServiceTests
    {
        [Fact]
        public void IsValidPdf_ReturnsTrueForPdfFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.pdf");
            fileMock.Setup(f => f.Length).Returns(100);

            var service = new FileService();

            // Act
            var result = service.IsValidPdf(fileMock.Object);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsValidPdf_ReturnsFalseForNonPdfFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.txt");
            fileMock.Setup(f => f.Length).Returns(100);

            var service = new FileService();

            // Act
            var result = service.IsValidPdf(fileMock.Object);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void IsValidPdf_ReturnsFalseForEmptyFile()
        {
            // Arrange
            var fileMock = new Mock<IFormFile>();
            fileMock.Setup(f => f.FileName).Returns("test.pdf");
            fileMock.Setup(f => f.Length).Returns(0);

            var service = new FileService();

            // Act
            var result = service.IsValidPdf(fileMock.Object);

            // Assert
            Assert.False(result);
        }
    }
}