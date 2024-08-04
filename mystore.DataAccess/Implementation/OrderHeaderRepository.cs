using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.DataAccess.Implementation
{
	public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
	{
		private readonly ApplicationDbContext _context;

		public OrderHeaderRepository(ApplicationDbContext context) : base(context) 
		{
			_context = context;
		}

		void IOrderHeaderRepository.Update(OrderHeader orderHeader)
		{
			_context.OrderHeaders.Update(orderHeader);
		}

		void IOrderHeaderRepository.UpdateOrderStatus(int id, string OrderStatus, string PaymentStatus)
		{
			var orderfromDb = _context.OrderHeaders.FirstOrDefault(x => x.Id == id);

			if (orderfromDb != null) { 
				orderfromDb.OrderStatus = OrderStatus;

				if(PaymentStatus != null)
				{
					orderfromDb.PaymentStatus = PaymentStatus;
				}
			}
		}


	}
}
