namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.DataProcessor.ExportDto;
    using Invoices.Utilities;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Globalization;
    using Formatting = Newtonsoft.Json.Formatting;

    public class Serializer
    {
        public static string ExportClientsWithTheirInvoices(InvoicesContext context, DateTime date)
        {
            XmlHelper helper = new XmlHelper();

            var clientsToExport = context.Clients
                .Include(c => c.Invoices)
                .Where(c => c.Invoices.Any(i => DateTime.Compare(i.IssueDate, date) > 0))
                .Select(c => new ExportClientXmlDto()
                {
                    ClientName = c.Name,
                    VatNumber = c.NumberVat,
                    InvoicesCount = c.Invoices.Count,
                    Invoices = c.Invoices
                               .OrderBy(i => i.IssueDate)
                               .ThenByDescending(i => i.DueDate)
                               .Select(i => new ExportInvoiceDto()
                               {
                                   InvoiceNumber = i.Number,
                                   InvoiceAmount = i.Amount,
                                   Currency = i.CurrencyType.ToString(),
                                   DueDate = i.DueDate.ToString("d", CultureInfo.InvariantCulture)
                               })
                               .ToArray()
                })
                .OrderByDescending(c => c.InvoicesCount)
                .ThenBy(c => c.ClientName)
                .ToArray();

            return helper.Serialize(clientsToExport, "Clients");
        }

        public static string ExportProductsWithMostClients(InvoicesContext context, int nameLength)
        {

            var productsWithMostClients = context.Products
                .Include(p => p.ProductsClients)
                .ThenInclude(pc => pc.Client)
                .Where(p => p.ProductsClients.Any(pc => pc.Client.Name.Length >= nameLength))
                .Select(p => new ExportProductDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Category = p.CategoryType.ToString(),
                    Clients = p.ProductsClients
                              .Where(pc => pc.Client.Name.Length >= nameLength)
                              .Select(pc => new ExportClientDto()
                              {
                                  Name = pc.Client.Name,
                                  NumberVat = pc.Client.NumberVat,
                              })
                              .OrderBy(c => c.Name)
                              .ToArray()
                })
                .OrderByDescending(p => p.Clients.Count())
                .ThenBy(p => p.Name)
                .Take(5)
                .ToList();

            return JsonConvert.SerializeObject(productsWithMostClients, Formatting.Indented);
        }
    }
}