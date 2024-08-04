using Microsoft.AspNetCore.Mvc;
using mystore.Entities.Models;
using mystore.DataAccess;
using mystore.Entities.Repositories;
using Microsoft.AspNetCore.Mvc.Rendering;
using mystore.Entities.ViewModels;

namespace mystore.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        //private readonly ApplicationDbContext _context;
        
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork , IWebHostEnvironment webHostEnvironment)
        {
            //_context = context;
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //var categories = _context.Categories.ToList();

            /*var categories = _unitOfWork.ProductRepository.GetAll();
            return View(categories);*/

            return View();
        }

        public IActionResult GetData()
        {
            var products = _unitOfWork.ProductRepository.GetAll(Includeword:"Category");
            return Json(new {data =  products});
            
        }

        public IActionResult Create()
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        //Protect from cross side forgery attack
        [ValidateAntiForgeryToken]
        public IActionResult Create(ProductVM productVM , IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;

                if(upload != null)
                {
                    string filename = Guid.NewGuid().ToString();

                    var file = Path.Combine(RootPath, @"Images\Products");
                    var ext = Path.GetExtension(upload.FileName);

                    using(var filestream = new FileStream(Path.Combine(file , filename + ext) , FileMode.Create))
                    {
                        upload.CopyTo(filestream);
                    }

                    productVM.Product.Img = @"Images\Products\"+ filename + ext;
                }

                //_context.Categories.Add(ctg);
                //_context.SaveChanges();
                _unitOfWork.ProductRepository.Add(productVM.Product);
                _unitOfWork.Complete();
                TempData["Create"] = "Item Has Created Successfully";
                return RedirectToAction("Index");
            }
            return View(productVM.Product);
        }

        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            //var category = _context.Categories.FirstOrDefault(c => c.Id == id);
            
            ProductVM productVM = new ProductVM()
            {
                Product = _unitOfWork.ProductRepository.GetFirstorDefault(x => x.Id == id),
                CategoryList = _unitOfWork.CategoryRepository.GetAll().Select(x => new SelectListItem
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            return View(productVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ProductVM productVM , IFormFile upload)
        {
            if (ModelState.IsValid)
            {
                string RootPath = _webHostEnvironment.WebRootPath;

                if (upload != null)
                {
                    var ex = Path.GetExtension(upload.FileName);
                    var fullroot = Path.Combine(RootPath, @"Images\Products");
                    string filename = Guid.NewGuid().ToString();

                    if (productVM.Product.Img != null)
                    {
                        var old = Path.Combine(RootPath, productVM.Product.Img.TrimStart('\\'));

                        if (System.IO.File.Exists(old))
                        {
                            System.IO.File.Delete(old);
                        }
                    }
                    using (var filestream = new FileStream(Path.Combine(fullroot, filename + ex), FileMode.Create))
                    {
                        upload.CopyTo(filestream);
                    }
                    productVM.Product.Img = @"Images\Products\" + filename + ex;
                }

               
                _unitOfWork.ProductRepository.Update(productVM.Product);
                _unitOfWork.Complete();
                TempData["Update"] = "Item Has Updated Successfully";
                return RedirectToAction("Index");
            }
           return View(productVM);

        }

        
/*        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                NotFound();
            }
            //var categ = _context.Categories.Find(id);
            var productInDB = _unitOfWork.ProductRepository.GetFirstorDefault(x => x.Id == id);
            return View(productInDB);
        }*/

        [HttpDelete]
        public IActionResult DeleteProduct(int? id)
        {
            var productInDB = _unitOfWork.ProductRepository.GetFirstorDefault(x => x.Id == id);
            if (productInDB == null)
            {
                return Json(new { success = false, message = "Error while Deleting" });
            }

            _unitOfWork.ProductRepository.Remove(productInDB);
     
                var old = Path.Combine(_webHostEnvironment.WebRootPath, productInDB.Img.TrimStart('\\'));

                if (System.IO.File.Exists(old))
                {
                    System.IO.File.Delete(old);
                }
            
            _unitOfWork.Complete();
            return Json(new { success = true, message = "Image has been deleted" });

        }
    }
}


