using BilkoNavigator_.Data;
using BilkoNavigator_.Models;
using BilkoNavigator_.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BilkoNavigator_.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public AdminController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync("Admin");
            var adminIds = adminUsers.Select(u => u.Id).ToHashSet();

            var normalUsers = await _context.Users
                .AsNoTracking()
                .Where(u => !adminIds.Contains(u.Id))
                .OrderBy(u => u.Email)
                .ToListAsync();

            var userIds = normalUsers.Select(u => u.Id).ToList();

            var findings = await _context.HerbFindings
                .AsNoTracking()
                .Where(f => userIds.Contains(f.UserId))
                .Include(f => f.Herb)
                .Select(f => new
                {
                    f.UserId,
                    HerbName = f.Herb != null ? f.Herb.PopularName : "Неизвестна билка",
                    f.FoundOn
                })
                .ToListAsync();

            var model = new AdminUsersPageViewModel
            {
                Users = normalUsers.Select(u =>
                {
                    var userFindings = findings.Where(f => f.UserId == u.Id).ToList();

                    var herbSummaries = userFindings
                        .GroupBy(f => f.HerbName)
                        .Select(g => new AdminUserHerbSummaryViewModel
                        {
                            HerbName = g.Key,
                            Count = g.Count(),
                            LastFoundOn = g.Max(x => x.FoundOn)
                        })
                        .OrderByDescending(s => s.Count)
                        .ThenBy(s => s.HerbName)
                        .ToList();

                    return new AdminUserRowViewModel
                    {
                        UserId = u.Id,
                        Email = u.Email ?? "(без имейл)",
                        UserName = u.UserName ?? "(без потребителско име)",
                        TotalFindings = userFindings.Count,
                        HerbSummaries = herbSummaries
                    };
                }).ToList()
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                TempData["AdminUsersError"] = "Невалиден потребител.";
                return RedirectToAction(nameof(Users));
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                TempData["AdminUsersError"] = "Потребителят не беше намерен.";
                return RedirectToAction(nameof(Users));
            }

            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                TempData["AdminUsersError"] = "Администратор не може да бъде изтрит.";
                return RedirectToAction(nameof(Users));
            }

            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var userFindings = await _context.HerbFindings
                    .Where(f => f.UserId == id)
                    .ToListAsync();

                var locationIds = userFindings
                    .Select(f => f.LocationId)
                    .Distinct()
                    .ToList();

                if (userFindings.Count > 0)
                {
                    _context.HerbFindings.RemoveRange(userFindings);
                    await _context.SaveChangesAsync();
                }

                if (locationIds.Count > 0)
                {
                    var orphanLocations = await _context.Locations
                        .Where(l => locationIds.Contains(l.Id))
                        .Where(l => !_context.HerbFindings.Any(f => f.LocationId == l.Id))
                        .ToListAsync();

                    if (orphanLocations.Count > 0)
                    {
                        _context.Locations.RemoveRange(orphanLocations);
                        await _context.SaveChangesAsync();
                    }
                }

                var deleteResult = await _userManager.DeleteAsync(user);
                if (!deleteResult.Succeeded)
                {
                    var reason = string.Join("; ", deleteResult.Errors.Select(e => e.Description));
                    throw new InvalidOperationException(reason);
                }

                await transaction.CommitAsync();
                TempData["AdminUsersSuccess"] = "Потребителят е изтрит успешно.";
            }
            catch
            {
                await transaction.RollbackAsync();
                TempData["AdminUsersError"] = "Грешка при изтриване на потребителя.";
            }

            return RedirectToAction(nameof(Users));
        }
    }
}
