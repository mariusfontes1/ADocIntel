using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace DocumentIntelligenceTest.Models
{
    public class DocumentResult
    {
        [JsonPropertyName("clientName")]
        public string? ClientName { get; set; }

        [JsonPropertyName("documentNumber")]
        public string? DocumentNumber { get; set; }

        [JsonPropertyName("date")]
        public DateTime? Date { get; set; }

        [JsonPropertyName("totalValue")]
        public decimal? TotalValue { get; set; }

        [JsonPropertyName("documentType")]
        public string? DocumentType { get; set; }

        [JsonPropertyName("confidence")]
        public double Confidence { get; set; }

        [JsonPropertyName("additionalFields")]
        public Dictionary<string, object> AdditionalFields { get; set; } = new();

        [JsonPropertyName("rawText")]
        public string? RawText { get; set; }

        [JsonPropertyName("processingTime")]
        public TimeSpan ProcessingTime { get; set; }
    }
}
