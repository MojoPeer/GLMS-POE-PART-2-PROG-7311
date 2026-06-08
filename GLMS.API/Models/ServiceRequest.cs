// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace GLMS.API.Models
{
    public class ServiceRequest
    {
        public int Id { get; set; }

        [Required]
        public int ContractId { get; set; }

        [JsonIgnore]
        [ForeignKey("ContractId")]
        public Contract? Contract { get; set; }

        public string ContractInfo { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Cost { get; set; }

        [Required]
        public string Status { get; set; } = "Pending";
    }
}
