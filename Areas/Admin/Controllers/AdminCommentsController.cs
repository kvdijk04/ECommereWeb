using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using PagedList.Core;
using AspNetCoreHero.ToastNotification.Abstractions;

namespace OnlineMarket.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminCommentsController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }

        public AdminCommentsController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;
        }
    

        // GET: Admin/AdminComments
        public async Task<IActionResult> Index (int page=1, int PID = 0)
        {
            var pageNumber = page;
            var pageSize = 10;

            List<Comment> lsCmts = new List<Comment>();
            if (PID != 0)
            {
                lsCmts = _context.Comments.Where(c => c.ProductId == PID).ToList();
            }
            else
            {

                lsCmts = _context.Comments.ToList();
            }
            
            PagedList<Comment> models = new PagedList<Comment>(lsCmts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = PID;
            ViewBag.CurrentPage = pageNumber;
            //var onlineShopContext = _context.Comments.Include(c => c.Product);
            ViewData["DanhMuc"] = new SelectList(_context.Products, "ProductId", "ProductName", PID);
            return View(models);
        }
        public IActionResult Filtter(int PID = 0)
        {
            var url = $"/Admin/AdminComments?PID={PID}";
            if (PID == 0)
            {
                url = $"/Admin/AdminComments";
            }

            return Json(new { status = "success", redirectUrl = url });
        }
        // GET: Admin/AdminComments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Admin/AdminComments/Create
        public IActionResult Create()
        {
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            return View();
        }

        // POST: Admin/AdminComments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,FullName,Phone,Email,ProductId,CommentDescription,Rating,CommentedOn")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
            return View(comment);
        }

        // GET: Admin/AdminComments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
            return View(comment);
        }

        // POST: Admin/AdminComments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,FullName,Phone,Email,ProductId,CommentDescription,Rating,CommentedOn")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
            return View(comment);
        }

        // GET: Admin/AdminComments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Product)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Admin/AdminComments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa phản hổi phẩm thành công");
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
