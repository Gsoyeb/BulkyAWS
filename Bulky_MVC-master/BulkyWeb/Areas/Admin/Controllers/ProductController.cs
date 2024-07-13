using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;  // Import logging
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<ProductController> _logger;  // Logger field

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, ILogger<ProductController> logger)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;  // Assign logger
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            _logger.LogInformation("Getting all products.");
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Ok(objProductList);
        }

        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            _logger.LogInformation($"Getting product with id {id}.");
            Product product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "Category,ProductImages");
            if (product == null)
            {
                _logger.LogWarning($"Product with id {id} not found.");
                return NotFound();
            }
            return Ok(product);
        }

        [HttpPost]
        public IActionResult Upsert([FromForm] ProductVM productVM, [FromForm] List<IFormFile> files)
        {
            _logger.LogInformation($"Upserting product with id {productVM?.Product?.Id}.");

            if (productVM == null || !ModelState.IsValid)
            {
                _logger.LogWarning("Invalid product model.");
                return BadRequest(ModelState);
            }

            if (productVM.Product.Id == 0)
            {
                _unitOfWork.Product.Add(productVM.Product);
                _logger.LogInformation("Product added.");
            }
            else
            {
                _unitOfWork.Product.Update(productVM.Product);
                _logger.LogInformation("Product updated.");
            }

            _unitOfWork.Save();

            string wwwRootPath = _webHostEnvironment.WebRootPath;
            if (files != null)
            {
                foreach (IFormFile file in files)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productPath = Path.Combine("images", "products", "product-" + productVM.Product.Id);
                    string finalPath = Path.Combine(wwwRootPath, productPath);

                    if (!Directory.Exists(finalPath))
                    {
                        Directory.CreateDirectory(finalPath);
                    }

                    using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    ProductImage productImage = new()
                    {
                        ImageUrl = Path.Combine(productPath, fileName),
                        ProductId = productVM.Product.Id,
                    };

                    if (productVM.Product.ProductImages == null)
                    {
                        productVM.Product.ProductImages = new List<ProductImage>();
                    }

                    productVM.Product.ProductImages.Add(productImage);
                }

                _unitOfWork.Product.Update(productVM.Product);
                _unitOfWork.Save();
            }

            _logger.LogInformation("Product created/updated successfully.");
            return Ok(new { success = true, message = "Product created/updated successfully" });
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            _logger.LogInformation($"Deleting product with id {id}.");
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);
            if (productToBeDeleted == null)
            {
                _logger.LogWarning($"Product with id {id} not found.");
                return NotFound(new { success = false, message = "Error while deleting" });
            }

            string productPath = Path.Combine("images", "products", "product-" + id);
            string finalPath = Path.Combine(_webHostEnvironment.WebRootPath, productPath);

            if (Directory.Exists(finalPath))
            {
                string[] filePaths = Directory.GetFiles(finalPath);
                foreach (string filePath in filePaths)
                {
                    System.IO.File.Delete(filePath);
                }

                Directory.Delete(finalPath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            _logger.LogInformation("Product deleted successfully.");
            return Ok(new { success = true, message = "Delete Successful" });
        }

        [HttpDelete("DeleteImage/{imageId}")]
        public IActionResult DeleteImage(int imageId)
        {
            _logger.LogInformation($"Deleting image with id {imageId}.");
            var imageToBeDeleted = _unitOfWork.ProductImage.Get(u => u.Id == imageId);
            if (imageToBeDeleted == null)
            {
                _logger.LogWarning($"Image with id {imageId} not found.");
                return NotFound(new { success = false, message = "Image not found" });
            }

            int productId = imageToBeDeleted.ProductId;
            if (!string.IsNullOrEmpty(imageToBeDeleted.ImageUrl))
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageToBeDeleted.ImageUrl.TrimStart('\\'));

                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
            }

            _unitOfWork.ProductImage.Remove(imageToBeDeleted);
            _unitOfWork.Save();

            _logger.LogInformation("Image deleted successfully.");
            return Ok(new { success = true, message = "Deleted successfully", productId = productId });
        }

        [HttpGet("GetView")]
        public IActionResult Index()
        {
            _logger.LogInformation("Getting all products for view.");
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Ok(objProductList);
        }

        [HttpGet("UpsertView/{id?}")]
        public IActionResult Upsert(int? id)
        {
            _logger.LogInformation($"Upsert view for product with id {id}.");
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == null || id == 0)
            {
                return Ok(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.Get(u => u.Id == id, includeProperties: "ProductImages");
                if (productVM.Product == null)
                {
                    _logger.LogWarning($"Product with id {id} not found.");
                    return NotFound();
                }
                return Ok(productVM);
            }
        }

        [HttpPost("UpsertView/{id?}")]
        public IActionResult UpsertView(int? id, [FromBody] ProductVM productVM, [FromForm] List<IFormFile> files)
        {
            _logger.LogInformation($"Upsert view for product with id {id}.");
            if (ModelState.IsValid)
            {
                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                }

                _unitOfWork.Save();

                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (files != null)
                {
                    foreach (IFormFile file in files)
                    {
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                        string productPath = Path.Combine("images", "products", "product-" + productVM.Product.Id);
                        string finalPath = Path.Combine(wwwRootPath, productPath);

                        if (!Directory.Exists(finalPath))
                        {
                            Directory.CreateDirectory(finalPath);
                        }

                        using (var fileStream = new FileStream(Path.Combine(finalPath, fileName), FileMode.Create))
                        {
                            file.CopyTo(fileStream);
                        }

                        ProductImage productImage = new()
                        {
                            ImageUrl = Path.Combine(productPath, fileName),
                            ProductId = productVM.Product.Id,
                        };

                        if (productVM.Product.ProductImages == null)
                        {
                            productVM.Product.ProductImages = new List<ProductImage>();
                        }

                        productVM.Product.ProductImages.Add(productImage);
                    }

                    _unitOfWork.Product.Update(productVM.Product);
                    _unitOfWork.Save();
                }

                return Ok(new { success = true, message = "Product created/updated successfully" });
            }
            else
            {
                productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                });
                return BadRequest(ModelState);
            }
        }
    }
}
