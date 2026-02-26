using BilkoNavigator_.Data;
using BilkoNavigator_.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
        public class HerbFindingDto
        {
            public int HerbId { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public string? Description { get; set; }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] HerbFindingDto dto)
        {
            if (dto == null)
                return BadRequest("DTO е null");

            if (dto.HerbId == 0)
                return BadRequest("HerbId е 0");

            if (dto.Latitude == 0 || dto.Longitude == 0)
                return BadRequest("Лат/Лон са 0");

            var herb = await _context.Herbs.FindAsync(dto.HerbId);
            if (herb == null)
                return NotFound("Билката не съществува");

            if (herb.IsProtected)
                return Forbid("Защитена билка");

            var userId = _userManager.GetUserId(User);
            if (userId == null)
                return Unauthorized("Не си логнат");

            var location = new Location
            {
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                Country = "BG"
            };

            _context.Locations.Add(location);
            await _context.SaveChangesAsync();

            var finding = new HerbFinding
            {
                HerbId = dto.HerbId,
                UserId = userId,
                LocationId = location.Id
            };

            _context.HerbFindings.Add(finding);
            await _context.SaveChangesAsync();

            return Ok("OK");
        }


   

        [HttpGet]
        public async Task<IActionResult> GetFindings(int herbId)
        {
            var points = await _context.HerbFindings
                .Where(h => h.HerbId == herbId)
                .Include(h => h.Location)
                .Select(h => new
                {
                   latitude= h.Location.Latitude,
                   longitude= h.Location.Longitude,
                   foundOn= h.FoundOn
                })
                .ToListAsync();

            return Json(points);
        }

    }
}
