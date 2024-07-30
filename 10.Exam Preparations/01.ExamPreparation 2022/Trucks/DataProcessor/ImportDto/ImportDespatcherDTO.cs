using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;
using Trucks.Data.Models;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Despatcher")]
    public class ImportDespatcherDTO
    {
        [Required]
        [MaxLength(40)]
        [MinLength(2)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [XmlElement("Position")]
        [Required]
        public string? Position { get; set; }

        [XmlArray("Trucks")]
        public ImportTruckDTO[] Trucks { get; set; }
    }
}
