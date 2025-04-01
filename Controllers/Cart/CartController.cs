﻿using System.Buffers.Text;
using Blink_API.Models;
using Blink_API.Services.CartService;
using Blink_API.Services.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Blink_API.Controllers.Cart
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly CartService cartService;

        public CartController(CartService _cartService)
        {
            cartService = _cartService;
        }

        [HttpGet]
        public async Task<ActionResult> GetAllProducts()
        {
            var carts = await cartService.GetAllCarts();
            if (carts == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var cart in carts)
            {
                foreach (var detail in cart.CartDetails)
                {
                    detail.ProductImageUrl = $"{baseUrl}{detail.ProductImageUrl.Replace("wwwroot/", "")}";
                }
            }
            return Ok(carts);
        }

        [HttpGet("GetById/{id}")]
        public async Task<ActionResult> GetByUserId(string id)
        {
            var cart = await cartService.GetByUserId(id);
            if (cart == null)
                return NotFound();
            string baseUrl = $"{Request.Scheme}://{Request.Host}/";
            foreach (var detail in cart.CartDetails)
            {
                detail.ProductImageUrl = $"{baseUrl}{detail.ProductImageUrl.Replace("wwwroot/", "")}";
            }
            return Ok(cart);
        }
    }
}
