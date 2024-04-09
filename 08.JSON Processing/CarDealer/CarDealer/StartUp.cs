using AutoMapper;
using CarDealer.Data;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using Castle.Core.Resource;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            CarDealerContext context = new CarDealerContext();

            //UNCOMMENT CODE TO EXECUTE THE REQUIRED QUERIES:

            //Problem 9
            //string input9 = File.ReadAllText(@"../../../Datasets/suppliers.json");
            //string output9 = ImportSuppliers(context, input9);
            //Console.WriteLine(output9);

            //Problem 10
            //string input10 = File.ReadAllText(@"../../../Datasets/suppliers.json");
            //string output10 = ImportSuppliers(context, input10);
            //Console.WriteLine(output10);

            //Problem 11
            //string input11 = File.ReadAllText(@"../../../Datasets/cars.json");
            //string output11 = ImportCars(context, input11);
            //Console.WriteLine(output11);

            //Problem 12
            //string input12 = File.ReadAllText(@"../../../Datasets/customers.json");
            //string output12 = ImportCustomers(context, input12);
            //Console.WriteLine(output12);

            //Problem 13
            //string input13 = File.ReadAllText(@"../../../Datasets/sales.json");
            //string output13 = ImportSales(context, input13);
            //Console.WriteLine(output13);

            //Problem 14
            //string output14 = GetOrderedCustomers(context);
            //Console.WriteLine(output14);

            //Problem 15
            //string output15 = GetCarsFromMakeToyota(context);
            //Console.WriteLine(output15);

            //Problem 16
            //string output16 = GetLocalSuppliers(context);
            //Console.WriteLine(output16);

            //Problem 17
            //string output17 = GetCarsWithTheirListOfParts(context);
            //Console.WriteLine(output17);

            //Problem 18
            //string output18 = GetTotalSalesByCustomer(context);
            //Console.WriteLine(output18);

            //Problem 19
            string output19 = GetSalesWithAppliedDiscount(context);
            Console.WriteLine(output19);
        }

        //Problem 9
        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MyMapper();

            var suppliersDTO = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            //Mapping the whole collection
            var suppliers = mapper.Map<Supplier[]>(suppliersDTO);

            //If there are any validations we need to check for we should use foreach where we are more flexible:
            //foreach (var dto in suppliersDTO!)
            //{
            //    Supplier supplier = mapper.Map<Supplier>(dto);
            //    suppliers.Add(supplier);
            //}

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}.";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MyMapper();

            ImportPartDTO[] partsDTO = JsonConvert.DeserializeObject<ImportPartDTO[]>(inputJson)!
                    .Where(dtoObj => context.Suppliers
                            .Select(s => s.Id)
                            .Contains(dtoObj.SupplierId))
                    .ToArray();

            var parts = new HashSet<Part>();

            foreach (var partDTO in partsDTO!)
            {
                Part part = mapper.Map<Part>(partDTO);
                parts.Add(part);
            }
            context.Parts.AddRange(parts);
            context.SaveChanges();
            return $"Successfully imported {parts.Count}.";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MyMapper();

            var carsDTO = JsonConvert.DeserializeObject<ImportCarDTO[]>(inputJson);

            var cars = new HashSet<Car>();
            var carParts = new HashSet<PartCar>();

            foreach (var carDTO in carsDTO!)
            {
                Car car = mapper.Map<Car>(carDTO);

                foreach (var part in carDTO.PartsId.Distinct())
                {
                    var carPart = new PartCar()
                    {
                        PartId = part,
                        Car = car,
                    };
                    carParts.Add(carPart);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.PartsCars.AddRange(carParts);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}.";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MyMapper();

            var customersDTO = JsonConvert.DeserializeObject<ImportCustomerDTO[]>(inputJson);

            var customers = new HashSet<Customer>();

            foreach (var customerDTO in customersDTO!)
            {
                Customer customer = mapper.Map<Customer>(customerDTO);

                customers.Add(customer);
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();
            return $"Successfully imported {customers.Count}.";
        }

        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            IMapper mapper = MyMapper();

            var salesDTO = JsonConvert.DeserializeObject<ImportSaleDTO[]>(inputJson);

            var sales = new HashSet<Sale>();

            foreach (var sDTO in salesDTO!)
            {
                Sale sale = mapper.Map<Sale>(sDTO);
                sales.Add(sale);
            }
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}.";
        }

        //Problem 14
        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new
                {
                    c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy"),
                    c.IsYoungDriver,
                })
                .AsNoTracking()
                .ToArray();

            var output = JsonConvert.SerializeObject(customers, Formatting.Indented);

            return output;
        }

        //Problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .Select(c => new
                {
                    c.Id,
                    c.Make,
                    c.Model,
                    c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(cars, Formatting.Indented);

            return result;
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new
                {
                    s.Id,
                    s.Name,
                    PartsCount = s.Parts.Count,
                })
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(suppliers, Formatting.Indented);
            return result;
        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var carsAndParts = context.Cars
                .Select(c => new
                {
                    car = new
                    {
                        c.Make,
                        c.Model,
                        c.TraveledDistance
                    },
                    parts = c.PartsCars
                            .Where(cp => cp.Car.Id == c.Id)
                            .Select (cp => new
                            {
                                Name = cp.Part.Name,
                                Price = cp.Part.Price.ToString("f2")
                            })
                            .ToArray()
                })
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(carsAndParts, Formatting.Indented);
            return result;
        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            // Setting the property naming convention to camelCase in order to match the requirements from the problem description:
            JsonSerializerSettings serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            var customersWithCars = context.Customers
                .Where(c=> c.Sales.Count() > 0)
                .Select(c => new
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(cp => cp.Part.Price))
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars)
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(customersWithCars, Formatting.Indented, serializerSettings);
            return result;
        }

        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var salesInfo = context.Sales
                .Select(s => new
                {
                    car = new
                    {
                        s.Car.Make,
                        s.Car.Model,
                        s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    priceWithDiscount = $"{s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - (s.Discount / 100)):F2}"

                })
                .Take(10)
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(salesInfo, Formatting.Indented);
            return result;
        }

        public static IMapper MyMapper()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CarDealerProfile>();
            }));

            return mapper;
        }
    }
}