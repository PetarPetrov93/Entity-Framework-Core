using AutoMapper;
using Newtonsoft.Json.Serialization;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile()
        {
            //User
            CreateMap<ImportUserDTO, User>();

            //Product
            CreateMap<ImportProductDTO, Product>();

            CreateMap<Product, ExportProductsInRangeDTO>()
                .ForMember(dest => dest.Buyer, opt => opt.MapFrom(p => p.Buyer.FirstName + " " + p.Buyer.LastName));

            CreateMap<Product, ExportProductForUsersWithSoldItemsDTO>();

            //Category
            CreateMap<ImportCategoryDTO, Category>();

            //CategoryProduct
            CreateMap<ImportCategoryProductDTO, CategoryProduct>();
        }
    }
}
