using BilkoNavigator_.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BilkoNavigator_.Controllers
{
    public class MapController : Controller
    {
        private readonly AppDbContext _context;

        public MapController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("/map")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("/map/points")]
        public async Task<IActionResult> GetPoints(int? herbId = null, bool onlyMine = false)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var isAdmin = User.IsInRole("Admin");

            var query = _context.HerbFindings
                .AsNoTracking()
                .Include(f => f.Location)
                .Include(f => f.Herb)
                    .ThenInclude(h => h.Image)
                .Where(f => f.Location != null && f.Herb != null);

            if (herbId.HasValue)
            {
                query = query.Where(f => f.HerbId == herbId.Value);
            }

            if (onlyMine)
            {
                if (string.IsNullOrEmpty(currentUserId))
                {
                    return Json(Array.Empty<object>());
                }

                query = query.Where(f => f.UserId == currentUserId);
            }

            var points = await query
                .Select(f => new
                {
                    id = f.Id,
                    herbId = f.HerbId,
                    herbName = f.Herb.PopularName,
                    herbImagePath = f.Herb.Image != null ? f.Herb.Image.ImagePath : null,
                    ownerUserId = f.UserId,
                    latitude = f.Location.Latitude,
                    longitude = f.Location.Longitude,
                    foundOn = f.FoundOn
                })
                .ToListAsync();

            var pointsWithPermissions = points.Select(p => new
            {
                p.id,
                p.herbId,
                p.herbName,
                p.herbImagePath,
                p.latitude,
                p.longitude,
                p.foundOn,
                canDelete = isAdmin || (!string.IsNullOrEmpty(currentUserId) && p.ownerUserId == currentUserId)
            });

            return Json(pointsWithPermissions);
        }

        [HttpGet("/map/herbs")]
        public async Task<IActionResult> GetHerbs()
        {
            var herbs = await _context.Herbs
                .AsNoTracking()
                .Include(h => h.Image)
                .OrderBy(h => h.PopularName)
                .Select(h => new
                {
                    herbId = h.Id,
                    herbName = h.PopularName,
                    herbImagePath = h.Image != null ? h.Image.ImagePath : null
                })
                .ToListAsync();

            return Json(herbs);
        }

        [Authorize]
        [HttpPost("/map/points/{id:int}/delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePoint(int id)
        {
            var finding = await _context.HerbFindings
                .FirstOrDefaultAsync(f => f.Id == id);

            if (finding == null)
            {
                return NotFound();
            }

            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var canDelete = User.IsInRole("Admin")
                || (!string.IsNullOrEmpty(currentUserId) && finding.UserId == currentUserId);

            if (!canDelete)
            {
                return Forbid();
            }

            var locationId = finding.LocationId;

            _context.HerbFindings.Remove(finding);
            await _context.SaveChangesAsync();

            var isLocationStillUsed = await _context.HerbFindings
                .AnyAsync(f => f.LocationId == locationId);

            if (!isLocationStillUsed)
            {
                var location = await _context.Locations.FindAsync(locationId);
                if (location != null)
                {
                    _context.Locations.Remove(location);
                    await _context.SaveChangesAsync();
                }
            }

            return Ok(new { success = true });
        }
    }
}
