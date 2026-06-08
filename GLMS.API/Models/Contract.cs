// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GLMS.API.Models
{
    public class Contract
    {
        public int Id { get; set; }

        [Required]
        public int ClientId { get; set; }

        [JsonIgnore]
        [ForeignKey("ClientId")]
        public Client? Client { get; set; }

        public string ClientName { get; set; } = string.Empty;

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public string Status { get; set; } = "Draft";

        [Required]
        public string ServiceLevel { get; set; } = string.Empty;

        public string? SignedAgreementPath { get; set; }

        [JsonIgnore]
        public List<ServiceRequest> ServiceRequests { get; set; } = new();
    }
}
