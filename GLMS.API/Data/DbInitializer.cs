// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using GLMS.API.Models;

namespace GLMS.API.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Clients.Any())
                return;

            var clients = new List<Client>
            {
                new Client { Name = "Global Logistics Inc.", ContactDetails = "info@globallogistics.com", Region = "North America" },
                new Client { Name = "TransWorld Shipping", ContactDetails = "contact@transworld.com", Region = "Europe" },
                new Client { Name = "Pacific Freight", ContactDetails = "support@pacificfreight.com", Region = "Asia" },
                new Client { Name = "Continental Movers", ContactDetails = "sales@continentalmovers.com", Region = "Africa" },
                new Client { Name = "Oceanic Carriers", ContactDetails = "help@oceaniccarriers.com", Region = "Australia" },
                new Client { Name = "Atlas Cargo", ContactDetails = "atlas@cargo.com", Region = "South America" },
                new Client { Name = "SkyBridge Logistics", ContactDetails = "sky@bridge.com", Region = "Europe" },
                new Client { Name = "BlueWave Freight", ContactDetails = "blue@wave.com", Region = "Asia" },
                new Client { Name = "Rapid Route Transport", ContactDetails = "rapid@route.com", Region = "Africa" },
                new Client { Name = "Unity Supply Chain", ContactDetails = "unity@supply.com", Region = "North America" }
            };

            context.Clients.AddRange(clients);
            context.SaveChanges();

            var contracts = new List<Contract>
            {
                new Contract { ClientId = clients[0].Id, ClientName = clients[0].Name, StartDate = DateTime.Today.AddMonths(-6), EndDate = DateTime.Today.AddMonths(6), Status = "Active", ServiceLevel = "Gold" },
                new Contract { ClientId = clients[1].Id, ClientName = clients[1].Name, StartDate = DateTime.Today.AddMonths(-12), EndDate = DateTime.Today.AddMonths(-1), Status = "Expired", ServiceLevel = "Silver" },
                new Contract { ClientId = clients[2].Id, ClientName = clients[2].Name, StartDate = DateTime.Today.AddMonths(-3), EndDate = DateTime.Today.AddMonths(9), Status = "Active", ServiceLevel = "Platinum" },
                new Contract { ClientId = clients[3].Id, ClientName = clients[3].Name, StartDate = DateTime.Today.AddMonths(-1), EndDate = DateTime.Today.AddMonths(11), Status = "On Hold", ServiceLevel = "Bronze" },
                new Contract { ClientId = clients[4].Id, ClientName = clients[4].Name, StartDate = DateTime.Today.AddMonths(-2), EndDate = DateTime.Today.AddMonths(10), Status = "Draft", ServiceLevel = "Gold" },
                new Contract { ClientId = clients[5].Id, ClientName = clients[5].Name, StartDate = DateTime.Today.AddMonths(-8), EndDate = DateTime.Today.AddMonths(4), Status = "Active", ServiceLevel = "Silver" },
                new Contract { ClientId = clients[6].Id, ClientName = clients[6].Name, StartDate = DateTime.Today.AddMonths(-4), EndDate = DateTime.Today.AddMonths(8), Status = "On Hold", ServiceLevel = "Gold" },
                new Contract { ClientId = clients[7].Id, ClientName = clients[7].Name, StartDate = DateTime.Today.AddMonths(-10), EndDate = DateTime.Today.AddMonths(2), Status = "Expired", ServiceLevel = "Bronze" },
                new Contract { ClientId = clients[8].Id, ClientName = clients[8].Name, StartDate = DateTime.Today.AddMonths(-1), EndDate = DateTime.Today.AddMonths(12), Status = "Active", ServiceLevel = "Platinum" },
                new Contract { ClientId = clients[9].Id, ClientName = clients[9].Name, StartDate = DateTime.Today, EndDate = DateTime.Today.AddMonths(12), Status = "Draft", ServiceLevel = "Silver" }
            };

            context.Contracts.AddRange(contracts);
            context.SaveChanges();

            var serviceRequests = new List<ServiceRequest>
            {
                new ServiceRequest { ContractId = contracts[0].Id, ContractInfo = $"Contract #{contracts[0].Id}", Description = "Urgent shipment to Canada", Cost = 15000m, Status = "Pending" },
                new ServiceRequest { ContractId = contracts[0].Id, ContractInfo = $"Contract #{contracts[0].Id}", Description = "Regular delivery to USA", Cost = 5000m, Status = "Completed" },
                new ServiceRequest { ContractId = contracts[2].Id, ContractInfo = $"Contract #{contracts[2].Id}", Description = "Express freight to Japan", Cost = 20000m, Status = "Pending" },
                new ServiceRequest { ContractId = contracts[2].Id, ContractInfo = $"Contract #{contracts[2].Id}", Description = "Standard shipping to China", Cost = 8000m, Status = "Completed" },
                new ServiceRequest { ContractId = contracts[5].Id, ContractInfo = $"Contract #{contracts[5].Id}", Description = "Port delivery to Brazil", Cost = 12000m, Status = "Pending" },
                new ServiceRequest { ContractId = contracts[5].Id, ContractInfo = $"Contract #{contracts[5].Id}", Description = "Customs transfer support", Cost = 9500m, Status = "Completed" },
                new ServiceRequest { ContractId = contracts[8].Id, ContractInfo = $"Contract #{contracts[8].Id}", Description = "Warehouse-to-port movement", Cost = 11000m, Status = "Pending" },
                new ServiceRequest { ContractId = contracts[8].Id, ContractInfo = $"Contract #{contracts[8].Id}", Description = "International route planning", Cost = 18000m, Status = "Completed" },
                new ServiceRequest { ContractId = contracts[0].Id, ContractInfo = $"Contract #{contracts[0].Id}", Description = "Priority freight insurance add-on", Cost = 7000m, Status = "Pending" },
                new ServiceRequest { ContractId = contracts[2].Id, ContractInfo = $"Contract #{contracts[2].Id}", Description = "Cross-border compliance support", Cost = 13000m, Status = "Completed" }
            };

            context.ServiceRequests.AddRange(serviceRequests);
            context.SaveChanges();
        }
    }
}
