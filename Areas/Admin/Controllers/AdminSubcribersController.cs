using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminSubcribersController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }

        public AdminSubcribersController(OnlineShopContext context, INotyfService notifyfService)
        {
            _notyfService = notifyfService;

            _context = context;
        }

        // GET: Admin/AdminSubcribers
        public async Task<IActionResult> Index()
        {

            return View(await _context.Subcriber.ToListAsync());
        }
        // GET: Admin/AdminSubcribers/Details/5
        public async Task<IActionResult> Details(int? id, Customer customer)
        {

            if (id == null)
            {
                return NotFound();
            }

            var subcriber = await _context.Subcriber
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subcriber == null)
            {
                return NotFound();
            }

            return View(subcriber);
        }

        // GET: Admin/AdminSubcribers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminSubcribers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Email")] Subcriber subcriber)
        {
            if (ModelState.IsValid)
            {
                _context.Add(subcriber);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(subcriber);
        }
        
        // GET: Admin/AdminSubcribers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcriber = await _context.Subcriber.FindAsync(id);
            if (subcriber == null)
            {
                return NotFound();
            }
            return View(subcriber);
        }

        // POST: Admin/AdminSubcribers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Email")] Subcriber subcriber)
        {
            if (id != subcriber.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(subcriber);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SubcriberExists(subcriber.Id))
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
            return View(subcriber);
        }

        // GET: Admin/AdminSubcribers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var subcriber = await _context.Subcriber
                .FirstOrDefaultAsync(m => m.Id == id);
            if (subcriber == null)
            {
                return NotFound();
            }

            return View(subcriber);
        }

        // POST: Admin/AdminSubcribers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var subcriber = await _context.Subcriber.FindAsync(id);
            _context.Subcriber.Remove(subcriber);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        private bool SubcriberExists(int id)
        {
            return _context.Subcriber.Any(e => e.Id == id);
        }
        public IActionResult Export()
        {
            var users = _context.Subcriber.ToList();
            var builder = new StringBuilder();
            builder.AppendLine("Id,Email,FullName,Phone");
            foreach (var u in users)
            {
                builder.AppendLine($"{u.Id},{u.Email},{u.FullName},{u.Phone}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "subcribers.csv");
        }
        public IActionResult ExportText()
        {
            var users = _context.Subcriber.ToList();
            var builder = new StringBuilder();
            builder.AppendLine("Id,Email,FullName,Phone");
            foreach (var u in users)
            {
                builder.AppendLine($"{u.Email},{u.FullName},{u.Phone}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "subcribers.txt");
        }
        public IActionResult ExportToExcel()
        {
            using (var workbook = new XLWorkbook())
            {
                var users = _context.Subcriber.ToList();

                var worksheet = workbook.Worksheets.Add("Users");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Email";
                worksheet.Cell(currentRow, 3).Value = "FullName";
                worksheet.Cell(currentRow, 4).Value = "Phone";

                foreach (var u in users)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = u.Id;
                    worksheet.Cell(currentRow, 2).Value = u.Email;
                    worksheet.Cell(currentRow, 3).Value = u.FullName;
                    worksheet.Cell(currentRow, 4).Value = u.Phone;

                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "subcribers.xlsx");
                }

            }
        }
    }
}
