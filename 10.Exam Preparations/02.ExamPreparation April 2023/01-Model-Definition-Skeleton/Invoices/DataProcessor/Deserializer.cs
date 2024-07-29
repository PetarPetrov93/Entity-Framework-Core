namespace Invoices.DataProcessor
{
    using Invoices.Data;
    using Invoices.Data.Models;
    using Invoices.Data.Models.Enums;
    using Invoices.DataProcessor.ImportDto;
    using Invoices.Utilities;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Text;
    using System.Text.Json;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedClients
            = "Successfully imported client {0}.";

        private const string SuccessfullyImportedInvoices
            = "Successfully imported invoice with number {0}.";

        private const string SuccessfullyImportedProducts
            = "Successfully imported product - {0} with {1} clients.";


        public static string ImportClients(InvoicesContext context, string xmlString)
        {
            XmlHelper helper = new XmlHelper();
            StringBuilder sb = new StringBuilder();

            ImportClientDto[] importClients = helper.Deserialize<ImportClientDto[]>(xmlString, "Clients");

            HashSet<Client> validClients = new HashSet<Client>();

            foreach (var clientDto in importClients)
            {
                if (IsValid(clientDto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                HashSet<Address> validAddresses = new HashSet<Address>();

                foreach (var addressDto in clientDto.Addresses)
                {
                    if (IsValid(addressDto) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Address address = new Address()
                    {
                        StreetName = addressDto.StreetName,
                        StreetNumber = addressDto.StreetNumber,
                        PostCode = addressDto.PostCode,
                        City = addressDto.City,
                        Country = addressDto.Country
                    };

                    validAddresses.Add(address);
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    NumberVat = clientDto.NumberVat,
                    Addresses = validAddresses
                };

                validClients.Add(client);
                sb.AppendLine(String.Format(SuccessfullyImportedClients, client.Name));
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }


        public static string ImportInvoices(InvoicesContext context, string jsonString) 
        {
            ImportInvoiceDto[] invoiceDtos = JsonSerializer.Deserialize<ImportInvoiceDto[]>(jsonString);
            StringBuilder sb = new StringBuilder();

            HashSet<Invoice> validInvoices = new HashSet<Invoice>();

            foreach (var invoiceDto in invoiceDtos)
            {
                if (IsValid(invoiceDto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isIssueDateValid = DateTime.TryParse(invoiceDto.IssueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime issueDate);
                bool isDueDateValid = DateTime.TryParse(invoiceDto.DueDate, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dueDate);

                if (isIssueDateValid == false || isDueDateValid == false || DateTime.Compare(dueDate, issueDate) < 0)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (context.Clients.Any(c => c.Id == invoiceDto.ClientId) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Invoice invoice = new Invoice()
                {
                    Number = invoiceDto.Number,
                    IssueDate = issueDate,
                    DueDate = dueDate,
                    Amount = invoiceDto.Amount,
                    CurrencyType = (CurrencyType)invoiceDto.CurrencyType,
                    ClientId = invoiceDto.ClientId
                };

                validInvoices.Add(invoice);
                sb.AppendLine(String.Format(SuccessfullyImportedInvoices, invoice.Number));
            }

            context.Invoices.AddRange(validInvoices);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static string ImportProducts(InvoicesContext context, string jsonString)
        {
            ImportProductDto[] productDtos = JsonSerializer.Deserialize<ImportProductDto[]>(jsonString);
            StringBuilder sb = new StringBuilder();

            int[] validClientIds = context.Clients
                .Select(c => c.Id)
                .ToArray();

            HashSet<Product> products = new HashSet<Product>();

            foreach (var productDto in productDtos)
            {
                if (IsValid(productDto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Product product = new Product()
                {
                    Name = productDto.Name,
                    Price = productDto.Price,
                    CategoryType = (CategoryType)productDto.CategoryType,
                };

                HashSet<ProductClient> productClientsToImport = new HashSet<ProductClient>();

                foreach (var clientId in productDto.Clients.Distinct())
                {
                    if (validClientIds.Contains(clientId) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ProductClient productClient = new ProductClient()
                    {
                        Product = product,
                        ClientId = clientId
                    };

                    productClientsToImport.Add(productClient);
                }

                product.ProductsClients = productClientsToImport;

                products.Add(product);
                sb.AppendLine(String.Format(SuccessfullyImportedProducts, productDto.Name, productClientsToImport.Count));
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return sb.ToString().TrimEnd();
        }

        public static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    } 
}
