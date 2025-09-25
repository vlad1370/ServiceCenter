using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ServiceCenter.Data;
using ServiceCenter.Models;
using ServiceCenter.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiceCenter.Controllers
{
    public class OrderController : Controller
    {
        private readonly ServiceCenterDbContext _context;

        public OrderController(ServiceCenterDbContext context)
        {
            _context = context;
        }

        // GET: Orders
        public async Task<IActionResult> Index(string searchString, DateTime? startDate, DateTime? endDate)
        {
            var orders = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderFaults)
                    .ThenInclude(of => of.FaultType)
                .AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.CarSerialNumber.Contains(searchString));
            }

            if (startDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate <= endDate.Value);
            }

            ViewData["CurrentFilter"] = searchString;
            ViewData["StartDate"] = startDate?.ToString("yyyy-MM-dd");
            ViewData["EndDate"] = endDate?.ToString("yyyy-MM-dd");

            return View(await orders.OrderByDescending(o => o.OrderDate).ToListAsync());
        }

        // GET: Orders/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderFaults)
                    .ThenInclude(of => of.FaultType)
                        .ThenInclude(ft => ft.Model)
                .Include(o => o.Car)
                    .ThenInclude(c => c.Model)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // GET: Orders/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new OrderCreateViewModel
            {
                OrderDate = DateTime.Now
            };

            await PopulateViewData(viewModel);
            return View(viewModel);
        }

        // POST: Orders/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(OrderCreateViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                // Автоматически находим CarId по серийному номеру
                var car = await _context.Cars
                    .FirstOrDefaultAsync(c => c.SerialNumber == viewModel.CarSerialNumber);

                // Автоматически рассчитываем стоимость на основе выбранных неисправностей
                decimal totalPrice = 0;
                if (viewModel.SelectedFaultIds != null && viewModel.SelectedFaultIds.Any())
                {
                    totalPrice = await _context.FaultTypes
                        .Where(ft => viewModel.SelectedFaultIds.Contains(ft.Id))
                        .SumAsync(ft => ft.RepairCost);
                }

                var order = new Order
                {
                    OrderDate = viewModel.OrderDate,
                    ReturnDate = viewModel.ReturnDate,
                    HasWarranty = viewModel.HasWarranty,
                    WarrantyPeriodDays = viewModel.WarrantyPeriodDays,
                    TotalPrice = totalPrice, // Используем рассчитанную стоимость
                    CustomerId = viewModel.CustomerId,
                    CarSerialNumber = viewModel.CarSerialNumber,
                    CarId = car.Id,
                    EmployeeId = viewModel.EmployeeId
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Добавляем неисправности
                if (viewModel.SelectedFaultIds != null)
                {
                    foreach (var faultId in viewModel.SelectedFaultIds)
                    {
                        _context.OrderFaults.Add(new OrderFault
                        {
                            OrderId = order.Id,
                            FaultTypeId = faultId
                        });
                    }
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            await PopulateViewData(viewModel);
            return View(viewModel);
        }

        // GET: Orders/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.OrderFaults)
                .Include(o => o.Car)
                    .ThenInclude(c => c.Model)
                        .ThenInclude(m => m.FaultTypes)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            var viewModel = new OrderEditViewModel
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                ReturnDate = order.ReturnDate,
                HasWarranty = order.HasWarranty,
                WarrantyPeriodDays = order.WarrantyPeriodDays,
                TotalPrice = order.TotalPrice,
                CustomerId = order.CustomerId,
                CarSerialNumber = order.CarSerialNumber,
                EmployeeId = order.EmployeeId,
                SelectedFaultIds = order.OrderFaults.Select(of => of.FaultTypeId).ToList()
            };

            await PopulateViewData(viewModel);
            return View(viewModel);
        }

        // POST: Orders/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, OrderEditViewModel viewModel)
        {
            if (id != viewModel.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var order = await _context.Orders
                        .Include(o => o.OrderFaults)
                        .FirstOrDefaultAsync(o => o.Id == id);

                    if (order == null) return NotFound();

                    // Обновляем поля
                    order.OrderDate = viewModel.OrderDate;
                    order.ReturnDate = viewModel.ReturnDate;
                    order.HasWarranty = viewModel.HasWarranty;
                    order.WarrantyPeriodDays = viewModel.WarrantyPeriodDays;
                    order.CustomerId = viewModel.CustomerId;
                    order.CarSerialNumber = viewModel.CarSerialNumber;
                    order.EmployeeId = viewModel.EmployeeId;

                    // Обновляем неисправности
                    var currentFaultIds = order.OrderFaults.Select(of => of.FaultTypeId).ToList();
                    var newFaultIds = viewModel.SelectedFaultIds ?? new List<int>();

                    // Удаляем старые
                    var toRemove = order.OrderFaults.Where(of => !newFaultIds.Contains(of.FaultTypeId)).ToList();
                    _context.OrderFaults.RemoveRange(toRemove);

                    // Добавляем новые
                    var toAdd = newFaultIds.Where(fid => !currentFaultIds.Contains(fid))
                        .Select(fid => new OrderFault { OrderId = order.Id, FaultTypeId = fid });
                    await _context.OrderFaults.AddRangeAsync(toAdd);

                    // Автоматически рассчитываем стоимость на основе выбранных неисправностей
                    decimal totalPrice = 0;
                    if (viewModel.SelectedFaultIds != null && viewModel.SelectedFaultIds.Any())
                    {
                        totalPrice = await _context.FaultTypes
                            .Where(ft => viewModel.SelectedFaultIds.Contains(ft.Id))
                            .SumAsync(ft => ft.RepairCost);
                    }

                    order.TotalPrice = totalPrice;

                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!OrderExists(viewModel.Id)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await PopulateViewData(viewModel);
            return View(viewModel);
        }

        // GET: Orders/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var order = await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Employee)
                .Include(o => o.OrderFaults) // Загружаем неисправности
                    .ThenInclude(of => of.FaultType) // Загружаем информацию о типе неисправности
                .AsNoTracking() // Для безопасного чтения
                .FirstOrDefaultAsync(m => m.Id == id);

            if (order == null) return NotFound();

            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }

        private async Task PopulateViewData(OrderCreateViewModel viewModel = null, string carSerialNumber = null)
        {
            ViewBag.AvailableSerialNumbers = await _context.Cars
                .Select(c => c.SerialNumber)
                .Distinct()
                .ToListAsync();

            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName", viewModel?.CustomerId);
            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", viewModel?.EmployeeId);

            var faultTypes = await _context.FaultTypes.Include(ft => ft.Model).ToListAsync();
            ViewBag.FaultTypes = new MultiSelectList(faultTypes, "Id", "Description", viewModel.SelectedFaultIds);
        }

        private async Task PopulateViewData(OrderEditViewModel viewModel)
        {
            ViewBag.Customers = new SelectList(await _context.Customers.ToListAsync(), "Id", "FullName", viewModel.CustomerId);
            ViewBag.Employees = new SelectList(await _context.Employees.ToListAsync(), "Id", "FullName", viewModel.EmployeeId);

            List<FaultType> faultTypes;

            if (!string.IsNullOrEmpty(viewModel.CarSerialNumber))
            {
                // Находим автомобиль по серийному номеру
                var car = await _context.Cars
                    .Include(c => c.Model)
                        .ThenInclude(m => m.FaultTypes)  // Загружаем неисправности модели
                    .FirstOrDefaultAsync(c => c.SerialNumber == viewModel.CarSerialNumber);

                // Берем только неисправности для этой модели
                faultTypes = car?.Model?.FaultTypes?.ToList() ?? new List<FaultType>();
            }
            else
            {
                // Если серийный номер не указан - пустой список
                faultTypes = new List<FaultType>();
            }

            ViewBag.FaultTypes = new MultiSelectList(faultTypes, "Id", "Description", viewModel.SelectedFaultIds);
        }

        // AJAX метод для проверки серийного номера
        public async Task<JsonResult> CheckSerialNumber(string serialNumber)
        {
            var exists = await _context.Cars.AnyAsync(c => c.SerialNumber == serialNumber);
            return Json(new { exists = exists });
        }

        [HttpGet]
        public async Task<JsonResult> GetFaultsByCar(string serialNumber)
        {
            if (string.IsNullOrEmpty(serialNumber))
            {
                return Json(new { success = false, message = "Введите серийный номер" });
            }

            var car = await _context.Cars
                .Include(c => c.Model)
                    .ThenInclude(m => m.FaultTypes)
                .FirstOrDefaultAsync(c => c.SerialNumber == serialNumber);

            if (car == null)
            {
                return Json(new { success = false, message = "Автомобиль не найден" });
            }

            if (car.Model?.FaultTypes?.Any() == true)
            {
                var faults = car.Model.FaultTypes
                    .Select(ft => new {
                        id = ft.Id,
                        text = $"{ft.Description} ({ft.RepairCost.ToString("N2")} руб.)"
                    })
                    .ToList();

                return Json(new { success = true, faults = faults });
            }
            else
            {
                return Json(new { success = false, message = "Для этой модели нет характерных неисправностей" });
            }
        }
    }
}