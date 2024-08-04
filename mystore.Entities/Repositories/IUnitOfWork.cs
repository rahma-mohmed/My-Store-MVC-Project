using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.Entities.Repositories
{
    public interface IUnitOfWork:IDisposable
    {
        ICategoryRepository CategoryRepository { get; }
        IProductRepository ProductRepository { get; }
        IShoppingCartRepository ShoppingCartRepository { get; }
        IorderDetailsRepository OrderDetailsRepository { get; }
        IOrderHeaderRepository OrderHeaderRepository { get; }
        IApplicationUserRepository ApplicationUserRepository { get; }
        int Complete();
    }
}
