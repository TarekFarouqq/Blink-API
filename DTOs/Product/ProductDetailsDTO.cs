using Blink_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Blink_API.DTOs.Product
{
    public class ProductDetailsDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public DateTime ProductCreationDate { get; set; }
        public DateTime ProductModificationDate { get; set; }
        public DateTime ProductSupplyDate { get; set; }
        public List<string> ProductImages { get; set; } 
        public string SupplierName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int CountOfRates { get; set; }
        public double AverageRate { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<ReviewCommentDTO> ProductReviews { get; set; }=new List<ReviewCommentDTO>();
    }
    public class ReviewCommentDTO
    {
        public int Rate { get; set; }
        public ICollection<string> ReviewComment { get; set; }

    }
}
