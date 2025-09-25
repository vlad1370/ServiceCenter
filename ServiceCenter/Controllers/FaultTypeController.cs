using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceCenter.Data;
using ServiceCenter.Models;

namespace ServiceCenter.Controllers
{
    public class FaultTypeController : Controller
    {
        private readonly ServiceCenterDbContext _context;

        public FaultTypeController(ServiceCenterDbContext context)
        {
            _context = context;
        }

        // GET: FaultTypes
        public async Task<IActionResult> Index()
        {
            var faultTypes = await _context.FaultTypes
                .Include(ft => ft.Model)
                .ToListAsync();

            return View(faultTypes);
        }

        // GET: FaultTypes/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faultType = await _context.FaultTypes
                .Include(ft => ft.Model)
                .Include(ft => ft.OrderFaults)
                    .ThenInclude(of => of.Order)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (faultType == null)
            {
                return NotFound();
            }

            return View(faultType);
        }

        // GET: FaultTypes/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ModelId"] = new SelectList(await _context.RepairableModels.ToListAsync(), "Id", "Name");
            return View();
        }

        // POST: FaultTypes/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FaultType faultType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(faultType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["ModelId"] = new SelectList(
                await _context.RepairableModels.ToListAsync(),
                "Id", "Name",
                faultType.ModelId);

            return View(faultType);
        }

        // GET: FaultTypes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faultType = await _context.FaultTypes.FindAsync(id);
            if (faultType == null)
            {
                return NotFound();
            }

            ViewData["ModelId"] = new SelectList(
                await _context.RepairableModels.ToListAsync(),
                "Id", "Name",
                faultType.ModelId);

            return View(faultType);
        }

        // POST: FaultTypes/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, FaultType faultType)
        {
            if (id != faultType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(faultType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FaultTypeExists(faultType.Id))
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

            ViewData["ModelId"] = new SelectList(
                await _context.RepairableModels.ToListAsync(),
                "Id", "Name",
                faultType.ModelId);

            return View(faultType);
        }

        // GET: FaultTypes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var faultType = await _context.FaultTypes
                .Include(ft => ft.Model)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (faultType == null)
            {
                return NotFound();
            }

            return View(faultType);
        }

        // POST: FaultTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var faultType = await _context.FaultTypes.FindAsync(id);
            _context.FaultTypes.Remove(faultType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FaultTypeExists(int id)
        {
            return _context.FaultTypes.Any(e => e.Id == id);
        }
    }
}