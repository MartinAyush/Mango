using AutoMapper;
using Mango.Services.OrderApi.Models;
using Mango.Services.OrderApi.Models.Dto;

namespace Mango.Services.OrderApi
{
	public class MappingConfig
	{
		public static MapperConfiguration RegisterMaps()
		{
			var mappingConfig = new MapperConfiguration(config =>
			{
				config.CreateMap<OrderHeader, OrderHeaderDto>().ReverseMap();
                config.CreateMap<OrderDetails, OrderDetailDto>().ReverseMap();
				config.CreateMap<OrderHeaderDto, CartHeaderDto>()
					.ForMember(dest => dest.CartTotal, u => u.MapFrom(src => src.OrderTotal)).ReverseMap();

				config.CreateMap<CartDetailsDto, OrderDetailDto>()
					.ForMember(dest => dest.ProductName, u => u.MapFrom(src => src.Product.Name))
					.ForMember(dest => dest.Price, u => u.MapFrom(src => src.Product.Price));

				config.CreateMap<OrderHeaderDto, CartHeaderDto>();
            });

			return mappingConfig;
		}
	}
}
