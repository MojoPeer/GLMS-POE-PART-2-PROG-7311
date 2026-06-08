// Code attribution
// OpenAI. 2026. ChatGPT (Version 5.3)
// Used for guidance

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace GLMS.API.Models
{
    public class Client
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string ContactDetails { get; set; } = string.Empty;

        [Required]
        public string Region { get; set; } = string.Empty;

        [JsonIgnore]
        public List<Contract> Contracts { get; set; } = new();
    }
}
