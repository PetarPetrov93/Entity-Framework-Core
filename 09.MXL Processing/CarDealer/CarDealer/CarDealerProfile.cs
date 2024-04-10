using AutoMapper;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;

namespace CarDealer
{
    public class CarDealerProfile : Profile
    {
        public CarDealerProfile()
        {
            //Supplier
            CreateMap<ImportSupplierDTO, Supplier>();

            CreateMap<Supplier, ExportSupplierDTO>()
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Parts.Count()));

            //Part
            CreateMap<ImportPartDTO, Part>()
                .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId.Value));

            CreateMap<Part, ExportPartDTO>();

            //Car
            CreateMap<ImportCarDTO, Car>()
                .ForSourceMember(src => src.PartCar, opt => opt.DoNotValidate());

            CreateMap<Car, ExportCarDTO>();
            CreateMap<Car, ExportBMWCarDTO>();

            CreateMap<Car, ExpotCarWithPartsDTO>()
                .ForMember(dest => dest.Parts, opt => opt.MapFrom(src => src.PartsCars
                                                                    .Select(pc => pc.Part)
                                                                    .OrderByDescending(p => p.Price)
                                                                    .ToArray()));

            //Customer
            CreateMap<ImportCustomerDTO, Customer>();


            //Sale
            CreateMap<ImportSaleDTO, Sale>();
        }
    }
}
