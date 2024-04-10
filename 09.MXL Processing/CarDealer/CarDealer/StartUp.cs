using AutoMapper;
using AutoMapper.QueryableExtensions;
using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using CarDealer.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ValueGeneration;
using System.IO;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            using CarDealerContext context = new CarDealerContext();

            //Problem 09
            //string xmlPath = File.ReadAllText("../../../Datasets/suppliers.xml");
            //string output9 = ImportSuppliers(context, xmlPath);
            //Console.WriteLine(output9);

            //Problem 10
            //string xmlPath = File.ReadAllText("../../../Datasets/parts.xml");
            //string output10 = ImportParts(context, xmlPath);
            //Console.WriteLine(output10);

            //Problem 11
            //string xmlPath = File.ReadAllText("../../../Datasets/cars.xml");
            //string output11 = ImportCars(context, xmlPath);
            //Console.WriteLine(output11);

            //Problem 12
            //string xmlPath = File.ReadAllText("../../../Datasets/customers.xml");
            //string output12 = ImportCustomers(context, xmlPath);
            //Console.WriteLine(output12);

            //Problem 13
            //string xmlPath = File.ReadAllText("../../../Datasets/sales.xml");
            //string output13 = ImportSales(context, xmlPath);
            //Console.WriteLine(output13);

            //Problem 14
            //string output14 = GetCarsWithDistance(context);
            //Console.WriteLine(output14);

            //Problem 15
            //string output15 = GetCarsFromMakeBmw(context);
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

        //Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            //Using the method we created in order to deserialize the data from the inputXml:
            XMLHelper helper = new XMLHelper();
            ImportSupplierDTO[] suppliersDTO = helper.Deserialize<ImportSupplierDTO[]>(inputXml, "Suppliers");

            var suppliers = new HashSet<Supplier>();

            IMapper mapper = MyMapper();

            foreach (var supplierDTO in suppliersDTO)
            {
                if (string.IsNullOrEmpty(supplierDTO.Name))
                {
                    continue;
                }

                Supplier supplier = mapper.Map<Supplier>(supplierDTO);
                suppliers.Add(supplier);
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();
            return $"Successfully imported {suppliers.Count}";
        }

        //Problem 10
        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            XMLHelper xmlHelper = new XMLHelper();
            ImportPartDTO[] partDTOs = xmlHelper.Deserialize<ImportPartDTO[]>(inputXml, "Parts");

            IMapper mapper = MyMapper();

            var parts = new HashSet<Part>();

            foreach (var partDTO in partDTOs)
            {
                if (string.IsNullOrEmpty(partDTO.Name) || partDTO.SupplierId is null || !context.Suppliers.Any(s => s.Id == partDTO.SupplierId))
                {
                    continue;
                }

                Part part = mapper.Map<Part>(partDTO);
                parts.Add(part);
            }
            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        //Problem 11
        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            XMLHelper helper = new XMLHelper();
            ImportCarDTO[] carsDTO = helper.Deserialize<ImportCarDTO[]>(inputXml, "Cars");
            IMapper mapper = MyMapper();
            var cars = new HashSet<Car>();

            foreach (var carDTO in carsDTO)
            {
                if (string.IsNullOrEmpty(carDTO.Make) || string.IsNullOrEmpty(carDTO.Model))
                {
                    continue;
                }

                Car car = mapper.Map<Car>(carDTO);

                foreach (var partDTO in carDTO.PartCar.DistinctBy(p => p.PartId))
                {
                    if (!context.Parts.Any(p => p.Id == partDTO.PartId))
                    {
                        continue;
                    }
                    PartCar partCar = new PartCar
                    {
                        //No need to inclide Car = car in this case because Entity Framework will do it anyways
                        //Car = car,
                        PartId = partDTO.PartId,
                    };
                    car.PartsCars.Add(partCar);
                }

                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        //Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            IMapper mapper = MyMapper();
            XMLHelper helper = new XMLHelper();
            var customersDTO = helper.Deserialize<ImportCustomerDTO[]>(inputXml, "Customers");

            var customers = new HashSet<Customer>();

            foreach (var customerDTO in customersDTO)
            {
                if (string.IsNullOrEmpty(customerDTO.Name))
                {
                    continue;
                }

                var customer = mapper.Map<Customer>(customerDTO);
                customers.Add(customer);
            }
            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        //Problem 13
        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            IMapper mapper = MyMapper();
            XMLHelper helper = new XMLHelper();
            var salesDTO = helper.Deserialize<ImportSaleDTO[]>(inputXml, "Sales");

            var sales = new HashSet<Sale>();

            foreach (var saleDTO in salesDTO)
            {
                if (!context.Cars.Any(c => c.Id == saleDTO.CarId))
                {
                    continue;
                }

                Sale sale = mapper.Map<Sale>(saleDTO);
                sales.Add(sale);
            }
            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        //Problem 14
        public static string GetCarsWithDistance(CarDealerContext context)
        {
            XMLHelper helper = new XMLHelper();
            IMapper mapper = MyMapper();

            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ProjectTo<ExportCarDTO>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportCarDTO[]>(cars, "cars");
        }

        //Problem 15
        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            XMLHelper helper = new XMLHelper();
            IMapper mapper = MyMapper();

            var BMWDtos = context.Cars
                .Where(c => c.Make.ToUpper() == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ProjectTo<ExportBMWCarDTO>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportBMWCarDTO[]>(BMWDtos, "cars");
        }

        //Problem 16
        public static string GetLocalSuppliers(CarDealerContext context)
        {
            XMLHelper helper = new XMLHelper();
            IMapper mapper = MyMapper();

            var suppliersDTOs = context.Suppliers
                .Where(s => s.IsImporter == false)
                .ProjectTo<ExportSupplierDTO>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize(suppliersDTOs, "suppliers");

        }

        //Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            XMLHelper helper = new XMLHelper();
            IMapper mapper = MyMapper();

            var carsWithPartsDTOs = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ProjectTo<ExpotCarWithPartsDTO>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExpotCarWithPartsDTO[]>(carsWithPartsDTOs, "cars");
        }

        //Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            XMLHelper helper = new XMLHelper();
            IMapper mapper = MyMapper();

            var buyersDTOs = context.Customers
                .Where(c => c.Sales.Count() > 0)
                .Select(c => new ExportCustomerWithCarDTO
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count(),
                    SpentMoney = c.Sales.Sum(s => s.Car.PartsCars
                                                    .Sum(pc => Math.Round(c.IsYoungDriver ? pc.Part.Price * 0.95m : pc.Part.Price, 2)))
                })
                .OrderByDescending(c => c.SpentMoney)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportCustomerWithCarDTO[]>(buyersDTOs, "customers");
        }

        //Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            //Get all sales with information about the car, customer and price of the sale with and without discount.
            //Don't take under consideration the young driver discount!

            XMLHelper helper = new XMLHelper();
            IMapper mapper = MyMapper();

            var salesDTO = context.Sales
                .Select(s => new ExportSaleDTO
                {
                    Car = new ExportCarForSaleDTO()
                            {
                                Make = s.Car.Make,
                                Model = s.Car.Model,
                                TraveledDistance = s.Car.TraveledDistance
                            },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,

                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price),
                    // this formatting is mandatory for Judge!!!
                    PriceWithDiscount = ((s.Car.PartsCars.Sum(pc => pc.Part.Price) - (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (s.Discount / 100))).ToString("0.####;0.####0"))
                })
                .AsNoTracking()
                .ToArray();

            return helper.Serialize(salesDTO, "sales");     
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