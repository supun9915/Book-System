using BookApplication.Models;
using BookApplication.Services;
using Microsoft.AspNetCore.Mvc;

namespace BookApplication.Controllers
{
    public class BookController : Controller
    {
        private readonly ApplicationDbContext context;
		private readonly IWebHostEnvironment environment;

		public BookController(ApplicationDbContext context, IWebHostEnvironment environment)
        {
            this.context = context;
			this.environment = environment;
		}
        public IActionResult Index()
        {
            var products = context.Products.ToList();
            return View(products);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
		public IActionResult Create(ProductDto productDto)
		{
            if(productDto.ImageFile == null) 
            {
                ModelState.AddModelError("ImageFile", "The image file id required");
            }

            if(!ModelState.IsValid)
            {
                return View(productDto);
            }

            // save the image file
            string newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
            newFileName += Path.GetExtension(productDto.ImageFile!.FileName);

            string imageFullPath = environment.WebRootPath + "/bookImages/" + newFileName;
            using (var stream = System.IO.File.OpenWrite(imageFullPath))
            {
                productDto.ImageFile.CopyTo(stream);
            }

            //save the object to the database
            Product product = new Product()
            {
                    Name = productDto.Name,
                    Brand = productDto.Brand,
                    Category = productDto.Category,
                    Price = productDto.Price,
                    Descrption = productDto.Descrption,
                    ImageFileName = newFileName,
                    CreatedAt = DateTime.Now,

            };

            context.Products.Add(product);
            context.SaveChanges();

            return RedirectToAction("Index", "Book");

		}

		public IActionResult Edit(int id)
		{
            var product = context.Products.Find(id);

            if(product == null)
            {
                return RedirectToAction("Index", "Products");
            }

            var productDto = new ProductDto()
            {
                Name = product.Name,
                Brand = product.Brand,
                Category = product.Category,
                Price = product.Price,
                Descrption = product.Descrption,
            };

            ViewData["ProductId"] = product.Id;
            ViewData["ImageFileName"] = product.ImageFileName;
            ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");


            return View(productDto);
		}

        [HttpPost]
		public IActionResult Edit(int id, ProductDto productDto)
        {
            var product = context.Products.Find(id);
            if(product == null)
            {
                return RedirectToAction("Index", "Book");
            }

			if (!ModelState.IsValid)
			{

				ViewData["ProductId"] = product.Id;
				ViewData["ImageFileName"] = product.ImageFileName;
				ViewData["CreatedAt"] = product.CreatedAt.ToString("MM/dd/yyyy");

				return View(productDto);
			}

            //update the image file if we have a new image file
            string newFileName = product.ImageFileName;
            if(productDto.ImageFile != null)
            {
                newFileName = DateTime.Now.ToString("yyyyMMddHHmmssfff");
                newFileName += Path.GetExtension(productDto.ImageFile.FileName);

                string imageFullPath = environment.WebRootPath + "/bookImages/" + newFileName;
                using (var stream = System.IO.File.Create(imageFullPath))
                {
                    productDto.ImageFile.CopyTo(stream);
                }

                //delete the old image
                String oldImageFullPath = environment.WebRootPath + "/bookImages/" + product.ImageFileName;
                System.IO.File.Delete(oldImageFullPath); 

            }

			//update the product in the database
			product.Name = productDto.Name;
			product.Brand = productDto.Brand;
			product.Category = productDto.Category;
			product.Price = productDto.Price;
			product.Descrption = productDto.Descrption;
			product.ImageFileName = newFileName;

			context.SaveChanges();

            return RedirectToAction("Index", "Book");


		}

        public IActionResult Delete(int id)
        {
            var product = context.Products.Find(id);
            if (product == null)
            {
                return RedirectToAction("Index", "Book");

            }

			//delete image from folder
			String imageFullPath = environment.WebRootPath + "/bookImages/" + product.ImageFileName;
			System.IO.File.Delete(imageFullPath);

            context.Products.Remove(product);   
            context.SaveChanges();

			return RedirectToAction("Index", "Book");

		}
	}
}
