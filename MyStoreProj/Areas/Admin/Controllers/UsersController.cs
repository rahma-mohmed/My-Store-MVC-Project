using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using mystore.DataAccess;
using mystore.Utilities;
using System.Security.Claims;

namespace MyStore.web.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // show all user except the current user

            var claimsIdentity =  (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            string userid = claim.Value;
            return View(_context.ApplicationUsers.Where(x => x.Id != userid).ToList());
        }

        public IActionResult LockUnlock(string ?id) { 
            var user = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);

            if (user == null) {
                return NotFound();
            }

            if(user.LockoutEnd == null || user.LockoutEnd < DateTime.Now){
                user.LockoutEnd = DateTime.Now.AddYears(1);
            }
            else
            {
                user.LockoutEnd = DateTime.Now;
            }
            _context.SaveChanges();
            return RedirectToAction("Index" , "Users" , new {area = "Admin"});
        }

        public IActionResult Delete(string? id)
        {
            var User = _context.ApplicationUsers.FirstOrDefault(x => x.Id == id);
            if (User == null)
            {
                NotFound();
            }

            _context.ApplicationUsers.Remove(User);
            _context.SaveChanges();
            TempData["Delete"] = "User Has Deleted Successfully";
            return RedirectToAction("Index");
        }
    }
}
