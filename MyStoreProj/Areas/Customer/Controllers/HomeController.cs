using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System.Security.Claims;

namespace mystore.Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll();
            return View(products);
        }

        [HttpGet]
        public IActionResult Details(int id)
        {
            if(id == 0)
            {
                return NotFound();
            }
            else
            {
                ShoppingCart obj = new ShoppingCart()
                {
                    ProductId = id,
                    Product = _unitOfWork.ProductRepository.GetFirstorDefault(x => x.Id == id, Includeword: "Category"),
                    count = 1
                };
                return View(obj);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(ShoppingCart shc)
        {
            var claims = (ClaimsIdentity)User.Identity;
            var claim = claims.FindFirst(ClaimTypes.NameIdentifier);
            shc.ApplicationUserId = claim.Value;
            shc.Id = 0;

            ShoppingCart obj = _unitOfWork.ShoppingCartRepository.GetFirstorDefault(
                u => u.ApplicationUserId == claim.Value && u.ProductId == shc.ProductId);

            if(obj == null)
            {
                _unitOfWork.ShoppingCartRepository.Add(shc);
                _unitOfWork.Complete();
            }
            else
            {
                _unitOfWork.ShoppingCartRepository.IncreaseCount(obj, shc.count);
            }
            _unitOfWork.Complete();
            return RedirectToAction("Index");
        }
    }
}
