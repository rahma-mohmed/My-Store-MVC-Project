using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
    public class ShoppingCartRepository : GenericRepository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext _context;

        public ShoppingCartRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public int DecreaseCount(ShoppingCart cart, int count)
        {
            cart.count -= count;
            return cart.count;
        }

        public int IncreaseCount(ShoppingCart cart, int count)
        {
            cart.count += count;
            return cart.count;
        }
    }
}
