using System.ComponentModel.DataAnnotations;
using System.Xml.Serialization;

namespace Invoices.DataProcessor.ImportDto
{
    [XmlType("Client")]
    public class ImportClientDto
    {
        [Required]
        [MaxLength(25)]
        [MinLength(10)]
        [XmlElement("Name")]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(15)]
        [MinLength(10)]
        [XmlElement("NumberVat")]
        public string NumberVat { get; set; } = null!;

        [XmlArray("Addresses")]
        public ImportAddressDto[] Addresses { get; set; } = null!;
    }
}
