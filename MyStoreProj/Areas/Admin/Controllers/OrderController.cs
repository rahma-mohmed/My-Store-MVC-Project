using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mystore.DataAccess.Implementation;
using mystore.Entities.Models;
using mystore.Entities.Repositories;
using mystore.Entities.ViewModels;
using mystore.Utilities;
using Stripe;

namespace MyStore.web.Areas.Admin.Controllers
{
    [Area("Admin")]
	[Authorize(Roles = SD.AdminRole)]

    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitofwork;
		[BindProperty]
		public OrderVM OrderVM { get; set; }

		public ShoppingCartVM ShoppingCartVM { get; set; }

		public OrderController(IUnitOfWork unitofwork)
        {
            _unitofwork = unitofwork;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetData()
        {
            IEnumerable<OrderHeader> orderHeaders;
            orderHeaders = _unitofwork.OrderHeaderRepository.GetAll(Includeword: "ApplicationUser");
            return Json(new { data = orderHeaders });
        }

        public IActionResult Details(int orderid)
        {
            OrderVM orderVM = new OrderVM()
            {
                OrderHeader = _unitofwork.OrderHeaderRepository.GetFirstorDefault(u => u.Id == orderid , Includeword: "ApplicationUser"),
                OrderDetails = _unitofwork.OrderDetailsRepository.GetAll(x => x.OrderHeaderId == orderid , Includeword: "Product")
            };
            return View(orderVM);
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult UpdateOrderDetails()
		{
			var orderfromdb = _unitofwork.OrderHeaderRepository.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
			orderfromdb.Name = OrderVM.OrderHeader.ApplicationUser.Name;
			orderfromdb.PhoneNumber = OrderVM.OrderHeader.ApplicationUser.PhoneNumber;
			orderfromdb.Address = OrderVM.OrderHeader.ApplicationUser.Address;
			orderfromdb.City = OrderVM.OrderHeader.ApplicationUser.City;

			if (OrderVM.OrderHeader.Carrier != null)
			{
				orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
			}

			if (OrderVM.OrderHeader.TrackingNumber != null)
			{
				orderfromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			}

			_unitofwork.OrderHeaderRepository.Update(orderfromdb);
			_unitofwork.Complete();

			TempData["Update"] = "Item Has Updated Successfully";
			return RedirectToAction("Details", "Order", new { orderid = orderfromdb.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartProcessing()
		{
			_unitofwork.OrderHeaderRepository.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Processing, null);
			_unitofwork.Complete();
			TempData["Update"] = "Order Status Has Updated Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult StartShiping()
		{
			_unitofwork.OrderHeaderRepository.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Shipped, null);

			var orderfromdb = _unitofwork.OrderHeaderRepository.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
			orderfromdb.TrackingNumber = OrderVM.OrderHeader.TrackingNumber;
			orderfromdb.Carrier = OrderVM.OrderHeader.Carrier;
			orderfromdb.OrderStatus = SD.Shipped;
			orderfromdb.ShippingDate = DateTime.Now;

			_unitofwork.OrderHeaderRepository.Update(orderfromdb);
			_unitofwork.Complete();

			_unitofwork.OrderHeaderRepository.UpdateOrderStatus(OrderVM.OrderHeader.Id, SD.Processing, null);
			_unitofwork.Complete();

			TempData["Update"] = "Order has Shipped Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public IActionResult CancelOrder()
		{
			var orderfromdb = _unitofwork.OrderHeaderRepository.GetFirstorDefault(u => u.Id == OrderVM.OrderHeader.Id);
			if(orderfromdb.PaymentStatus == SD.Approve)
			{
				var op = new RefundCreateOptions
				{
					Reason = RefundReasons.RequestedByCustomer,
					PaymentIntent = orderfromdb.PaymentIntentId
				};

				var service = new RefundService();
				Refund refund = service.Create(op);

				_unitofwork.OrderHeaderRepository.UpdateOrderStatus(orderfromdb.Id, SD.Cancelled, SD.Refund);
			}
			else
			{
				_unitofwork.OrderHeaderRepository.UpdateOrderStatus(orderfromdb.Id, SD.Cancelled, SD.Cancelled);
			}
			_unitofwork.Complete();
			TempData["Update"] = "Order Has Cancelled Successfully";
			return RedirectToAction("Details", "Order", new { orderid = OrderVM.OrderHeader.Id });
		}
	}
}
