using System.Runtime.CompilerServices;
using AutoMapper;
using Blink_API.DTOs.Product;
using Blink_API.DTOs.Category;
using Blink_API.Models;
using Blink_API.DTOs.DiscountDTO;

using Blink_API.DTOs.CategoryDTOs;

using Blink_API.DTOs.ProductDTOs;
 
using Blink_API.DTOs.BrandDtos;
 
using Blink_API.DTOs.BranchDto;
using Blink_API.DTOs.InventoryDTOS;
 
using Blink_API.DTOs.BiDataDtos;
using Microsoft.AspNetCore.Identity;
using Blink_API.DTOs.IdentityDTOs;

using Blink_API.DTOs.CartDTOs;
using Blink_API.Services.PaymentServices;
using Blink_API.DTOs.PaymentCart;

using Blink_API.DTOs.OrdersDTO;


 

using Blink_API.DTOs.IdentityDTOs.UserDTOs;
using Blink_API.DTOs.UsersDtos;
using UserDto = Blink_API.DTOs.UsersDtos.UserDto;





namespace Blink_API.MapperConfigs
{
    public class MapperConfig:Profile
    {
        public MapperConfig()
        {
            // Start Category
            CreateMap<Category, ParentCategoryDTO>().ReverseMap();
            CreateMap<Category, ChildCategoryDTO>().ReverseMap();
            CreateMap<Category, ReadCategoryDTO>()
               .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src =>
                   src.SubCategories.Where(c => !c.IsDeleted)));
            CreateMap<Category, ReadChildCategoryDTO>();
            CreateMap<InsertChildCategoryDTO, Category>()
           .ForMember(dest => dest.CategoryImage, opt => opt.Ignore())
           .ForMember(dest => dest.SubCategories, opt => opt.Ignore()) 
           .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
           .ForMember(dest => dest.ParentCategoryId, opt => opt.Ignore());

