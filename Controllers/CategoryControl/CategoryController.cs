﻿using Blink_API.DTOs.Category;
using Blink_API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blink_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryService categoryService;
        public CategoryController(CategoryService _categoryService)
        {
            categoryService = _categoryService;
        }
        [HttpGet("GetParentCategories")]
        public async Task<ActionResult> GetParentCategories()
        {
            var categories = await categoryService.GetParentCategories();
            if (categories == null) return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var category in categories)
            {
                category.CategoryImage = baseUrl + category.CategoryImage.Replace("wwwroot", "");
            }
            return Ok(categories);
        }
        [HttpGet("GetChildCategories")]
        public async Task<ActionResult> GetChildCategories()
        {
            var categories = await categoryService.GetChildCategories();
            if (categories == null) return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var category in categories)
            {
                category.CategoryImage = baseUrl + category.CategoryImage.Replace("wwwroot", "");
            }
            return Ok(categories);
        }
        [HttpGet("GetParentCategoryById")]
        public async Task<ActionResult> GetParentCategoryById(int id)
        {
            var category = await categoryService.GetParentCategoryById(id);
            if (category == null) return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            category.CategoryImage = baseUrl + category.CategoryImage.Replace("wwwroot", "");
            return Ok(category);
        }
        [HttpGet("GetChildCategoryById")]
        public async Task<ActionResult> GetChildCategoryById(int id)
        {
            var category = await categoryService.GetChildCategoryById(id);
            if (category == null) return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            category.CategoryImage = baseUrl + category.CategoryImage.Replace("wwwroot", "");
            return Ok(category);
        }
        [HttpGet("GetChildCategoryByParentId")]
        public async Task<ActionResult> GetChildCategoryByParentId(int id)
        {
            var categories = await categoryService.GetChildCategoryByParentId(id);
            if (categories == null) return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}";
            foreach (var category in categories)
            {
                category.CategoryImage = baseUrl + category.CategoryImage.Replace("wwwroot", "");
            }
            return Ok(categories);
        }
    }
}
