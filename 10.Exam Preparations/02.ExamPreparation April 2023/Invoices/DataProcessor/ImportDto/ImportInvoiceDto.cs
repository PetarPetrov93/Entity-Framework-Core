using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportInvoiceDto
    {
        [Required]
        [Range(1_000_000_000, 1_500_000_000)]
        [JsonProperty("Name")]
        public int Number { get; set; }

        [Required]
        [JsonProperty("IssueDate")]
        public string IssueDate { get; set; } = null!;

        [Required]
        [JsonProperty("DueDate")]
        public string DueDate { get; set; } = null!;

        [Required]
        [JsonProperty("Amount")]
        public decimal Amount { get; set; }

        [Required]
        [Range(0, 2)]
        [JsonProperty("CurrencyType")]
        public int CurrencyType { get; set; }

        [Required]
        [JsonProperty("ClientId")]
        public int ClientId { get; set; }
    }
}
