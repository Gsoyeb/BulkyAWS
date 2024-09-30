using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBookWeb.ViewComponents {
    public class ShoppingCartViewComponent : ViewComponent {

        private readonly IUnitOfWork _unitOfWork;
        public ShoppingCartViewComponent(IUnitOfWork unitOfWork) {
            _unitOfWork = unitOfWork;
        }

        public async Task<IViewComponentResult> InvokeAsync() 
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claim = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            if (claim != null) {

                if (HttpContext.Session.GetInt32(SD.SessionCart) == null) {
                    HttpContext.Session.SetInt32(SD.SessionCart,
                    _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == claim.Value).Count());
                }
                
                // Use null-coalescing operator to handle nullable int
                int sessionCartValue = HttpContext.Session.GetInt32(SD.SessionCart) ?? 0;

                return View(sessionCartValue);            }
            else {
                HttpContext.Session.Clear();
                return View(0);
            }
        }

    }
}
