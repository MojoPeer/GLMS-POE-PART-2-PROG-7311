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
            var dbContext = GetInMemoryDbContext();
            var service = new ServiceRequestAppService(dbContext);

            var contract = new Contract
            {
                Id = 1,
                ClientId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Status = "Draft",
                ServiceLevel = "Gold"
            };

            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            var request = new ServiceRequest
            {
                ContractId = 1,
                Description = "Test Request",
                Cost = 1000,
                Status = "Pending"
            };

            var result = await service.AddAsync(request);

            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_ReturnsFalse_WhenContractStatusIsExpired()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ServiceRequestAppService(dbContext);

            var contract = new Contract
            {
                Id = 2,
                ClientId = 1,
                StartDate = DateTime.Today.AddMonths(-2),
                EndDate = DateTime.Today.AddDays(-1),
                Status = "Expired",
                ServiceLevel = "Silver"
            };

            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            var request = new ServiceRequest
            {
                ContractId = 2,
                Description = "Test Request",
                Cost = 1000,
                Status = "Pending"
            };

            var result = await service.AddAsync(request);

            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_ReturnsFalse_WhenContractStatusIsOnHold()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ServiceRequestAppService(dbContext);

            var contract = new Contract
            {
                Id = 3,
                ClientId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(1),
                Status = "On Hold",
                ServiceLevel = "Bronze"
            };

            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            var request = new ServiceRequest
            {
                ContractId = 3,
                Description = "Test Request",
                Cost = 1000,
                Status = "Pending"
            };

            var result = await service.AddAsync(request);

            Assert.False(result);
        }

        [Fact]
        public async Task AddAsync_ReturnsTrue_WhenContractStatusIsActive()
        {
            var dbContext = GetInMemoryDbContext();
            var service = new ServiceRequestAppService(dbContext);

            var contract = new Contract
            {
                Id = 4,
                ClientId = 1,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today.AddMonths(2),
                Status = "Active",
                ServiceLevel = "Platinum"
            };

            dbContext.Contracts.Add(contract);
            dbContext.SaveChanges();

            var request = new ServiceRequest
            {
                ContractId = 4,
                Description = "Test Request",
                Cost = 1000,
                Status = "Pending"
            };

            var result = await service.AddAsync(request);

            Assert.True(result);
        }
    }
}