using Microsoft.AspNetCore.Mvc;

namespace BilkoNavigator_.Controllers
{
    public class RecognitionController : Controller
    {
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
