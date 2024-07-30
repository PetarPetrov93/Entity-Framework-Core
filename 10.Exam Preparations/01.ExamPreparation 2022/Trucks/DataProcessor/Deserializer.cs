namespace Trucks.DataProcessor
{
    using Data;
    using Newtonsoft.Json;
    using System.ComponentModel.DataAnnotations;
    using System.Text;
    using Trucks.Data.Models;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ImportDto;
    using Trucks.Utilities;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedDespatcher
            = "Successfully imported despatcher - {0} with {1} trucks.";

        private const string SuccessfullyImportedClient
            = "Successfully imported client - {0} with {1} trucks.";

        private static XmlHelper xmlHelper;

        public static string ImportDespatcher(TrucksContext context, string xmlString)
        {
            xmlHelper = new XmlHelper();

            ImportDespatcherDTO[] despatcherDTOs = xmlHelper.Deserialize<ImportDespatcherDTO[]>(xmlString, "Despatchers");

            ICollection<Despatcher> validDespatchers = new HashSet<Despatcher>();

            StringBuilder sb = new StringBuilder();

            foreach (var despatcherDTO in despatcherDTOs)
            {
                if (IsValid(despatcherDTO) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                var despatcher = new Despatcher()
                {
                    Name = despatcherDTO.Name,
                    Position = despatcherDTO.Position
                };

                ICollection<Truck> trucks = new HashSet<Truck>();

                foreach (var truckDTO in despatcherDTO.Trucks)
                {
                    if (IsValid(truckDTO) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    var truck = new Truck()
                    {
                        RegistrationNumber = truckDTO.RegistrationNumber,
                        VinNumber = truckDTO.VinNumber,
                        TankCapacity = truckDTO.TankCapacity,
                        CargoCapacity = truckDTO.CargoCapacity,
                        CategoryType = (CategoryType)truckDTO.CategoryType,
                        MakeType = (MakeType)truckDTO.MakeType
                    };

                    trucks.Add(truck);
                }

                despatcher.Trucks = trucks;

                validDespatchers.Add(despatcher);
                sb.AppendLine(String.Format(SuccessfullyImportedDespatcher, despatcher.Name, trucks.Count()));
            }

            context.Despatchers.AddRange(validDespatchers);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }
        public static string ImportClient(TrucksContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            ImportClientDTO[] clientDtos = JsonConvert.DeserializeObject<ImportClientDTO[]>(jsonString);

            ICollection<Client> validClients = new HashSet<Client>();

            ICollection<int> validTruckIDs = context.Trucks
                .Select(t => t.Id)
                .ToArray();

            foreach (var clientDto in clientDtos)
            {
                if (IsValid(clientDto) == false)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (clientDto.Type == "usual")
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Client client = new Client()
                {
                    Name = clientDto.Name,
                    Type = clientDto.Type,
                    Nationality = clientDto.Nationality,

                };

                foreach (var truckId in clientDto.TruckIds.Distinct())
                {
                    if (validTruckIDs.Contains(truckId) == false)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    ClientsTrucks ct = new ClientsTrucks()
                    {
                        Client = client,
                        TruckId = truckId
                    };

                    client.ClientsTrucks.Add(ct);
                }

                validClients.Add(client);

                sb.AppendLine(String.Format(SuccessfullyImportedClient, client.Name, client.ClientsTrucks.Count));
            }

            context.Clients.AddRange(validClients);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}