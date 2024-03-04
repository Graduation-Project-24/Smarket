using AutoMapper;
using Smarket.Models;
using Smarket.Models.DTOs;



namespace Smarket.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Package, PackageDto>().ReverseMap();
        }
    }
}