// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.AppServices;
using GLMS.Data;
using GLMS.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using Xunit;

namespace GLMS.Tests
{
    public class ServiceRequestAppServiceTests
    {
        private ApplicationDbContext GetInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public async Task AddAsync_ReturnsFalse_WhenContractStatusIsDraft()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var service = new ServiceRequestAppService(dbContext);

            var contract = new Contract { Id = 1, Status = "Draft" };
            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            var request = new ServiceRequest { ContractId = 1, Description = "Test Request", Cost = 1000 };

            // Act
            var result = await service.AddAsync(request);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_ReturnsTrue_WhenContractStatusIsActive()
        {
            // Arrange
            var dbContext = GetInMemoryDbContext();
            var service = new ServiceRequestAppService(dbContext);

            var contract = new Contract { Id = 1, Status = "Active" };
            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            var request = new ServiceRequest { ContractId = 1, Description = "Test Request", Cost = 1000 };

            // Act
            var result = await service.AddAsync(request);

            // Assert
            Assert.True(result);
        }
    }
}