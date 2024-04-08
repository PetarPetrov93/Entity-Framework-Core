using AutoMapper;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;

namespace ProductShop
{
    //using the Automapper and ingerrining it's Profile class to crate our own custom class and inside we are creating maps for DTOs-to-Models
    public class ProductShopProfile : Profile
    {
        public ProductShopProfile() 
        {
            //User
            CreateMap<ImportUserDTO, User>();

            //Product
            //ForImport:
            CreateMap<ImportProductDTO, Product>();
            //ForExport (This customisation is needed because the property names in the DTO class and the Product class differ in names):
            CreateMap<Product, ExportProductInRangeDTO>()
                .ForMember(d => d.ProductName, opt => opt.MapFrom(pr => pr.Name))
                .ForMember(d => d.ProductPrice, opt => opt.MapFrom(pr => pr.Price))
                .ForMember(d => d.Seller, opt => opt.MapFrom(pr => $"{pr.Seller.FirstName} {pr.Seller.LastName}"));

            //Category
            CreateMap<ImportCategoryDTO, Category>();

            //CategoryProduct
            CreateMap<ImportCategoryProductDTO, CategoryProduct>();
        }
    }
}
