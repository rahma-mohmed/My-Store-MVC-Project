using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
	public class OrderDetailsRepository : GenericRepository<OrderDetail>, IorderDetailsRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderDetailsRepository(ApplicationDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(OrderDetail orderDetail)
		{
			_context.OrderDetails.Update(orderDetail);
		}
	}
}