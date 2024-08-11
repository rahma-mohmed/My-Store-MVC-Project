using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mystore.Entities.Repositories;
using mystore.Utilities;

namespace MyStore.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.AdminRole)]
    public class DashBoardController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public DashBoardController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            ViewBag.Orders = _unitOfWork.OrderHeaderRepository.GetAll().Count();
            ViewBag.ApprovedOrders = _unitOfWork.OrderHeaderRepository.GetAll(x => x.OrderStatus == SD.Approve).Count();
            ViewBag.Users = _unitOfWork.ApplicationUserRepository.GetAll().Count();
            ViewBag.Products = _unitOfWork.ProductRepository.GetAll().Count();

            return View();
        }
    }
}
