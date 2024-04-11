using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ProductShop.DTOs.Export
{
    [XmlType("User")]
    public class ExportUserWithProductsDTO
    {
        [XmlElement("firstName")]
        public string Firstname { get; set; }

        [XmlElement("lastName")]
        public string Lastname { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlElement("SoldProducts")]
        public ExportSoldProductDTO SoldProducts { get; set; }
    }
}
