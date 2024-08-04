using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
    public class CategoryRepository : GenericRepository<Category> , ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public void Update(Category category)
        {
            var CategoryINDB = _context.Categories.FirstOrDefault(x => x.Id == category.Id);
            if (CategoryINDB != null) {
                CategoryINDB.Name = category.Name;
                CategoryINDB.Description = category.Description;    
                CategoryINDB.CreatedTime = DateTime.Now;        
            }
        }
    }
}
