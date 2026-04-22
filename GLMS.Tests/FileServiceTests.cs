// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;
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

            var service = new FileService();

            // Act
            var result = service.IsValidPdf(fileMock.Object);

            // Assert
            Assert.False(result);
        }
    }
}