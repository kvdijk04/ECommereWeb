using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;

namespace OnlineMarket.Controllers
{
    public class SubcribersController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }

        public SubcribersController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;

        }

        // GET: Subcribers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Subcriber.ToListAsync());
        }

        // GET: Subcribers/Details/5
        public async Task<IActionResult> Details(int? id)
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

        // GET: Subcribers/Create
        //public IActionResult Create()
        //{
        //    return View();
        //}
        // POST: Subcribers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,Email")] Subcriber subcriber)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        Subcriber esubcriber = _context.Subcriber.Where(x => x.Email == subcriber.Email).FirstOrDefault();
        //        if (esubcriber != null)
        //        {
        //            _notyfService.Error("Tài khoản đã đăng ký");
        //        }
        //        else
        //        {
        //            _context.Add(subcriber);
        //            await _context.SaveChangesAsync();
        //            _notyfService.Success("Bạn đã đăng ký thành công");

        //        }

        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(subcriber);
        //}

        // GET: Subcribers/Edit/5
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

        // POST: Subcribers/Edit/5
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

        // GET: Subcribers/Delete/5
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

        // POST: Subcribers/Delete/5
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
    }
}
