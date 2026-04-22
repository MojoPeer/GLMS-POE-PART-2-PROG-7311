using System.Collections.Generic;

namespace GLMS.Models
{
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ContactDetails { get; set; }
        public string Region { get; set; }

        // Navigation property
        public ICollection<Contract> Contracts { get; set; }
    }
}
