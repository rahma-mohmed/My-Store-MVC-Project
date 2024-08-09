using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mystore.Utilities;
using mystore.Entities.Models;

namespace mystore.DataAccess.DBIntilalizer
{
	public class DBIntilalizer : IDBIntilalizer
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly RoleManager<IdentityRole> _roleManager;
		private readonly ApplicationDbContext _context;

		public DBIntilalizer(UserManager<IdentityUser> userManager,RoleManager<IdentityRole> roleManager,ApplicationDbContext context)
		{
			_userManager = userManager;
			_roleManager = roleManager;
			_context = context;
		}

		public void Intailze()
		{
			try
			{
				if (_context.Database.GetPendingMigrations().Count() > 0) {
					_context.Database.Migrate();
				}
			}
			catch (Exception ex)
			{

			}

			if (!_roleManager.RoleExistsAsync(SD.AdminRole).GetAwaiter().GetResult())
			{
				_roleManager.CreateAsync(new IdentityRole(SD.AdminRole)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.EditorRole)).GetAwaiter().GetResult();
				_roleManager.CreateAsync(new IdentityRole(SD.CustomerRole)).GetAwaiter().GetResult();


				_userManager.CreateAsync(new ApplicationUser
				{
					UserName = "Admin@mystore.com",
					Email = "Admin@mystore.com",
					Name = "Admin",
					PhoneNumber = "1234567890",
					Address = "Damietta",
					City = "Damietta",
				},
				"142Rahma@").GetAwaiter().GetResult();

				ApplicationUser user = _context.ApplicationUsers.FirstOrDefault(u => u.Email == "Admin@mystore.com");
				_userManager.AddToRoleAsync(user, SD.AdminRole).GetAwaiter().GetResult(); ;
			}

			return;
		}
	}
}
