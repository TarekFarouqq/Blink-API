﻿using Blink_API.Models;
using Blink_API.Repositories;
using Blink_API.Repositories.BrandRepository;
using Blink_API.Repositories.CartRepos;
using Blink_API.Repositories.DiscountRepos;

namespace Blink_API
{
    public class UnitOfWork
    {
        private readonly BlinkDbContext db;
        private ProductRepo productRepo;
        private CategoryRepo categoryRepo;
        private DiscountRepo discountRepo;
        private CartRepo cartRepo;
        private CartDetailsRepo cartDetailsRepo;
        private BrandRepos brandRepo;

        public UnitOfWork(BlinkDbContext _db)
        {
            db = _db;
        }

        public ProductRepo ProductRepo
        {
            get
            {
                if (productRepo == null)
                {
                    productRepo = new ProductRepo(db);
                }
                return productRepo;
            }
        }

        public CategoryRepo CategoryRepo
        {
            get
            {
                if (categoryRepo == null)
                {
                    categoryRepo = new CategoryRepo(db);
                }
                return categoryRepo;
            }
        }

        public DiscountRepo DiscountRepo
        {
            get
            {
                if (discountRepo == null)
                {
                    discountRepo = new DiscountRepo(db);
                }
                return discountRepo;
            }
        }

        public CartRepo CartRepo
        {
            get
            {
                if (cartRepo == null)
                {
                    cartRepo = new CartRepo(db);
                }
                return cartRepo;
            }
        }

        public CartDetailsRepo CartDetailsRepo
        {
            get
            {
                if (cartDetailsRepo == null)
                {
                    cartDetailsRepo = new CartDetailsRepo(db);
                }
                return cartDetailsRepo;
            }
        }

        public BrandRepos BrandRepos
        {
            get
            {
                if (brandRepo == null)
                {
                    brandRepo = new BrandRepos(db);
                }
                return brandRepo;
            }
        }

        
    }
}
