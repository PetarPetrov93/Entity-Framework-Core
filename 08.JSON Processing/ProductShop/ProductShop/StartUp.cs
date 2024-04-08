using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //UNCOMMENT TASKS INPUT/OUTPUT BEFORE EXECUTING:

            //Problem 1
            //This is how to get the text from the json file, using relative path
            //string inputJsonPr1 = File.ReadAllText(@"../../../Datasets/users.json");

            //string message = ImportUsers(context, inputJsonPr1);
            //Console.WriteLine(message);

            //Problem 2
            //string inputJsonPr2 = File.ReadAllText(@"../../../Datasets/products.json");
            //string message = ImportProducts(context, inputJsonPr2);
            //Console.WriteLine(message);

            //Problem 3
            //string inputJsonPr3 = File.ReadAllText(@"../../../Datasets/categories.json");
            //string message = ImportCategories(context, inputJsonPr3);
            //Console.WriteLine(message);

            //Problem 4
            //string inputJsonPr4 = File.ReadAllText(@"../../../Datasets/categories-products.json");
            //string message = ImportCategoryProducts(context, inputJsonPr4);
            //Console.WriteLine(message);

            //Problem 5
            //string message5 = GetProductsInRange(context);
            //Console.WriteLine(message5);

            //Problem 6
            //string message6 = GetSoldProducts(context);
            //Console.WriteLine(message6);
            
            //Problem 7
            //string message7 = GetCategoriesByProductsCount(context);
            //Console.WriteLine(message7);

            //Problem 8
            string message8 = GetUsersWithProducts(context);
            Console.WriteLine(message8);

        }

        //Problem 1
        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            //This is how to use a given automapper profile (ProductShopProfile)
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            //DeserializeObject<T> is used here because the json file is an array of objects, if it was a single object we would use DeserializeObject()
            var userDTOs = JsonConvert.DeserializeObject<ImportUserDTO[]>(inputJson);

            //We will store the users here so that we don't have to call savechanges for every single one of them in the foreach loop
            ICollection<User> usersToAddToTheDB = new HashSet<User>();

            //mapping each userDTO which we received from the JSON file to a User
            foreach (var userDTO in userDTOs)
            {
                User user = mapper.Map<User>(userDTO);
                usersToAddToTheDB.Add(user);
            }

            //adding the whole hashset of users to the Users table
            context.Users.AddRange(usersToAddToTheDB);
            context.SaveChanges();

            return $"Successfully imported {usersToAddToTheDB.Count}";
        }

        //Problem 2
        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            //This can be extracted into a separate static method for better code quality, however I'd rather writing it in each method for practice purposes
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var productsToAddDTO = JsonConvert.DeserializeObject<ImportProductDTO[]>(inputJson);

            // This is an alternative to the foreach from the above method. This can be used when we do not need to do any validations.
            Product[] productsToAdd = mapper.Map<Product[]>(productsToAddDTO);

            context.Products.AddRange(productsToAdd);
            context.SaveChanges();
            return $"Successfully imported {productsToAdd.Length}";
        }

        //Problem 3
        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var categoriesDTO = JsonConvert.DeserializeObject<ImportCategoryDTO[]>(inputJson);

            var categoriesToAdd = new HashSet<Category>();

            foreach (var cat in categoriesDTO!)
            {
                if (cat.Name is not null)
                {
                    var catToAdd = mapper.Map<Category>(cat);
                    categoriesToAdd.Add(catToAdd);
                }
            }

            context.Categories.AddRange(categoriesToAdd);
            context.SaveChanges();

            return $"Successfully imported {categoriesToAdd.Count}";
        }

        //Problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var catPrDTO = JsonConvert.DeserializeObject<ImportCategoryProductDTO[]>(inputJson);

            var allCatPrToAdd = new HashSet<CategoryProduct>();

            foreach (var catPr in catPrDTO!)
            {
                //This is a good validation. It checks if the given IDs from the Json file are indeed valid ProductId and CategoryId
                //If there are no Product or Category with the given Id this means that the data given in the Json file is invalid
                //DO NOT USE FOR JUDGE!
                if (!context.Products.Any(p => p.Id == catPr.ProductId) ||
                    !context.Categories.Any(c => c.Id == catPr.CategoryId))
                {
                    continue;
                }
                var catPrToAdd = mapper.Map<CategoryProduct>(catPr);
                allCatPrToAdd.Add(catPrToAdd);
            }

            context.CategoriesProducts.AddRange(allCatPrToAdd);
            context.SaveChanges();

            return $"Successfully imported {allCatPrToAdd.Count}";
        }

        //Problem 5 (Exporting to Json) with DTO
        public static string GetProductsInRange(ProductShopContext context)
        {
            //THIS IS HOW TO EXPORT OBJECTS TO JSON USING ANNONIMOUS OBJECT:
            //var productsToExp = context.Products
            //        .Where(p => p.Price >= 500 && p.Price <= 1000)
            //        .OrderBy(p => p.Price)
            //        .Select(p => new
            //        {
            //            name = p.Name,
            //            price = p.Price,
            //            seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
            //        })
            //        .AsNoTracking()
            //        .ToArray();

            //THIS IS HOW TO EXPORT OBJECTS TO JSON USING AUTOMAPPER:
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));

            var productsToExp = context.Products
                    .Where(p => p.Price >= 500 && p.Price <= 1000)
                    .OrderBy(p => p.Price)
                    .AsNoTracking()
                    .ProjectTo<ExportProductInRangeDTO>(mapper.ConfigurationProvider)
                    .ToArray();

            return JsonConvert.SerializeObject(productsToExp, Formatting.Indented);
        }

        //Problem 6 (Exporting to Json) with anonymous object
        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(ps => ps.Buyer != null))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    soldProducts = u.ProductsSold
                        .Select(p => new
                        {
                            p.Name,
                            p.Price,
                            BuyerFirstName = p.Buyer.FirstName,
                            BuyerLastName = p.Buyer.LastName
                        })
                        .ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .AsNoTracking()
                .ToArray();


            //Here we pass the JsonSerializerSettings as the last argument:
            string output = JsonConvert.SerializeObject (users, Formatting.Indented, PascalCaseToCamelCaseConverter());
            return output;
        }

        //Problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categorias = context.Categories
                .Select(c => new
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count(),

                    AveragePrice = (c.CategoriesProducts.Any() ?
                        c.CategoriesProducts.Average(cp => cp.Product.Price) : 0).ToString("f2"),

                    TotalRevenue = (c.CategoriesProducts.Any() ?
                        c.CategoriesProducts.Sum(cp => cp.Product.Price) : 0).ToString("f2")
                })
                .OrderByDescending(c => c.ProductsCount)
                .AsNoTracking()
                .ToArray();

            string result = JsonConvert.SerializeObject(categorias, Formatting.Indented, PascalCaseToCamelCaseConverter());
            return result;
        }

        //Problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            //Since the default config of the serializer in the method does not include NullValueHandling
            //I'm initializing a new JsonSerialiserSettings here and adding this setting to it:
            JsonSerializerSettings ignoreNullSettings = PascalCaseToCamelCaseConverter();
            ignoreNullSettings.NullValueHandling = NullValueHandling.Ignore;

            var usersInfo = context.Users
                .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                .Select(u => new
                {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = new
                    {
                        Count = u.ProductsSold.Count(p => p.Buyer != null),
                        Products = u.ProductsSold
                            .Where(p => p.Buyer != null)
                            .Select(p => new
                            {
                                p.Name,
                                p.Price
                            })
                            .ToArray()
                    }
                    
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .AsNoTracking()
                .ToArray();
            var users = new
            {
                UsersCount = usersInfo.Length,
                Users = usersInfo
            };

            string output = JsonConvert.SerializeObject(users, Formatting.Indented, ignoreNullSettings);
            return output;
        }

        //This will convert PascalCase to camelCase for the properties names frtom the anonymous object
        //since the C# convention is PascalCase and for the tasks it is required the Json files to be in camelCase
        public static JsonSerializerSettings PascalCaseToCamelCaseConverter()
        {
            //creating a new setting:
            DefaultContractResolver contractResolver = new DefaultContractResolver()
            {
                NamingStrategy = new CamelCaseNamingStrategy(false, true)
            };

            //passing the created setting to the JsonSerializer which will change the convention from PascalCase to camelCase in the document
            JsonSerializerSettings pascalCaseToCamelCase = new JsonSerializerSettings()
            {
                ContractResolver = contractResolver,
            };
            return pascalCaseToCamelCase;
        }
    }
}