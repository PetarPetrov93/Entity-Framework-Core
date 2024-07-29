using Invoices.Data.Models.Enums;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Invoices.DataProcessor.ImportDto
{
    public class ImportProductDto
    {
        [Required]
        [MaxLength(30)]
        [MinLength(9)]
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [Range(5.00, 1000.00)]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [Required]
        [Range(0, 4)]
        [JsonProperty("CategoryType")]
        public int CategoryType { get; set; }

        [Required]
        [JsonProperty("Clients")]
        public int[] Clients { get; set; } = null!;
    }
}
