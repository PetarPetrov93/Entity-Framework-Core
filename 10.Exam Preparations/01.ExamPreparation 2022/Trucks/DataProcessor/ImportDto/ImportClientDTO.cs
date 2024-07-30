using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Trucks.DataProcessor.ImportDto
{
    public class ImportClientDTO
    {
        [Required]
        [MaxLength(40)]
        [MinLength(3)]
        [JsonProperty("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        [JsonProperty("Nationality")]
        public string Nationality { get; set; } = null!;

        [Required]
        [JsonProperty("Type")]
        public string Type { get; set; } = null!;

        [JsonProperty("Trucks")]
        public int[] TruckIds { get; set; }
    }
}
