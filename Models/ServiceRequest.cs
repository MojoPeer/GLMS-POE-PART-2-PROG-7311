using System;

namespace GLMS.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }
        public int ContractId { get; set; }
        public string Description { get; set; }
        public decimal Cost { get; set; }
        public decimal ZARValue { get; set; } // For converted value
        public ServiceRequestStatus Status { get; set; }

        // Navigation property
        public Contract Contract { get; set; }
    }
}
