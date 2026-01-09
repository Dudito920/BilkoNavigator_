using BilkoNavigator_.Data;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BilkoNavigator_.Controllers
{
    public class HerbFindingsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public HerbFindingsController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: HerbFindings/Create?herbId=5
        public async Task<IActionResult> Create(int herbId)
        {
            var herb = await _context.Herbs.FindAsync(herbId);
            if (herb == null)
                return NotFound();
            if (herb.IsProtected)
                return Forbid();
            ViewBag.HerbId = herbId;
            return View();
        }

        // POST: HerbFindings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int herbId, double latitude, double longitude, string description)
        {
            var herb = await _context.Herbs.FindAsync(herbId);
            if (herb == null)
                return NotFound();
            if (herb.IsProtected)
                return Forbid();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Challenge();
            var location = new Location { Latitude = latitude, Longitude = longitude, Description = description };
            _context.Locations.Add(location);
            await _context.SaveChangesAsync();
            var finding = new HerbFinding
            {
                HerbId = herbId,
                UserId = user.Id, // If UserId is string, adjust accordingly
                LocationId = location.Id,
                FoundOn = DateTime.Now
            };
            _context.HerbFindings.Add(finding);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Herbs", new { id = herbId });
        }
    }
}
