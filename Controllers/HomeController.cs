using CarWashOps.Data;
using CarWashOps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarWashOps.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var vm = new DashboardViewModel
            {
                WashesToday = await _context.Washes.CountAsync(w => w.WashDate == today),
                RevenueToday = await _context.Washes
                    .Where(w => w.WashDate == today)
                    .SumAsync(w => (decimal?)w.Price) ?? 0,
                ActiveEmployees = await _context.Employees.CountAsync(e => e.Status == "Active"),
                TotalClients = await _context.Clients.CountAsync(),

                InProgress = await _context.Washes
                    .Include(w => w.Employee)
                    .Where(w => w.WashDate == today && w.Status == "In Progress")
                    .OrderBy(w => w.WashTime)
                    .ToListAsync(),

                Waiting = await _context.Washes
                    .Where(w => w.WashDate == today && w.Status == "Waiting")
                    .OrderBy(w => w.WashTime)
                    .ToListAsync(),

                RecentWashes = await _context.Washes
                    .Include(w => w.Employee)
                    .Include(w => w.Client)
                    .OrderByDescending(w => w.CarId)
                    .Take(6)
                    .ToListAsync()
            };

            return View(vm);
        }

        public IActionResult Error() => View();
    }

    public class DashboardViewModel
    {
        public int WashesToday { get; set; }
        public decimal RevenueToday { get; set; }
        public int ActiveEmployees { get; set; }
        public int TotalClients { get; set; }
        public List<Wash> InProgress { get; set; } = new();
        public List<Wash> Waiting { get; set; } = new();
        public List<Wash> RecentWashes { get; set; } = new();
    }
}
