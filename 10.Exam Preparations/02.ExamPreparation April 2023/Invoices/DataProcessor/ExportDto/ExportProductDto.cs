using Invoices.Data.Models.Enums;

namespace Invoices.DataProcessor.ExportDto
{
    public class ExportProductDto
    {
        public string Name { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; }

        public ExportClientDto[] Clients { get; set; } = null!;
    }
}
