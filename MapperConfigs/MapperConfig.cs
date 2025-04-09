using System.Runtime.CompilerServices;
using AutoMapper;
using Blink_API.DTOs.Product;
using Blink_API.DTOs.Category;
using Blink_API.Models;
using Blink_API.DTOs.DiscountDTO;
using Blink_API.DTOs.CategoryDTOs;

namespace Blink_API.MapperConfigs
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            CreateMap<Category, ParentCategoryDTO>().ReverseMap();
            CreateMap<Category, ChildCategoryDTO>().ReverseMap();
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
                .ForMember(dest => dest.CountOfRates, option => option.MapFrom(src => src.Reviews.Select(r => r.ReviewId).Count()))
                .ForMember(dest => dest.ProductPrice, option => 
                option.MapFrom(src => src.StockProductInventories.Any() == true ? src.StockProductInventories.Average(p => p.StockUnitPrice):0))
                .ForMember(dest=>dest.StockQuantity,option=>
                option.MapFrom(src=>src.StockProductInventories.Any()==true? src.StockProductInventories.Sum(s=>s.StockQuantity) : 0 ))
                .ReverseMap();
            CreateMap<Discount, DiscountDetailsDTO>()
                .ForMember(dest=>dest.DiscountProducts,option=>option.MapFrom(src=>src.ProductDiscounts.Select(dp=>new DiscountProductDetailsDTO
                {
                    DiscountId=dp.DiscountId,
                    ProductId = dp.ProductId,
                    DiscountAmount=dp.DiscountAmount,
                    IsDeleted=dp.IsDeleted
                })))
                .ReverseMap();
            CreateMap<ProductDiscount, DiscountProductDetailsDTO>().ReverseMap();

            CreateMap<CreateCategoryDTO, Category>();

        }
    }
}
