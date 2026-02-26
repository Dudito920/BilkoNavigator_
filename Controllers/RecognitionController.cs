using BilkoNavigator_.Data;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BilkoNavigator_.Controllers
{
    public class RecognitionController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;


        public RecognitionController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View("Upload");
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Upload(IFormFile image)
        {
            if (image == null || image.Length == 0)
            {
                ViewBag.Error = "Моля, изберете снимка.";
                return View();
            }

            // временно "разпознаване"
            string recognizedHerb = "Коприва";

            ViewBag.HerbName = recognizedHerb;
            return View("Result");
        }
    }
}
