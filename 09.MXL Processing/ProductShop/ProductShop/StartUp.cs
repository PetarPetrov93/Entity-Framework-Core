using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using ProductShop.Utilities;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            using ProductShopContext context = new ProductShopContext();

            //UNCOMMENT the required problem:

            //Problem 1
            //string input1 = File.ReadAllText("../../../Datasets/users.xml");
            //string result1 = ImportUsers(context, input1);
            //Console.WriteLine(result1);

            //Problem 2
            //string input2 = File.ReadAllText("../../../Datasets/products.xml");
            //string result2 = ImportProducts(context, input2);
            //Console.WriteLine(result2);

            //Problem 3
            //string input3 = File.ReadAllText("../../../Datasets/categories.xml");
            //string result3 = ImportCategories(context, input3);
            //Console.WriteLine(result3);

            //Problem 4
            //string input4 = File.ReadAllText("../../../Datasets/categories-products.xml");
            //string result4 = ImportCategoryProducts(context, input4);
            //Console.WriteLine(result4);

            //Problem 5
            //string output5 = GetProductsInRange(context);
            //Console.WriteLine(output5);

            //Problem 6
            //string output6 = GetSoldProducts(context);
            //Console.WriteLine(output6);

            //Problem 7
            //string output7 = GetCategoriesByProductsCount(context);
            //Console.WriteLine(output7);

            //Problem 8
            //string output8 = GetUsersWithProducts(context);
            //Console.WriteLine(output8);
        }

        //Problem 1
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            IMapper mapper = MyMapper();

            var usersDTOs = xmlHelper.Deserialize<ImportUserDTO[]>(inputXml, "Users");

            HashSet<User> users = new HashSet<User>();

            foreach (var userDTO in usersDTOs)
            {
                if (string.IsNullOrEmpty(userDTO.FirstName) || string.IsNullOrEmpty(userDTO.LastName))
                {
                    continue;
                }

                User user = mapper.Map<User>(userDTO);
                users.Add(user);
            }
            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        //Problem 2
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            IMapper mapper = MyMapper();

            var productsDTOs = xmlHelper.Deserialize<ImportProductDTO[]>(inputXml, "Products");

            HashSet<Product> products = new HashSet<Product>();

            foreach (var productDTO in productsDTOs)
            {
                if (string.IsNullOrEmpty(productDTO.Name))
                {
                    continue;
                }


                Product product = mapper.Map<Product>(productDTO);
                products.Add(product);
            }
            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        //Problem 3
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            IMapper mapper = MyMapper();

            var categoriesDTOs = xmlHelper.Deserialize<ImportCategoryDTO[]>(inputXml, "Categories");

            HashSet<Category> categories = new HashSet<Category>();
            foreach (var categoryDTO in categoriesDTOs)
            {
                if (string.IsNullOrEmpty(categoryDTO.Name))
                {
                    continue;
                }
                Category category = mapper.Map<Category>(categoryDTO);
                categories.Add(category);
            }
            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        //Problem 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlHelper xmlHelper = new XmlHelper();
            IMapper mapper = MyMapper();

            var categoryProductDTOs = xmlHelper.Deserialize<ImportCategoryProductDTO[]>(inputXml, "CategoryProducts");

            HashSet<CategoryProduct> categoryProducts = new HashSet<CategoryProduct>();

            foreach (var cpDTO in categoryProductDTOs)
            {
                if (!context.Products.Any(p => p.Id == cpDTO.ProductId) ||
                    !context.Categories.Any(c => c.Id == cpDTO.CategoryId))
                {
                    continue;
                }
                CategoryProduct categoryProduct = mapper.Map<CategoryProduct>(cpDTO);
                categoryProducts.Add(categoryProduct);
            }
            context.CategoryProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        //Problem 5
        public static string GetProductsInRange(ProductShopContext context)
        {
            IMapper mapper = MyMapper();
            XmlHelper helper = new XmlHelper();

            var productsDTOs = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Take(10)
                .ProjectTo<ExportProductsInRangeDTO>(mapper.ConfigurationProvider)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportProductsInRangeDTO[]>(productsDTOs, "Products");
        }

        //Problem 6
        public static string GetSoldProducts(ProductShopContext context)
        {
            XmlHelper helper = new XmlHelper();

            var usersWithSoldProductsDTO = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .Select(u => new ExportUserWithSoldItemDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Products = u.ProductsSold.Select(ps => new ExportProductForUsersWithSoldItemsDTO()
                                                    {
                                                        Name = ps.Name,
                                                        Price = ps.Price
                                                    }).ToArray()
                })
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportUserWithSoldItemDTO[]>(usersWithSoldProductsDTO, "Users");
        }

        //Problem 7
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            XmlHelper helper = new XmlHelper();

            var categoriesDTOs = context.Categories
                .Select(c => new ExportCategoryDTO
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Any() ? 
                                   c.CategoryProducts.Average(cp => cp.Product.Price) : 0,

                    TotalRevenue = c.CategoryProducts
                                    .Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .AsNoTracking()
                .ToArray();

            return helper.Serialize<ExportCategoryDTO[]>(categoriesDTOs, "Categories");
        }

        //Problem 8
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            XmlHelper helper = new XmlHelper();

            var users = new ExportUserWithCountAndProductsDTO
            {
                Count = context.Users.Count(x => x.ProductsSold.Any()),
                Users = context.Users.Where(x => x.ProductsSold.Count() > 0)
                               .Select(u => new ExportUserWithProductsDTO
                               {
                                   Firstname = u.FirstName,
                                   Lastname = u.LastName,
                                   Age = u.Age,
                                   SoldProducts = new ExportSoldProductDTO
                                   {
                                       Count = u.ProductsSold.Count,
                                       SoldProducts = u.ProductsSold.Select(s =>
                                       new ExportProductMiniDTO
                                       {
                                           Name = s.Name,
                                           Price = s.Price
                                       })
                                       .OrderByDescending(y => y.Price)
                                       .ToArray()
                                   }
                               })
                               .OrderByDescending(v => v.SoldProducts.Count)
                               .Take(10)
                               .AsNoTracking()
                               .ToList()
            };

            return helper.Serialize<ExportUserWithCountAndProductsDTO>(users, "Users");
        }


        public static IMapper MyMapper()
        {
            IMapper mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductShopProfile>();
            }));
            return mapper;
        }
    }
}