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

        

        // GET: Herbs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var herb = await _context.Herbs
                .Include(h => h.Image)
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Herb herb)
        {
            // Логване на ModelState грешки
            foreach (var state in ModelState)
            {
                foreach (var error in state.Value.Errors)
                {
                    Console.WriteLine($"{state.Key}: {error.ErrorMessage}");
                }
            }

            // Премахваме Image от валидацията, защото е [NotMapped]
            ModelState.Remove(nameof(Herb.Image));

            if (!ModelState.IsValid)
            {
                return View(herb);
            }

            // Записване на файл, ако има
            if (herb.ImageFile != null && herb.ImageFile.Length > 0)
            {
                var imagesDir = Path.Combine(_environment.WebRootPath, "images");
               
                Directory.CreateDirectory(imagesDir);

                var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(herb.ImageFile.FileName)}";
                var fullPath = Path.Combine(imagesDir, fileName);

                using var stream = new FileStream(fullPath, FileMode.Create);
                await herb.ImageFile.CopyToAsync(stream);

                herb.Image = new HerbImage
                {
                    ImagePath = "/images/" + fileName,
                    UploadedOn = DateTime.Now
                };
            }

            // Добавяне в базата
            _context.Herbs.Add(herb);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Herbs/Index
       
        public async Task<IActionResult> Index()
        {
            var herbs = await _context.Herbs
                .Include(h => h.Image)
                .ToListAsync();

            return View(herbs);
        }


        // GET: Herbs/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var herb = await _context.Herbs
                .Include(h => h.Image)
                .FirstOrDefaultAsync(h => h.Id == id);

            if (herb == null) return NotFound();

            return View(herb);
        }

        // POST: Herbs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Herb herb)
        {
            if (id != herb.Id) return NotFound();

            ModelState.Remove(nameof(Herb.Image));

            if (!ModelState.IsValid)
                return View(herb);

            var dbHerb = await _context.Herbs
                .Include(h => h.Image)
                .FirstAsync(h => h.Id == id);

            dbHerb.PopularName = herb.PopularName;
            dbHerb.LatinName = herb.LatinName;
            dbHerb.IsPoisonous = herb.IsPoisonous;
            dbHerb.IsProtected = herb.IsProtected;

            if (herb.ImageFile != null)
            {
                var imagesDir = Path.Combine(_environment.WebRootPath, "images");
                Directory.CreateDirectory(imagesDir);

                var fileName = $"{Guid.NewGuid()}_{herb.ImageFile.FileName}";
                var path = Path.Combine(imagesDir, fileName);

                using var stream = new FileStream(path, FileMode.Create);
                await herb.ImageFile.CopyToAsync(stream);

                dbHerb.Image = new HerbImage
                {
                    ImagePath = "/images/" + fileName
                };
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
                await _context.SaveChangesAsync();
            }

            
            return RedirectToAction(nameof(Index));
        }

        private bool HerbExists(int id)
        {
            return _context.Herbs.Any(e => e.Id == id);
        }

        // GET: Herbs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var herb = await _context.Herbs
                .Include(h => h.Image)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (herb == null)
                return NotFound();

            return View(herb);
        }

    }
}
