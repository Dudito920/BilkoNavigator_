using BilkoNavigator_.Data;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BilkoNavigator_.Controllers
{
    public class HerbsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IWebHostEnvironment _environment;



        public HerbsController(AppDbContext context, UserManager<User> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            _environment = environment;
        }

        // GET: Herbs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Herbs.ToListAsync());
        }

        // GET: Herbs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herb = await _context.Herbs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (herb == null)
            {
                return NotFound();
            }

            return View(herb);
        }

        // GET: Herbs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Herbs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ////[HttpPost]
        ////[ValidateAntiForgeryToken]
        ////public async Task<IActionResult> Create([Bind("Id,PopularName,LatinName,DialectNames,Aroma,Taste,Habitat,Season,IsPoisonous,IsProtected,UsedPart,Benefits,Description")] Herb herb, IFormFile Image)
        ////{

        ////    var imagesDir = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images");
        ////    if (!Directory.Exists(imagesDir))
        ////        Directory.CreateDirectory(imagesDir);

        ////    if (ModelState.IsValid)
        ////    {
        ////        if (Image != null && Image.Length > 0)
        ////        {

        ////            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Image.FileName)}";
        ////            var imagePath = Path.Combine("wwwroot/images", fileName);

        ////            //var imagePath = Path.Combine("wwwroot/images", Image.FileName);

        ////            using (var stream = new FileStream(imagePath, FileMode.Create))
        ////            {
        ////                await Image.CopyToAsync(stream);
        ////            }

        ////            herb.Image = new HerbImage
        ////            {
        ////                ImagePath = $"/images/{fileName}",
        ////                UploadedOn = DateTime.Now
        ////            };
        ////        }

        ////        _context.Herbs.Add(herb);
        ////        await _context.SaveChangesAsync();
        ////        return RedirectToAction(nameof(Index));
        ////    }

        ////    // Log validation errors for debugging
        ////    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
        ////    {
        ////        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
        ////    }

        ////    return View(herb);
        ////}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Herb herb, IFormFile Image)
        {

            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }

            if (!ModelState.IsValid)
            {
                return View(herb); 
            }

            if (Image != null && Image.Length > 0)
            {
                var imagesDir = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(imagesDir))
                {
                    Directory.CreateDirectory(imagesDir);
                }

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(Image.FileName)}";
                var fullPath = Path.Combine(imagesDir, fileName);

                using (var stream = new FileStream(fullPath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                herb.Image = new HerbImage
                {
                    ImagePath = "/images/" + fileName,
                    UploadedOn = DateTime.Now
                };
            }

            _context.Herbs.Add(herb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index)); 
        }


        // GET: Herbs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herb = await _context.Herbs.FindAsync(id);
            if (herb == null)
            {
                return NotFound();
            }
            return View(herb);
        }

        // POST: Herbs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PopularName,LatinName,DialectNames,Aroma,Taste,Habitat,Season,IsPoisonous,IsProtected,UsedPart,Benefits,Description")] Herb herb)
        {
            if (id != herb.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(herb);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HerbExists(herb.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(herb);
        }

        // GET: Herbs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herb = await _context.Herbs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (herb == null)
            {
                return NotFound();
            }

            return View(herb);
        }

        // POST: Herbs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var herb = await _context.Herbs.FindAsync(id);
            if (herb != null)
            {
                _context.Herbs.Remove(herb);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HerbExists(int id)
        {
            return _context.Herbs.Any(e => e.Id == id);
        }
    }
}
