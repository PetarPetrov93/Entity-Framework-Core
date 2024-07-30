namespace Trucks.DataProcessor
{
    using Data;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using System.Text.Json.Nodes;
    using Trucks.Data.Models.Enums;
    using Trucks.DataProcessor.ExportDto;
    using Trucks.Utilities;

    public class Serializer
    {
        public static string ExportDespatchersWithTheirTrucks(TrucksContext context)
        {
            XmlHelper helper = new XmlHelper();

            var despatchers = context.Despatchers
                .Where(d => d.Trucks.Any())
                .ToList()
                .Select(d => new ExportDespatcherDTO()
                {
                    TrucksCount = d.Trucks.Count,
                    DespatcherName = d.Name,
                    Trucks = d.Trucks
                    .Select(t => new ExportTruckDTO()
                    {
                        RegistrationNumber = t.RegistrationNumber,
                        Make = t.MakeType.ToString(),
                    })
                    .OrderBy(t => t.RegistrationNumber)
                    .ToArray()
                })
                .OrderByDescending(d => d.TrucksCount)
                .ThenBy(d => d.DespatcherName)
                .ToArray();

            return helper.Serialize(despatchers, "Despatchers");
        }

        public static string ExportClientsWithMostTrucks(TrucksContext context, int capacity)
        {
            var clients = context.Clients
                .Include(c => c.ClientsTrucks)
                .ThenInclude(ct => ct.Truck)
                .ToList()
                .Where(c => c.ClientsTrucks.Any(ct => ct.Truck.TankCapacity >= capacity))
                .Select(c => new
                {
                    c.Name,
                    Trucks = c.ClientsTrucks
                            .Where(ct => ct.Truck.TankCapacity >= capacity)
                            .Select(ct => new
                            {
                                TruckRegistrationNumber = ct.Truck.RegistrationNumber,
                                VinNumber = ct.Truck.VinNumber,
                                TankCapacity = ct.Truck.TankCapacity,
                                CargoCapacity = ct.Truck.CargoCapacity,
                                CategoryType = ct.Truck.CategoryType.ToString(),
                                MakeType = ct.Truck.MakeType.ToString()
                            })
                            .OrderBy(t => t.MakeType)
                            .ThenByDescending(t => t.CargoCapacity)
                            .ToList()
                })
                .OrderByDescending(c => c.Trucks.Count)
                .ThenBy(c => c.Name)
                .Take(10)
                .ToList();

            return JsonConvert.SerializeObject(clients, Formatting.Indented);
        }
    }
}
