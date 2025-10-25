using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceCenter.Data;
using ServiceCenter.Models;


namespace ServiceCenter.Controllers
{
    public class RepairableModelController : Controller
    {
        private readonly ServiceCenterDbContext _context;

        public RepairableModelController(ServiceCenterDbContext context)
        {
            _context = context;
        }

        // GET: RepairableModels
        public async Task<IActionResult> Index()
        {
            return View(await _context.RepairableModels.ToListAsync());
        }

        // GET: RepairableModels/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repairableModel = await _context.RepairableModels
                .Include(rm => rm.FaultTypes)
                .Include(rm => rm.Cars)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (repairableModel == null)
            {
                return NotFound();
            }

            return View(repairableModel);
        }

        // GET: RepairableModels/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: RepairableModels/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RepairableModel repairableModel)
        {
            if (ModelState.IsValid)
            {
                _context.Add(repairableModel);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Модель успешно добавлена";
                return RedirectToAction(nameof(Index));
            }
            return View(repairableModel);
        }

        // GET: RepairableModels/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repairableModel = await _context.RepairableModels.FindAsync(id);
            if (repairableModel == null)
            {
                return NotFound();
            }
            return View(repairableModel);
        }

        // POST: RepairableModels/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, RepairableModel repairableModel)
        {
            if (id != repairableModel.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(repairableModel);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RepairableModelExists(repairableModel.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                TempData["SuccessMessage"] = "Изменения сохранены";
                return RedirectToAction(nameof(Index));
            }
            return View(repairableModel);
        }

        // GET: RepairableModels/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var repairableModel = await _context.RepairableModels
                .FirstOrDefaultAsync(m => m.Id == id);
            if (repairableModel == null)
            {
                return NotFound();
            }

            return View(repairableModel);
        }

        // POST: RepairableModels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var repairableModel = await _context.RepairableModels.FindAsync(id);
            _context.RepairableModels.Remove(repairableModel);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Модель удалена";
            return RedirectToAction(nameof(Index));
        }

        private bool RepairableModelExists(int id)
        {
            return _context.RepairableModels.Any(e => e.Id == id);
        }
    }
}