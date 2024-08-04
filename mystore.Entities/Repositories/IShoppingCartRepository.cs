using mystore.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.Entities.Repositories
{
    public interface IShoppingCartRepository : IGenericRepository<ShoppingCart>
    {
        int IncreaseCount(ShoppingCart cart, int count);
        int DecreaseCount(ShoppingCart cart, int count);
    }
}
