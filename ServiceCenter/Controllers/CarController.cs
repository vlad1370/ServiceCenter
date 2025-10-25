using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceCenter.Data;
using ServiceCenter.Models;


namespace ServiceCenter.Controllers
{
    public class CarController : Controller
    {
        private readonly ServiceCenterDbContext _context;

        public CarController(ServiceCenterDbContext context)
        {
            _context = context;
        }

        // GET: Cars
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.Customer)
                .ToListAsync();

            return View(cars);
        }

        // GET: Cars/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.Customer)
                .Include(c => c.Orders)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Cars/Create
        public async Task<IActionResult> Create()
        {
            ViewData["ModelId"] = new SelectList(await _context.RepairableModels.ToListAsync(), "Id", "Name");
            ViewData["CustomerId"] = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName");
            return View();
        }

        // POST: Cars/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SerialNumber,ModelId,CustomerId")] Car car)
        {
            if (ModelState.IsValid)
            {
                // Проверка уникальности серийного номера
                if (await _context.Cars.AnyAsync(c => c.SerialNumber == car.SerialNumber))
                {
                    ModelState.AddModelError("SerialNumber", "Автомобиль с таким серийным номером уже существует");
                    await PopulateViewData(car);
                    return View(car);
                }

                _context.Add(car);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Автомобиль успешно добавлен";
                return RedirectToAction(nameof(Index));
            }

            await PopulateViewData(car);
            return View(car);
        }


        // GET: Cars/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            await PopulateViewData(car);
            return View(car);
        }

        // POST: Cars/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Car car)
        {
            if (id != car.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(car);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CarExists(car.Id))
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

            await PopulateViewData(car);
            return View(car);
        }

        // GET: Cars/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                .Include(c => c.Customer)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Cars/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Автомобиль удалён";
            return RedirectToAction(nameof(Index));
        }

        private bool CarExists(int id)
        {
            return _context.Cars.Any(e => e.Id == id);
        }

        private async Task PopulateViewData(Car car = null)
        {
            ViewData["ModelId"] = new SelectList(
                await _context.RepairableModels.ToListAsync(),
                "Id", "Name",
                car?.ModelId);

            ViewData["CustomerId"] = new SelectList(
                await _context.Customers.ToListAsync(),
                "Id", "FullName",
                car?.CustomerId);
        }
    }
}