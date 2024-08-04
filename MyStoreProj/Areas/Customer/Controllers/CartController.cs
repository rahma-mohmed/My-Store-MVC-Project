using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mystore.DataAccess.Implementation;
using mystore.Entities.Models;
using mystore.Entities.Repositories;
using mystore.Entities.ViewModels;
using mystore.Utilities;
using Stripe.Checkout;
using System.Security.Claims;
using static System.Net.WebRequestMethods;

namespace MyStore.web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartVM ShoppingCartVM { get; set; }
        public int totalCarts {  get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value , Includeword : "Product")
            };

            foreach (var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.total += item.count * item.Product.Price;
            }
            return View(ShoppingCartVM);
        }

        public IActionResult Plus(int cartid)
        {
            var shoppingcart = _unitOfWork.ShoppingCartRepository.GetFirstorDefault(x => x.Id == cartid);
            _unitOfWork.ShoppingCartRepository.IncreaseCount(shoppingcart , 1);
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }

		public IActionResult Minus(int cartid)
		{
			var shoppingcart = _unitOfWork.ShoppingCartRepository.GetFirstorDefault(x => x.Id == cartid);

            if(shoppingcart.count <= 1)
            {
                _unitOfWork.ShoppingCartRepository.Remove(shoppingcart);
				_unitOfWork.Complete();
				return RedirectToAction("Index" , "Home");
			}
            else
            {
				_unitOfWork.ShoppingCartRepository.DecreaseCount(shoppingcart, 1);
			}
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}

		public IActionResult Remove(int cartid)
		{
			var shoppingcart = _unitOfWork.ShoppingCartRepository.GetFirstorDefault(x => x.Id == cartid);
			_unitOfWork.ShoppingCartRepository.Remove(shoppingcart );
			_unitOfWork.Complete();
			return RedirectToAction("Index");
		}

        [HttpGet]
		public IActionResult Summary()
		{
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            ShoppingCartVM = new ShoppingCartVM()
            {
                CartsList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value),
				orderHeader = new()
            };
            ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUserRepository.GetFirstorDefault(x => x.Id == claim.Value);

            ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.Name;
            ShoppingCartVM.orderHeader.Address = ShoppingCartVM.orderHeader.ApplicationUser.Address;
			ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
			ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;

            foreach(var item in ShoppingCartVM.CartsList)
            {
                ShoppingCartVM.orderHeader.TotalPrice += (item.count * item.Product.Price);
            }


			return View(ShoppingCartVM);
		}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Summary")]
		public IActionResult SummaryOnPost(ShoppingCartVM shoppingCart)
        {
			var claimIdentity = (ClaimsIdentity)User.Identity;
			var claim = claimIdentity.FindFirst(ClaimTypes.NameIdentifier);

            shoppingCart.CartsList = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == claim.Value);

            shoppingCart.orderHeader.OrderStatus = SD.Pending;
            shoppingCart.orderHeader.PaymentStatus = SD.Pending;
            shoppingCart.orderHeader.OrderDate = DateTime.Now;
            shoppingCart.orderHeader.ApplicationUserId = claim.Value;

            foreach (var item in shoppingCart.CartsList) { 
                shoppingCart.orderHeader.TotalPrice += (item.count * item.Product.Price);
			}

            _unitOfWork.OrderHeaderRepository.Add(shoppingCart.orderHeader);
            _unitOfWork.Complete();

            foreach(var item in shoppingCart.CartsList)
            {
                OrderDetail detail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderId = shoppingCart.orderHeader.Id,
                    Price = item.Product.Price,
                    Count = item.count
                };
				_unitOfWork.OrderDetailsRepository.Add(detail);
				_unitOfWork.Complete();
			}

			var Domain = "https://localhost:44350/";


			var options = new SessionCreateOptions
			{
		        LineItems = new List<SessionLineItemOptions>(),
				Mode = "payment",
				SuccessUrl = Domain + $"customer/cart/OrderConfirmation?id={shoppingCart.orderHeader.Id}",
				CancelUrl = Domain + $"customer/cart",
			};

            foreach(var item in shoppingCart.CartsList)
            {
                var sessionlineoptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)(item.Product.Price) * 100,
                        Currency = "usd",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name,
                        },
                    },
                    Quantity = item.count,
                };
				options.LineItems.Add(sessionlineoptions);
			}

			var service = new SessionService();
			Session session = service.Create(options);

            shoppingCart.orderHeader.SessionId = session.Id;
            shoppingCart.orderHeader.PaymentIntentId = session.PaymentIntentId;
            _unitOfWork.Complete();

			Response.Headers.Add("Location", session.Url);
			return new StatusCodeResult(303);


			/*_unitOfWork.ShoppingCartRepository.RemoveRange(shoppingCart.CartsList);
            _unitOfWork.Complete();
            return RedirectToAction("Index" , "Home");*/
		}

        public IActionResult OrderConfirmation(int id)
        {
            OrderHeader orderheader = _unitOfWork.OrderHeaderRepository.GetFirstorDefault(u => u.Id == id);

            var service = new SessionService();

            Session session = service.Get(orderheader.SessionId);

            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeaderRepository.UpdateOrderStatus(id , SD.Approve , SD.Approve);
                _unitOfWork.Complete();
            }

            List<ShoppingCart> shoppingcarts = _unitOfWork.ShoppingCartRepository.GetAll(u => u.ApplicationUserId == orderheader.ApplicationUserId).ToList();

            _unitOfWork.ShoppingCartRepository.RemoveRange(shoppingcarts);
            _unitOfWork.Complete();
            return View("Index" , "Home");
        }

	}
}

