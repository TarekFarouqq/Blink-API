﻿using Blink_API.Models;
using Microsoft.EntityFrameworkCore;

namespace Blink_API.Repositories
{
    public class CategoryRepo:GenericRepo<Category,int>
    {
        private readonly BlinkDbContext _db;
        public CategoryRepo(BlinkDbContext db):base(db)
        {
            _db = db;
        }
        public async Task<List<Category>> GetParentCategories()
        {
            return await _db.Categories
                .Where(pc=>pc.ParentCategoryId == null)
                .Where(pc=>!pc.IsDeleted)
                .ToListAsync();
        }
        public async Task<List<Category>> GetChildCategories()
        {
            return await _db.Categories
                .Where(pc => pc.ParentCategoryId != null)
                .Where(pc => !pc.IsDeleted)
                .ToListAsync();
        }
    }
}