            CreateMap<InsertCategoryDTO, Category>()
                .ForMember(dest => dest.CategoryImage, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore()) 
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategoryId, opt => opt.Ignore());

            CreateMap<UpdateParentCategoryDTO, Category>()
            .ForMember(dest => dest.CategoryImage, opt => opt.Ignore())
            .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
            .ForMember(dest => dest.ParentCategory, opt => opt.Ignore());

            CreateMap<UpdateChildCategoryDTO, Category>()
                .ForMember(dest => dest.CategoryImage, opt => opt.Ignore())
                .ForMember(dest => dest.SubCategories, opt => opt.Ignore())
                .ForMember(dest => dest.ParentCategory, opt => opt.Ignore());

            // End Of Category
            //Review Comment
            CreateMap<UserReviewCommentDTO, Review>()
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.ReviewRate))
            .ForMember(dest => dest.CreationDate, opt => opt.MapFrom(src => DateTime.Now))
            .ForMember(dest => dest.ReviewComments, opt => opt.MapFrom(src => new List<ReviewComment>
            {
                new ReviewComment
                {
                    Content = src.Comment,
                    IsDeleted = false
                }
            }))
            .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false)).ReverseMap();
            // End Of Review Comment
            ///////
            CreateMap<Discount, DiscountDetailsDTO>()
                .ForMember(dest => dest.DiscountProducts, option => option.MapFrom(src => src.ProductDiscounts.Select(dp => new DiscountProductDetailsDTO
                {
                    DiscountId = dp.DiscountId,
                    ProductId = dp.ProductId,
                    DiscountAmount = dp.DiscountAmount,
                    IsDeleted = dp.IsDeleted
                })))
                .ReverseMap();
            ///////
            CreateMap<Cart, ReadCartDTO>()
                .ForMember(dest => dest.UserId, option => option.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CartId, option => option.MapFrom(src => src.CartId))
                .ForMember(dest => dest.CartDetails, option => option.MapFrom(src => src.CartDetails.Select(r => new CartDetailsDTO
                {
                    ProductId = r.Product.ProductId,
                    ProductName = r.Product.ProductName,
                    ProductImageUrl = r.Product.ProductImages.FirstOrDefault().ProductImagePath,
                    ProductUnitPrice = r.Product.StockProductInventories.Any() == true ? r.Product.StockProductInventories.Average(p => p.StockUnitPrice) : 0,
                    Quantity = r.Quantity
                }
            ))).ReverseMap();
            ///////
           

            CreateMap<CreateCategoryDTO, Category>();

            ///////
            CreateMap<Product, ProductDiscountsDTO>()
                .ForMember(dest => dest.SupplierName, option => option.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}"))
                .ForMember(dest => dest.BrandName, option => option.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.CategoryName, option => option.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.ProductImages, option => option.MapFrom(src => src.ProductImages.Select(img => img.ProductImagePath).ToList()))
                .ForMember(dest => dest.AverageRate, option => 
                option.MapFrom(src => src.Reviews.Any() == true ? src.Reviews.Average(r => r.Rate) : 0))
                .ForMember(dest => dest.ProductReviews, option => 
                option.MapFrom(src => src.Reviews != null ? src.Reviews.Select(r => new ReviewCommentDTO
                    {
                        Username = r.User != null ? r.User.UserName : "Unknown User",
                        Rate = r.Rate,
                        ReviewComment = r.ReviewComments != null
                            ? r.ReviewComments.Select(rc => rc.Content).ToList()
                            : new List<string>()
                    }).ToList()
                    : new List<ReviewCommentDTO>()))
                .ForMember(dest => dest.CountOfRates, option => option.MapFrom(src => src.Reviews.Select(r => r.ReviewId).Count()))
                .ForMember(dest => dest.ProductPrice, option =>
                option.MapFrom(src => src.StockProductInventories.Any() == true ? src.StockProductInventories.Average(p => p.StockUnitPrice) : 0))
                .ForMember(dest => dest.StockQuantity, option =>
                option.MapFrom(src => src.StockProductInventories.Any() == true ? src.StockProductInventories.Sum(s => s.StockQuantity) : 0))
                .ForMember(dest => dest.DiscountPercentage, option => option.MapFrom(src => src.ProductDiscounts
                .Where(pd => !pd.IsDeleted && pd.Discount.DiscountFromDate <= DateTime.UtcNow && pd.Discount.DiscountEndDate >= DateTime.UtcNow)
                .Select(pd => pd.Discount.DiscountPercentage)
                .FirstOrDefault()))
                .ForMember(dest => dest.DiscountAmount, option => option.MapFrom(src => src.ProductDiscounts
                .Where(pd => !pd.IsDeleted && pd.Discount.DiscountFromDate <= DateTime.UtcNow && pd.Discount.DiscountEndDate >= DateTime.UtcNow)
                .Select(pd => pd.DiscountAmount)
                .FirstOrDefault())).ReverseMap();

            CreateMap<Product, InsertProductDTO>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore());

            CreateMap<InsertProductDTO, Product>()
                .ForMember(dest => dest.ProductImages, opt => opt.Ignore());


            CreateMap<Product, InsertProductDTO>()
                .ForMember(dest => dest.ProductStocks, opt => opt.Ignore());



            CreateMap<UpdateProductDTO, Product>().ReverseMap();

            CreateMap<InsertProductImagesDTO, ProductImage>().ReverseMap();

            CreateMap<InsertFilterAttributeDTO, FilterAttributes>().ReverseMap();
            CreateMap<InsertDefaultAttributesDTO, DefaultAttributes>().ReverseMap();
            CreateMap<ReadFilterAttributesDTO, FilterAttributes>().ReverseMap();
            CreateMap<ReadDefaultAttributesDTO, DefaultAttributes>().ReverseMap();
            CreateMap<InsertProductAttributeDTO, ProductAttributes>().ReverseMap();
            CreateMap<InsertProductStockDTO, StockProductInventory>().ReverseMap();
            //CreateMap<ProductImage, InsertProductImagesDTO>()
            //    .ForMember(dest=>dest.ProductId,option=>option.MapFrom(src => src.Product.ProductId))
            //    .ReverseMap();

            ////////////  
            CreateMap<Brand, BrandDTO>()
                .ReverseMap();

            CreateMap<insertBrandDTO, Brand>()
                .ForMember(dest => dest.BrandImage, option => option.Ignore())
                .ReverseMap();

            ////////////
            CreateMap<Branch, ReadBranchDTO>();
            CreateMap<AddBranchDTO, Branch>();

            /////////////
            CreateMap<Inventory, ReadInventoryDTO>().ForMember(dest => dest.BranchName, option => option.MapFrom(src => src.Branch.BranchName));
            CreateMap<AddInventoryDTO, Inventory>();
            /////////////
            CreateMap<RegisterDto, ApplicationUser>()
                .ForMember(dest => dest.FirstName, option => option.MapFrom(src => src.FName))
                .ForMember(dest => dest.LastName, option => option.MapFrom(src => src.LName))
                .ForMember(dest => dest.Email, option => option.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, option => option.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Address, option => option.MapFrom(src => src.Address))
                .ForMember(dest => dest.UserName, option => option.MapFrom(src => src.UserName))
                .ForMember(dest => dest.LastModification, option => option.MapFrom(src => DateTime.Now))
                .ReverseMap();


            //Payment


            // Mapping from Cart to CartPaymentDTO
            CreateMap<Cart, CartPaymentDTO>()
            .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.CartDetails))
            .ForMember(dest => dest.SubTotal, opt => opt.MapFrom(src =>
                src.CartDetails.Sum(cd =>
                    cd.Quantity *
                    cd.Product.ProductDiscounts
                        .Where(d => !d.IsDeleted)
                        .OrderByDescending(d => d.DiscountAmount)
                        .Select(d => d.DiscountAmount)
                        .FirstOrDefault()
                )))
            .ForMember(dest => dest.ShippingPrice, opt => opt.Ignore()) 
            .ForMember(dest => dest.PaymentStatus, opt => opt.Ignore()) 
            .ForMember(dest => dest.PaymentIntentId, opt => opt.Ignore())
            .ForMember(dest => dest.PaymentMethod, opt => opt.Ignore())
            .ForMember(dest => dest.ClientSecret, opt => opt.Ignore());

            CreateMap<CartDetail, CartDetailsDTO>()
               .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
               .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.ProductName))
               .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
                   src.Product.ProductImages.FirstOrDefault().ProductImagePath)) // Assuming one main image
               .ForMember(dest => dest.ProductUnitPrice, opt => opt.MapFrom(src =>
                   src.Product.ProductDiscounts
                       .Where(d => !d.IsDeleted)
                       .OrderByDescending(d => d.DiscountAmount)
                       .Select(d => d.DiscountAmount)
                       .FirstOrDefault()))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            CreateMap<ReadCartDTO, CartPaymentDTO>().ReverseMap();


            // orderrrrrrr
            CreateMap<OrderHeader, orderDTO>()
            .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderHeaderId))
            .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.OrderSubtotal))
            .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.OrderTax))
            .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.OrderShippingCost))
            .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.OrderTotalAmount))
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails));
            
            CreateMap<OrderDetail, ConfirmedOrderItemDTO>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.product.ProductName))
                .ForMember(dest => dest.ProductImageUrl, opt => opt.MapFrom(src =>
                   src.product.ProductImages != null && src.product.ProductImages.Any()
                         ? src.product.ProductImages.FirstOrDefault().ProductImagePath
                         : null
                     ))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.SellQuantity))
                .ForMember(dest => dest.UnitPrice, opt => opt.MapFrom(src => src.SellPrice));

            //// OrderHeader → orderDTO
            //CreateMap<OrderHeader, orderDTO>()
            //    .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderHeaderId))
            //    .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus))
            //    .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
            //    .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src => src.OrderSubtotal))
            //    .ForMember(dest => dest.Tax, opt => opt.MapFrom(src => src.OrderTax))
            //    .ForMember(dest => dest.Shipping, opt => opt.MapFrom(src => src.OrderShippingCost))
            //    .ForMember(dest => dest.Total, opt => opt.MapFrom(src => src.OrderTotalAmount))
            //    .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.PaymentIntentId))
            //    .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.OrderDetails));

            //// OrderDetail → ConfirmedOrderItemDTO
          
            //// Payment → PaymentDTO
            //CreateMap<Payment, PaymentDTO>()
            //    .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.PaymentIntentId))
            //    .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.Method))
            //    .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
            //    .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.OrderHeader.OrderTotalAmount));










            CreateMap<CartPaymentDTO, CustomerCart>();
            CreateMap<CartPaymentDTO, Cart>().ReverseMap();
            CreateMap<CartPaymentDTO, ReadCartDTO>().ReverseMap();
            //CreateMap<BasketItemDto, BasketItem>();


            // user :
            CreateMap<ApplicationUser, UserDto>()
                  .ForMember(dest => dest.Role, opt => opt.Ignore())   
                 

                .ReverseMap();

            CreateMap<ApplicationUser, AddUserDto>()
                   .ForMember(dest => dest.Role, opt => opt.Ignore())   
                 

                .ReverseMap();

            ////// ************* BIII ******************
            // 1- stock_fact :
            CreateMap<StockProductInventory, stock_factDto>()
                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.Product.ProductId))
                .ForMember(dest => dest.InventoryId, option => option.MapFrom(src => src.Inventory.InventoryId))
                .ForMember(dest => dest.StockUnitPrice, option => option.MapFrom(src => src.StockUnitPrice))
                .ForMember(dest => dest.StockQuantity, option => option.MapFrom(src => src.StockQuantity))
                .ReverseMap();

            // review diminsiion :
            CreateMap<Review, Review_DimensionDto>()

                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.ProductId))
                 .ForMember(dest => dest.ReviewComments, opt => opt.MapFrom(src => src.ReviewComments.Select(c => c.Content).ToList()))

                .ReverseMap();
            //Payment
            CreateMap<CartPaymentDTO, CustomerCart>();
            CreateMap<CartPaymentDTO, Cart>().ReverseMap();
            CreateMap<CartPaymentDTO,ReadCartDTO>().ReverseMap();
            //CreateMap<BasketItemDto, BasketItem>();

            // payment dimension :
            CreateMap<Payment, Payment_DimensionDto>()

                .ForMember(dest => dest.Method, option => option.MapFrom(src => src.Method))
                .ForMember(dest => dest.PaymentDate, option => option.MapFrom(src => src.PaymentDate))
                .ReverseMap();

            // user role :
            CreateMap<IdentityUserRole<string>, UserRoles_DimensionDto>()
                .ForMember(dest => dest.UserId, option => option.MapFrom(src => src.UserId))
                .ForMember(dest => dest.RoleId, option => option.MapFrom(src => src.RoleId))

                .ReverseMap();
            // role dimension :
            CreateMap<IdentityRole, Role_DiminsionDto>()
                .ForMember(dest => dest.RoleId, option => option.MapFrom(src => src.Id))
                .ForMember(dest => dest.RoleName, option => option.MapFrom(src => src.Name))
                .ReverseMap();

            // user dimension :
            CreateMap<ApplicationUser, User_DimensionDto>()
    .ForMember(dest => dest.User_ID, opt => opt.MapFrom(src => src.Id))
    .ForMember(dest => dest.User_Name, opt => opt.MapFrom(src => src.UserName))
    .ForMember(dest => dest.First_Name, opt => opt.MapFrom(src => src.FirstName))
    .ForMember(dest => dest.Last_Name, opt => opt.MapFrom(src => src.LastName))
    .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.PasswordHash))
    .ForMember(dest => dest.Last_Modification, opt => opt.MapFrom(src => src.LastModification))
    .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
    .ForMember(dest => dest.E_mail, opt => opt.MapFrom(src => src.Email))
    .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.PhoneNumber))
    .ForMember(dest => dest.Created_in, opt => opt.MapFrom(src => src.CreatedIn))
    .ForMember(dest => dest.User_Granted, opt => opt.MapFrom(src => src.UserGranted))
    .ReverseMap();

            // product discount :
            CreateMap<ProductDiscount, Product_DiscountDto>()

                .ForMember(dest => dest.DiscountId, option => option.MapFrom(src => src.DiscountId))
                .ForMember(dest => dest.DiscountAmount, option => option.MapFrom(src => src.DiscountAmount))
                .ReverseMap();

            // inventory transaction :

            CreateMap<InventoryTransactionHeader, Inventory_Transaction_Dto>()
               .ForMember(dest => dest.InventoryTransactionHeaderId, option => option.MapFrom(src => src.InventoryTransactionHeaderId))
               .ForMember(dest => dest.InventoryId, option => option.MapFrom(src => src.Inventories.FirstOrDefault().InventoryId))
               .ReverseMap();

            // cart diminsion :
            CreateMap<Cart, cart_DiminsionDto>()
                .ForMember(dest => dest.CreationDate, option => option.MapFrom(src => src.CartDetails.FirstOrDefault().CreationDate))
                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.CartDetails.FirstOrDefault().ProductId))
                .ForMember(dest => dest.Quantity, option => option.MapFrom(src => src.CartDetails.FirstOrDefault().Quantity))
                .ReverseMap();

            // order_fact :
            CreateMap<OrderDetail, order_FactDto>()
                .ForMember(dest => dest.OrderId, option => option.MapFrom(src => src.OrderHeader.OrderHeaderId))
                .ForMember(dest => dest.PaymentId, option => option.MapFrom(src => src.OrderHeader.PaymentId))
                .ForMember(dest => dest.CartId, option => option.MapFrom(src => src.OrderHeader.CartId))
                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.OrderDetailId, option => option.MapFrom(src => src.OrderDetailId))
                .ForMember(dest => dest.OrderDate, option => option.MapFrom(src => src.OrderHeader.OrderDate))
                .ForMember(dest => dest.Subtotal, option => option.MapFrom(src => src.OrderHeader.OrderSubtotal))
                .ForMember(dest => dest.Tax, option => option.MapFrom(src => src.OrderHeader.OrderTax))
                .ForMember(dest => dest.ShippingCost, option => option.MapFrom(src => src.OrderHeader.OrderShippingCost))
                .ForMember(dest => dest.TotalAmount, option => option.MapFrom(src => src.OrderHeader.OrderTotalAmount))
                .ForMember(dest => dest.OrderStatus, option => option.MapFrom(src => src.OrderHeader.OrderStatus))
                .ForMember(dest => dest.Quantity, option => option.MapFrom(src => src.SellQuantity))
                .ForMember(dest => dest.SellPrice, option => option.MapFrom(src => src.SellPrice))

           .ReverseMap();

            // discount :
            CreateMap<Discount, Discount_DimensionDto>()

                .ForMember(dest => dest.DiscountPercentage, option => option.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.DiscountFromDate, option => option.MapFrom(src => src.DiscountFromDate))
                .ForMember(dest => dest.DiscountEndDate, option => option.MapFrom(src => src.DiscountEndDate))
                .ForMember(dest => dest.DiscountAmount, option => option.MapFrom(src => src.ProductDiscounts.FirstOrDefault().DiscountAmount))
                .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.ProductDiscounts.FirstOrDefault().ProductId))
                .ReverseMap();

            // branch inventory :
            CreateMap<Branch, Branch_inventoryDto>()

                .ForMember(dest => dest.InventoryId, option => option.MapFrom(src => src.Inventories.FirstOrDefault().InventoryId))
                .ForMember(dest => dest.InventoryName, option => option.MapFrom(src => src.Inventories.FirstOrDefault().InventoryName))
                .ForMember(dest => dest.InventoryAddress, option => option.MapFrom(src => src.Inventories.FirstOrDefault().InventoryAddress))
                .ForMember(dest => dest.BranchPhone, option => option.MapFrom(src => src.Phone))
                .ForMember(dest => dest.BranchAddress, option => option.MapFrom(src => src.BranchAddress))
                .ForMember(dest => dest.InventoryPhone, option => option.MapFrom(src => src.Inventories.FirstOrDefault().Phone))
                .ReverseMap();


            // inventory transaction fact :
            CreateMap<TransactionDetail, InventoryTransaction_FactDto>()

                .ForMember(dest => dest.TransactionDate, option => option.MapFrom(src => src.InventoryTransactionHeader.InventoryTransactionDate))
                .ForMember(dest => dest.TransactionType, option => option.MapFrom(src => src.InventoryTransactionHeader.InventoryTransactionType))
                .ForMember(dest => dest.InventoryTransactionHeaderId, option => option.MapFrom(src => src.InventoryTransactionHeaderId))
                .ForMember(dest => dest.SrcInventoryId, option => option.MapFrom(src => src.SrcInventory.InventoryId))
                .ForMember(dest => dest.DistInventoryId, option => option.MapFrom(src => src.DistInventory.InventoryId))
                .ForMember(dest => dest.InventoryTransactionHeaderId, option => option.MapFrom(src => src.InventoryTransactionHeader.InventoryTransactionHeaderId))
                // .ForMember(dest => dest.Quantity, option => option.MapFrom(src => src.InventoryTransactionHeader.InventoryTransactionDetails.Sum(s => s.SellQuantity)))
                // .ForMember(dest => dest.ProductId, option => option.MapFrom(src => src.InventoryTransactionHeader.InventoryTransactionDetails.FirstOrDefault().ProductId))

                .ReverseMap();

            // product diminsion :
            CreateMap<Product, Product_DiminsionDto>()

                .ForMember(dest => dest.ParentCategoryId, option => option.MapFrom(src => src.Category.ParentCategoryId))
                .ForMember(dest => dest.CategoryName, option => option.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.CategoryDescription, option => option.MapFrom(src => src.Category.CategoryDescription))
                .ForMember(dest => dest.CategoryImage, option => option.MapFrom(src => src.Category.CategoryImage))
                .ForMember(dest => dest.BrandName, option => option.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.BrandId, option => option.MapFrom(src => src.Brand.BrandId))
                .ForMember(dest => dest.BrandImage, option => option.MapFrom(src => src.Brand.BrandImage))
                .ForMember(dest => dest.BrandDescription, option => option.MapFrom(src => src.Brand.BrandDescription))
                .ForMember(dest => dest.ProductImagePath, option => option.MapFrom(src => src.ProductImages.FirstOrDefault().ProductImagePath))
                // .ForMember(dest => dest.BrandWebSiteURL, option => option.MapFrom(src => src.Brand.WebSiteURL))
                .ReverseMap();





            #region ReviewSuppliedProduct
            CreateMap<InsertReviewSuppliedProductDTO, ReviewSuppliedProduct>();
            CreateMap<ReviewSuppliedProductImages, ReadReviewSuppliedProductImagesDTO>().ReverseMap();
            CreateMap<ReviewSuppliedProduct, ReadReviewSuppliedProductDTO>()
                .ForMember(dest => dest.BrandName, option => option.MapFrom(src => src.Brand.BrandName))
                .ForMember(dest => dest.InventoryName, option => option.MapFrom(src => src.Inventory.InventoryName))
                .ForMember(dest => dest.CategoryName, option => option.MapFrom(src => src.Category.CategoryName))
                .ForMember(dest => dest.SupplierName, option => option.MapFrom(src => $"{src.Supplier.FirstName} {src.Supplier.LastName}"))
                .ForMember(dest => dest.ProductImages, option => option.MapFrom(src => src.ReviewSuppliedProductImages))
                .ForMember(dest=>dest.ProductQuantity,option=>option.MapFrom(src=>src.ProductQuantity))
                .ForMember(dest=>dest.ProductPrice,option=>option.MapFrom(src=>src.ProductPrice))
                .ReverseMap();
            CreateMap<ReviewSuppliedProductImages, ReadReviewSuppliedProductImagesDTO>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImagePath));
            CreateMap<Product, ReadReviewSuppliedProductDTO>()
                .ForMember(dest=>dest.ProductName,option=>option.MapFrom(src=>src.ProductName))
                .ForMember(dest=>dest.ProductDescription,option=>option.MapFrom(src=>src.ProductDescription))
                .ForMember(dest=>dest.SupplierId, option=>option.MapFrom(src=>src.SupplierId))
                .ForMember(dest=>dest.BrandId, option=>option.MapFrom(src=>src.BrandId))
                .ForMember(dest=>dest.CategoryId, option=>option.MapFrom(src=>src.CategoryId))
                .ReverseMap();
            //CreateMap<ReadReviewSuppliedProductImagesDTO, ReviewSuppliedProductImages>().ReverseMap();
            CreateMap<ReadReviewSuppliedProductImagesDTO, ReviewSuppliedProductImages>()
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.RequestId, opt => opt.MapFrom(src => src.RequestId));
            CreateMap<ReviewSuppliedProduct, Product>()
             .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName))
             .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.ProductDescription))
             .ForMember(dest => dest.SupplierId, opt => opt.MapFrom(src => src.SupplierId))
             .ForMember(dest => dest.BrandId, opt => opt.MapFrom(src => src.BrandId))
             .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
             .ForMember(dest=>dest.ProductId,option=>option.Ignore())
             .ForMember(dest=>dest.ProductCreationDate, option=>option.Ignore())
             .ForMember(dest=>dest.ProductModificationDate, option=>option.Ignore())
             .ForMember(dest=>dest.ProductSupplyDate, option=>option.Ignore())
             .ForMember(dest=>dest.IsDeleted, option=>option.Ignore())
             .ForMember(dest=>dest.Category, option=>option.Ignore())
             .ForMember(dest=>dest.Brand, option=>option.Ignore())
             .ForMember(dest=>dest.User, option=>option.Ignore())
             .ForMember(dest=>dest.Reviews, option=>option.Ignore())
             .ForMember(dest=>dest.ProductImages, option=>option.Ignore())
             .ForMember(dest=>dest.TransactionProducts, option=>option.Ignore())
             .ForMember(dest=>dest.OrderDetails, option=>option.Ignore())
             .ForMember(dest=>dest.CartDetails, option=>option.Ignore())
             .ForMember(dest=>dest.WishListDetails, option=>option.Ignore())
             .ForMember(dest=>dest.ProductDiscounts, option=>option.Ignore())
             .ForMember(dest=>dest.StockProductInventories, option=>option.Ignore())
             .ForMember(dest=>dest.ProductAttributes, option=>option.Ignore())
             .ReverseMap();
            #endregion



        }
    }
 
}
