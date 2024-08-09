using Microsoft.AspNetCore.Mvc;
using mystore.Entities.Models;
using mystore.DataAccess;
using mystore.Entities.Repositories;
using Microsoft.AspNetCore.Authorization;

namespace mystore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class CategoryController : Controller
    {
        //private readonly ApplicationDbContext _context;
        
        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            //_context = context;
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index()
        {
            //var categories = _context.Categories.ToList();
            var categories = _unitOfWork.CategoryRepository.GetAll();
            return View(categories);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        //Protect from cross side forgery attack
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category ctg)
        {
            if (ModelState.IsValid)
            {
                //_context.Categories.Add(ctg);
                //_context.SaveChanges();
                _unitOfWork.CategoryRepository.Add(ctg);
                _unitOfWork.Complete();
                TempData["Create"] = "Item Has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(ctg);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            var category = _unitOfWork.CategoryRepository.GetFirstorDefault(x => x.Id == id);
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category ctg)
        {

            if (ModelState.IsValid)
            {
                //_context.Categories.Update(ctg);
                //_context.SaveChanges();
                _unitOfWork.CategoryRepository.Update(ctg);
                _unitOfWork.Complete();
                TempData["Update"] = "Item Has Updated Successfully";
                return RedirectToAction("Index");
            }
            else
            {
                return View(ctg);
            }

        }

        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            //var categ = _context.Categories.Find(id);
            var categ = _unitOfWork.CategoryRepository.GetFirstorDefault(x => x.Id == id);
            return View(categ);
        }

        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var categ = _unitOfWork.CategoryRepository.GetFirstorDefault(x => x.Id == id);
            if (categ == null)
            {
                NotFound();
            }
            //_context.Categories.Remove(categ);
            //_context.SaveChanges();
            _unitOfWork.CategoryRepository.Remove(categ);
            _unitOfWork.Complete();
            TempData["Delete"] = "Data Has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}


