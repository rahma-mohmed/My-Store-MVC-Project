using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using mystore.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace mystore.Entities.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<ShoppingCart> CartsList { get; set; }
        public decimal total {  get; set; }

        public int OrderId {  get; set; }
		[ValidateNever]
		public OrderHeader orderHeader { get; set; }
    }
}
