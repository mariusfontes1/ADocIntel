using System.ComponentModel.DataAnnotations;

namespace DocumentIntelligenceTest.Models
{
    public class AzureConfig
    {
        [Required]
        public string Endpoint { get; set; } = string.Empty;

        [Required]
        public string ApiKey { get; set; } = string.Empty;

        public string ModelId { get; set; } = "prebuilt-document";
    }
}
