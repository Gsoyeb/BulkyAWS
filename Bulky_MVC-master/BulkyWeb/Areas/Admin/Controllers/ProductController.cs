using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.DataAcess.Data;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using System.Data;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index() 
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();       // includeProperties:"Category" to populate FK as well

            return View(objProductList);
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()     // ViewModel is a model that is specific for the View.
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()     // Pass this as a viewbag: when to use? when it's not binded by a data. After re-direct it gets lost. Lifetime: current http request. It is a wrapper around viewdata.
                }),
                Product = new Product()
            };


            if (id == null || id == 0)
            {
                //create
                return View(productVM);


                // Before version without VM: Finish the notes first. At the end see why VM is used: Not everything is available by Product Object. And we cannot keep adding ViewData or ViewBag. Too many in future. So, VM is the solution. Look up.
                // ViewBag: Temporary Data is not model
                IEnumerable<SelectListItem> categoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem     // Careful with library of SelectListItem library. There could be some depreciation
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                ViewBag.CategoryList = categoryList;            // ViewBag: --> <select asp-for="CategoryId" asp-items="ViewBag.CategoryList" class="form-select border-0 shadow">
                ViewData["CategoryList"] = categoryList;        // ViewData: --> <select asp-for="CategoryId" asp-items="@(ViewData["CategoryList"] as IEnumerable<SelectListItem>)" class="form-select border-0 shadow">
                                                                // Key of ViewData and property of ViewBag cannot match ==> ViewBag.CategoryList != ViewData["CategoryList"] because they are exact same.
                                                                // TempData: Is available in the first and second request (so, it is also available in the first re-direct). Must check the TempData if it's not null because gotta cast it

                return View();





            }
            else
            {
                //update
                productVM.Product = _unitOfWork.Product.Get(u=>u.Id==id,includeProperties:"ProductImages");
                return View(productVM);
            }
            
        }
        [HttpPost]      // Post. As in when you receive
        public IActionResult Upsert(ProductVM productVM, List<IFormFile> files)     // Make sure we receive ProductVM and not Product. Category and CategoryList come as invalid. But we can say we do not need their validation.
        {
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0) {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();


                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)          // This is for update. If nothing to update, then we will not update the image
                {

                    foreach(IFormFile file in files) 
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);     // GUID provides random strings + the filename
                        string productPath = @"images\products\product-" + productVM.Product.Id;            // Folders where the file will be saved
                        string finalPath = Path.Combine(wwwRootPath, productPath);                          // Combine root directory and product path diirectory

                        if (!Directory.Exists(finalPath))                                                   // Check if there is an already directory. If it does not, then create a folder.
                            Directory.CreateDirectory(finalPath);

                        // var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create). This will already create a file for you. But will be of 0 Bytes.
                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create)) {
                            file.CopyTo(fileStream);                                                        // After this the file will be populated
                        }

                        ProductImage productImage = new() {                                                 // Now the that image is saved, we gotta remember the location of it in the db. So, create new ProductImage
                            ImageUrl = @"\" + productPath + @"\" + fileName,
                            ProductId=productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)        // if ProductImages of productVM.Product is not initialised, then create a new list to add them. Could have been done before the loop
                            productVM.Product.ProductImages = new List<ProductImage>();

                        productVM.Product.ProductImages.Add(productImage);      // Not sure if it will allow multiple 

                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();




                }

                
                TempData["success"] = "Product created/updated successfully";
                return RedirectToAction("Index");
            }
            else
            {   // This is clever: Imagine operation was not succesful (because validation failed). In that case, I want the same values that were filled to be re-passed. But gotta repass .CategoryList because remember it's valid for one request
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return View(productVM);     // What is a strongly typed view? Answer is VM. Why? Because for each View, we have only one dedicated Model. As a result, that view is strongly typed to the view.
            }
        }


        public IActionResult DeleteImage(int imageId) {     // Delete image from db and root
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);      // Retrieve the string location of the image to be deleted from db.
            int productId = imageToBeDeleted.ProductId;
            if (imageToBeDeleted != null) {
                if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl)) {
                    var oldImagePath =
                                   Path.Combine(_webHostEnvironment.WebRootPath,
                                   imageToBeDeleted.ImageUrl.TrimStart('\\'));

                    if (System.IO.File.Exists(oldImagePath)) {
                        System.IO.File.Delete(oldImagePath);        // if image exists
                    }
                }

                _unitOfWork.ProductImage.Remove(imageToBeDeleted);      // Delete the image from folder location string from db
                _unitOfWork.Save();

                TempData["success"] = "Deleted successfully";       // Send the message to the redirected url
            }

            return RedirectToAction(nameof(Upsert), new { id = productId });        // Redirect to Upsert function with the producID retrieved from image To be deleted
        }

        #region API CALLS

        // APIs are already integrated in ASP.NET

        [HttpGet]
        public IActionResult GetAll()           // URL: https://localhost:PORT_NUMBER/admin/product/getall
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });         // Check in the product.js
        }


        [HttpDelete]                                // If it does not work, remove HttpDelete
        public IActionResult Delete(int? id)        // Get the ID
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);      // Retrieve from db
            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });     // Send Error message
            }

            string productPath = @"images\products\product-" + id;
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath)) {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths) {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);        // Remove the image physically
            }


            _unitOfWork.Product.Remove(productToBeDeleted);         // Remove the product from db now
            _unitOfWork.Save();

            return Json(new { success = true, message = "Delete Successful" });
        }

        #endregion
    }
}
