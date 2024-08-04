using mystore.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.Entities.Repositories
{
	public interface IorderDetailsRepository : IGenericRepository<OrderDetail>
	{
		void Update(OrderDetail orderDetail);
	}
}
