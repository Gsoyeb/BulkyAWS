using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.Models.ViewModels
{
    public class ProductVM                                                  // Product + CategoryList
    {
        public Product Product { get; set; }

        [ValidateNever]         // No need for their validation
        public IEnumerable<SelectListItem> CategoryList { get; set; }        
    }
}
