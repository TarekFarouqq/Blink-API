﻿using Blink_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Blink_API.Repositories.BiDataRepos
{
    public class Product_DiminsionRepos:GenericRepo<Product,int>


    {
        private readonly BlinkDbContext _context;
        public Product_DiminsionRepos(BlinkDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<List<Product>> GetAllProducts()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImages)
                .Include(p => p.ProductAttributes)
                .ToListAsync();
        }
       
    }
}
