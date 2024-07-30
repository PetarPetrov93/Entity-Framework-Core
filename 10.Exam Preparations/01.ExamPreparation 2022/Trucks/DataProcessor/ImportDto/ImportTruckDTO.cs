using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckDTO
    {
        [MaxLength(8)]
        [MinLength(8)]
        [RegularExpression(@"[A-Z]{2}\d{4}[A-Z]{2}")]
        [XmlElement("RegistrationNumber")]
        public string? RegistrationNumber { get; set; }

        [MaxLength(17)]
        [MinLength(17)]
        [Required]
        [XmlElement("VinNumber")]
        public string VinNumber { get; set; } = null!;

        [Range(950, 1420)]
        [XmlElement("TankCapacity")]
        public int TankCapacity { get; set; }

        [Range(5000, 29000)]
        [XmlElement("CargoCapacity")]
        public int CargoCapacity { get; set; }

        [Range(0, 3)]
        [XmlElement("CategoryType")]
        public int CategoryType { get; set; }

        [Range(0, 4)]
        [XmlElement("MakeType")]
        public int MakeType { get; set; }
    }
}
