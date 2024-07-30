﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Trucks.DataProcessor.ExportDto
{
    [XmlType("Truck")]
    public class ExportTruckDTO
    {
        [XmlElement("RegistrationNumber")]
        public string? RegistrationNumber { get; set; }

        [XmlElement("Make")]
        public string Make { get; set; } = null!;
    }
}
