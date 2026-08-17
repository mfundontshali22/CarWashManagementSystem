using CarWashOps.Data;
using CarWashOps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarWashOps.Controllers
{
    public class WashesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public WashesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Washes
        public async Task<IActionResult> Index()
        {
            var washes = _context.Washes
                .Include(w => w.Employee)
                .Include(w => w.Client)
                .OrderByDescending(w => w.WashDate)
                .ThenByDescending(w => w.WashTime);

            return View(await washes.ToListAsync());
        }

        // GET: Washes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var wash = await _context.Washes
                .Include(w => w.Employee)
                .Include(w => w.Client)
                .FirstOrDefaultAsync(w => w.CarId == id);

            if (wash == null) return NotFound();
            return View(wash);
        }

        // GET: Washes/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View(new Wash { WashDate = DateTime.Today, WashTime = DateTime.Now.TimeOfDay });
        }

        // POST: Washes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CarName,LicensePlate,WashTime,WashDate,ServiceType,Price,Status,EmployeeNum,ClientId")] Wash wash)
        {
            if (ModelState.IsValid)
            {
                _context.Add(wash);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Wash logged successfully.";
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(wash.EmployeeNum, wash.ClientId, wash.ServiceType, wash.Status);
            return View(wash);
        }

        // GET: Washes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var wash = await _context.Washes.FindAsync(id);
            if (wash == null) return NotFound();

            PopulateDropdowns(wash.EmployeeNum, wash.ClientId, wash.ServiceType, wash.Status);
            return View(wash);
        }

        // POST: Washes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("CarId,CarName,LicensePlate,WashTime,WashDate,ServiceType,Price,Status,EmployeeNum,ClientId")] Wash wash)
        {
            if (id != wash.CarId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wash);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Wash updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Washes.AnyAsync(e => e.CarId == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            PopulateDropdowns(wash.EmployeeNum, wash.ClientId, wash.ServiceType, wash.Status);
            return View(wash);
        }

        // GET: Washes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var wash = await _context.Washes
                .Include(w => w.Employee)
                .Include(w => w.Client)
                .FirstOrDefaultAsync(w => w.CarId == id);

            if (wash == null) return NotFound();
            return View(wash);
        }

        // POST: Washes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var wash = await _context.Washes.FindAsync(id);
            if (wash != null)
            {
                _context.Washes.Remove(wash);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Wash deleted.";
            }
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDropdowns(int? selectedEmployee = null, int? selectedClient = null,
            string? selectedService = null, string? selectedStatus = null)
        {
            ViewBag.EmployeeNum = new SelectList(
                _context.Employees.OrderBy(e => e.EmployeeName), "EmployeeNum", "EmployeeName", selectedEmployee);

            ViewBag.ClientId = new SelectList(
                _context.Clients.OrderBy(c => c.ClientName), "ClientId", "ClientName", selectedClient);

            ViewBag.ServiceTypes = new SelectList(new[]
            {
                "Exterior Wash", "Full Valet", "Interior Detail", "Wax & Polish", "Engine Clean"
            }, selectedService);

            ViewBag.StatusOptions = new SelectList(new[] { "Waiting", "In Progress", "Done" }, selectedStatus);
        }
    }
}
