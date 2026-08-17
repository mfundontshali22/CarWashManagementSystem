using CarWashOps.Data;
using CarWashOps.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CarWashOps.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            return View(await _context.Employees.OrderBy(e => e.EmployeeName).ToListAsync());
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var employee = await _context.Employees
                .Include(e => e.Washes)
                .FirstOrDefaultAsync(e => e.EmployeeNum == id);

            if (employee == null) return NotFound();
            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.StatusOptions = new SelectList(new[] { "Active", "Off Shift", "On Leave" });
            return View();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EmployeeName,ServiceType,Phone,Status")] Employee employee)
        {
            if (ModelState.IsValid)
            {
                _context.Add(employee);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Employee added.";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.StatusOptions = new SelectList(new[] { "Active", "Off Shift", "On Leave" });
            return View(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null) return NotFound();

            ViewBag.StatusOptions = new SelectList(new[] { "Active", "Off Shift", "On Leave" }, employee.Status);
            return View(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EmployeeNum,EmployeeName,ServiceType,Phone,Status")] Employee employee)
        {
            if (id != employee.EmployeeNum) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(employee);
                    await _context.SaveChangesAsync();
                    TempData["Message"] = "Employee updated.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Employees.AnyAsync(e => e.EmployeeNum == id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.StatusOptions = new SelectList(new[] { "Active", "Off Shift", "On Leave" }, employee.Status);
            return View(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeNum == id);
            if (employee == null) return NotFound();
            return View(employee);
        }

        // POST: Employees/Delete/5
        // Washes handled by this employee are kept; their EmployeeNum is set to NULL
        // automatically by the ON DELETE SET NULL foreign key.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee != null)
            {
                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Employee deleted.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
