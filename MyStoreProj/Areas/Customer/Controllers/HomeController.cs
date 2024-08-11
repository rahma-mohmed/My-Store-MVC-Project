using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mystore.Entities.Models;
using mystore.Entities.Repositories;
using System.Security.Claims;
using X.PagedList.Extensions;
using mystore.Utilities;
using Microsoft.AspNetCore.Http;

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
        public IActionResult Index(int ? page)
        {
            int pageNumber = page ?? 1;
            int pageSize = 10;

            var products = _unitOfWork.ProductRepository.GetAll().ToPagedList(pageNumber , pageSize);
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
                HttpContext.Session.SetInt32(SD.SessionKey
                    ,_unitOfWork.ShoppingCartRepository.GetAll(x => x.ApplicationUserId == claim.Value).ToList().Count());
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
