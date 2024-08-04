using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
    public class ProductRepository : GenericRepository<Product> , IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Product product)
        {
            var ProductInDB = _context.Products.FirstOrDefault(x => x.Id == product.Id);
            if (ProductInDB != null) {
                ProductInDB.Name = product.Name;
                ProductInDB.Description = product.Description;
                ProductInDB.Price = product.Price;
                ProductInDB.Category = product.Category;
                ProductInDB.Img = product.Img;
            }
        }
    }
}
