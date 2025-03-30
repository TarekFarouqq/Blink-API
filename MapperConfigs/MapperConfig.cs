using System.Runtime.CompilerServices;
using AutoMapper;
using Blink_API.DTOs.Product;
using Blink_API.Models;

namespace Blink_API.MapperConfigs
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Product, ProductDetailsDTO>()
                .ForMember(dest => dest.SupplierName, option => option.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.BrandName, option => option.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.CategoryName, option => option.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ProductImages, option => option.MapFrom(src => src.ProductImages.Select(img => img.ProductImagePath).ToList()))
                .ForMember(dest => dest.AverageRate, option => option.MapFrom(src => src.Reviews.Any() == true ? src.Reviews.Average(r => r.Rate) : 0))
                .ForMember(dest => dest.ProductReviews, option => option.MapFrom(src => src.Reviews.Select(r => new ReviewCommentDTO
                {
                    Rate = r.Rate,
                    ReviewComment = r.ReviewComments.Select(rc => rc.Content).ToList()
                })))
                .ForMember(dest => dest.CountOfRates, option => option.MapFrom(src => src.Reviews.Count()))
                .ReverseMap();
        }
    }
}
