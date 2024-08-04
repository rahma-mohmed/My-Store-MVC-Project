using mystore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ICategoryRepository CategoryRepository { get; private set; }

        public IProductRepository ProductRepository { get; private set; }

        public IShoppingCartRepository ShoppingCartRepository { get; private set; }

		public IorderDetailsRepository OrderDetailsRepository { get; private set; }

		public IOrderHeaderRepository OrderHeaderRepository { get; private set; }

        public IApplicationUserRepository ApplicationUserRepository { get; private set; }


		public UnitOfWork(ApplicationDbContext context) 
        {
            _context = context;
            CategoryRepository = new CategoryRepository(context);
            ProductRepository = new ProductRepository(context);
            ShoppingCartRepository = new ShoppingCartRepository(context);
            OrderDetailsRepository = new OrderDetailsRepository(context);
            OrderHeaderRepository = new OrderHeaderRepository(context);
            ApplicationUserRepository = new ApplicationUserRepository(context);
        }

        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
             _context.Dispose();
        }
    }
}
