﻿using System.IO;
using System.Reflection.Metadata.Ecma335;
using Blink_API.DTOs.Product;
using Blink_API.DTOs.ProductDTOs;
using Blink_API.Models;
using Blink_API.Services.Product;
using Blink_API.Services.ProductServices;
using Microsoft.AspNetCore.Mvc;

namespace Blink_API.Controllers.Product
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly ProductService productService;
        private readonly ReviewSuppliedProductService reviewSuppliedProductsService;
        private readonly ProductReviewService productReviewService;
        public ProductController(ProductService _productService , ReviewSuppliedProductService _reviewSuppliedProductsService, ProductReviewService _productReviewService)
        {
            productService = _productService;
            reviewSuppliedProductsService = _reviewSuppliedProductsService;
            productReviewService = _productReviewService;
        }
        [HttpGet]
        public async Task<ActionResult> GetAll()
        {
            var products = await productService.GetAll();
            if (products == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var product in products)
            {
                product.ProductImages = product.ProductImages.Select(img => $"{baseUrl}{img.Replace("wwwroot/", "")}").ToList();
            }
            return Ok(products);
        }
        [HttpGet("GetPagesCount/{pgSize}")]
        public async Task<ActionResult> GetPagesCount(int pgSize)
        {
            var count = await productService.GetPagesCount(pgSize);
            if (count == 0)
                return NotFound();
            return Ok(count);
        }
        [HttpGet("GetAllWithPaging/{pgNumber}/{pgSize}")]
        public async Task<ActionResult> GetAllPagginated(int pgNumber,int pgSize)
        {
            var products = await productService.GetAllPagginated(pgNumber,pgSize);
            if (products == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var product in products)
            {
                product.ProductImages = product.ProductImages.Select(img => $"{baseUrl}{img.Replace("wwwroot/", "")}").ToList();
            }
            return Ok(products);
        }
        [HttpGet("GetFilteredProducts/{filter}/{pgNumber}/{pgSize}")]
        public async Task<ActionResult> GetFilteredProducts(string filter, int pgNumber, int pgSize)
        {
            var products = await productService.GetFilteredProducts(filter, pgNumber, pgSize);
            if (products == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var product in products)
            {
                product.ProductImages = product.ProductImages.Select(img => $"{baseUrl}{img.Replace("wwwroot/", "")}").ToList();
            }
            return Ok(products);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult> GetById(int id)
        {
            var product = await productService.GetById(id);  
            if(product == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            product.ProductImages = product.ProductImages.Select(img => $"{baseUrl}{img.Replace("wwwroot/", "")}").ToList();
            return Ok(product);
        }
        [HttpGet("GetByChildCategory/{id}")]
        public async Task<ActionResult> GetByChildCategory(int id)
        {
            var products = await productService.GetByChildCategoryId(id);
            if (products == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var product in products)
            {
                product.ProductImages = product.ProductImages.Select(img => $"{baseUrl}{img.Replace("wwwroot/", "")}").ToList();
            }
            return Ok(products);
        }
        [HttpGet("GetByParentCategory/{id}")]
        public async Task<ActionResult> GetByParentCategory(int id)
        {
            var products = await productService.GetByParentCategoryId(id);
            if (products == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var product in products)
            {
                product.ProductImages = product.ProductImages.Select(img => $"{baseUrl}{img.Replace("wwwroot/", "")}").ToList();
            }
            return Ok(products);
        }
        [HttpPost]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Add([FromForm] InsertProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }
            if (productDTO == null)
                return BadRequest();
            var result = await productService.Add(productDTO);
            if (result == 0)
                return BadRequest();
            return Ok(result);
        }
        [HttpPut("{id}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> Update(int id, [FromForm] UpdateProductDTO productDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(errors);
            }
            if (productDTO == null)
                return BadRequest();
            await productService.Update(id, productDTO);
            return Ok();
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            if (id <= 0)
                return BadRequest();
            await productService.Delete(id);
            return Ok();
        }
        #region FilterAttributes
        [HttpGet("GetFilterAttributes")]
        public async Task<ActionResult> GetFilterAttributes()
        {
            var Attributes = await productService.GetFilterAttributesAsync();
            return Ok(Attributes);
        }
        [HttpGet("GetFilterAttributeById/{id}")]
        public async Task<ActionResult> GetFilterAttributeById(int id)
        {
            var Attribute = await productService.GetFilterAttributeById(id);
            return Ok(Attribute);
        }
        [HttpPost("AddFilterAttribute")]
        public async Task<ActionResult> AddFilterAttribute(InsertFilterAttributeDTO filterAttributes)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (filterAttributes == null)
                return BadRequest();
            var result = await productService.AddFilterAttribute(filterAttributes);
            if(result.StatusCode != 200)
                return BadRequest("There is an error happened");
            return Ok("Attribute Saved Success");
        }
        [HttpGet("GetDefaulAttributesByAttributeId/{id}")]
        public async Task<ActionResult> GetDefaulAttributesByAttributeId(int id)
        {
            var attributes = await productService.GetDefaultAttributesByAttributeId(id);
            return Ok(attributes);
        }
        [HttpPost("AddDefaultAttributes")]
        public async Task<ActionResult> AddDefaultAttributes(InsertDefaultAttributesDTO attributes)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (attributes == null)
                return BadRequest();
            await productService.AddDefaultAttribute(attributes);
            return Ok("Default Attributes Inserted Success");
        }
        [HttpPost("AddProductAttribute/{productId}")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddProductAttribute(int productId, [FromForm] ICollection<InsertProductAttributeDTO> attributes)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (attributes.Count == 0)
                await productService.DeleteProductAttributes(productId);
                //return BadRequest("List Was Empty");
            await productService.AddProductAttribute(attributes);
            return Ok(new { success = "ProductAttributes Inserted Success" });
        }
        [HttpGet("GetProductAttributes/{id}")]
        public async Task<ActionResult> GetProductAttributes(int id)
        {
            var productAttributes = await productService.GetProductAttributes(id);
            return Ok(productAttributes);
        }
        [HttpGet("GetFillteredProducts/{pgNumber}/{fromPrice}/{toPrice}/{rating}")]
        public async Task<ActionResult> GetFillteredProducts(int pgNumber,decimal fromPrice,decimal toPrice,int rating)
        {
            var filters = HttpContext.Request.Query;
            var filtersProduct = new Dictionary<int, List<string>>();
            foreach (var key in filters.Keys)
            {
                if (int.TryParse(key, out int attributeId))
                {
                    if (!filtersProduct.ContainsKey(attributeId))
                    {
                        filtersProduct[attributeId] = new List<string>();
                    }

                    filtersProduct[attributeId].AddRange(filters[key]);
                }
            }
            var products = await productService.GetFillteredProducts(filtersProduct, pgNumber,fromPrice,toPrice,rating);
            return Ok(products);
        }
        #endregion
        #region Product Stock
        [HttpGet("GetProductStock/{id}")]
        public async Task<ActionResult> GetProductStock(int id)
        {
            var productStock = await productService.GetProductStock(id);
            if (productStock == null)
                return NotFound();
            return Ok(productStock);
        }
        #endregion
        #region ReviewSuppliedProduct
        [HttpGet("GetSuppliedProducts")]
        public async Task<ActionResult> GetSuppliedProducts()
        {
            var reviewSuppliedProducts = await reviewSuppliedProductsService.GetSuppliedProducts();
            if (reviewSuppliedProducts == null)
                return NotFound();
            return Ok(reviewSuppliedProducts);
        }
        [HttpGet("GetSuppliedProductByRequestId/{requestId}")]
        public async Task<ActionResult> GetSuppliedProductByRequestId(int requestId)
        {
            var reviewSuppliedProduct = await reviewSuppliedProductsService.GetSuppliedProductByRequestId(requestId);
            if (reviewSuppliedProduct == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            if(reviewSuppliedProduct.ProductImages.Count > 0)
            {
                foreach (var ProductImage in reviewSuppliedProduct.ProductImages)
                {
                    if (ProductImage != null)
                    {
                        ProductImage.ImageUrl = $"{baseUrl}{ProductImage.ImageUrl.Replace("wwwroot/", "").TrimStart('/')}";
                    }
                }
            }
            return Ok(reviewSuppliedProduct);
        }
        [HttpPost("AddRequestSuppliedProduct")]
        [Consumes("multipart/form-data")]
        public async Task<ActionResult> AddRequestProduct([FromForm]InsertReviewSuppliedProductDTO insertReviewSuppliedProductDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            if (insertReviewSuppliedProductDTO == null)
                return BadRequest();
            await reviewSuppliedProductsService.AddRequestProduct(insertReviewSuppliedProductDTO);
            return Ok();
        }
        [HttpPut("UpdateRequestSuppliedProduct/{requestId}")]
        public async Task<ActionResult> UpdateRequestSupplierProduct( int requestId, [FromBody]ReadReviewSuppliedProductDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { error = "Model State is Invalid" });
            if(model == null)
                return BadRequest(new { error = "Model is Null" });
            await reviewSuppliedProductsService.UpdateRequestProduct(requestId,model);
            return Ok(new { success = "Product Reviewed Success" });
        }
        #endregion
        #region Sprint3
        [HttpGet("CheckUserAvailableToReview/{userId}/{productId}")]
        public async Task<ActionResult> CheckUserAvailableToReview(string userId, int productId)
        {
            var result = await productService.CheckUserAvailableToReview(userId, productId);
            return Ok(new {userAllow= result });
        }
        [HttpPost("AddReview")]
        public async Task<ActionResult> AddReview(UserReviewCommentDTO reviewCommentDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Model State is Invalid" });
            if (reviewCommentDTO == null)
                return BadRequest(new { Message = "Model is Null" });
            var result = await productReviewService.AddRevew(reviewCommentDTO);
            if (result)
            {
                return Ok(new { Message = "Review Added Success" });
            }
            else
            {
                return BadRequest(new { Message = "There is an error happened" });
            }
        }
        #endregion
    }
}
