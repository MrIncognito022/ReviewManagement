using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using OfficeOpenXml;
using WebApp.Data;
using WebApp.Models;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment hostingEnvironment, ApplicationDbContext context)
        {
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }
        [HttpPost]
        public IActionResult CreateCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.TodayDate = DateTime.Now; // Set the current date

                _context.Customers.Add(customer);
                _context.SaveChanges();
                TempData["success"] = "Customer Created";
                return RedirectToAction("Index"); // Redirect to a success page
            }

            return View(customer); // Show the form again with validation errors
        }
        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Upload(FileUploadViewModel model)
        {
            
            if (model.File == null || model.File.Length == 0)
            {
                ModelState.AddModelError("", "Please select an Excel file to upload.");
                return View(model);
            }

            if (!Path.GetExtension(model.File.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("File", "Please select an Excel file (.xlsx).");
                return View(model);
            }

            var filePath = Path.Combine(_hostingEnvironment.WebRootPath, "uploads", model.File.FileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            using (var package = new ExcelPackage(new FileInfo(filePath)))
            {
                var worksheet = package.Workbook.Worksheets[0];
                var rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    var firstName = worksheet.Cells[row, 1].Value?.ToString();
                    var lastName = worksheet.Cells[row, 2].Value?.ToString();
                    var checkoutDate = worksheet.Cells[row, 3].Value?.ToString();
                    var email = worksheet.Cells[row, 4].Value?.ToString();
                    var todayDate = DateTime.Now;

                    var data = new Customer
                    {
                        FirstName = firstName,
                        LastName = lastName,
                        CheckoutDate = DateTime.Parse(checkoutDate),
                        Email = email,
                        TodayDate = todayDate
                    };

                    _context.Customers.Add(data);
                }

                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index");
        }

        

    }
}