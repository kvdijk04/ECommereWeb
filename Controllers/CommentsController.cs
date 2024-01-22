using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreHero.ToastNotification.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineMarket.Models;
using PagedList.Core;

namespace OnlineMarket.Controllers
{
    public class CommentsController : Controller
    {
        private readonly OnlineShopContext _context;
        public INotyfService _notyfService { get; }

        public CommentsController(OnlineShopContext context, INotyfService notifyfService)
        {
            _context = context;
            _notyfService = notifyfService;
        }

        // GET: Comments
        public async Task<IActionResult> Index(int pid=0, int page=1)
        {
            var pageNumber = page;
            var pageSize = 2;

            List<Comment> lsCmts = new List<Comment>();
            lsCmts = _context.Comments.Include(x => x.Product).ToList();
            PagedList<Comment> models = new PagedList<Comment>(lsCmts.AsQueryable(), pageNumber, pageSize);

            ViewBag.CurrentCateID = pid;
            ViewBag.CurrentPage = pageNumber;
            //var onlineShopContext = _context.Comments.Include(c => c.Product);
            return View(models);
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");
            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Comments/Create
        [Route("tao-review-moi.html")]
        public IActionResult Create()
        {

            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName");

            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("tao-review-moi.html/Create")]
        public async Task<IActionResult> Create([Bind("CommentId,FullName,Phone,Email,ProductId,CommentDescription,Rating,CommentedOn")] Comment comment)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    comment.CommentedOn = DateTime.Now;
                    _context.Add(comment);
                    await _context.SaveChangesAsync();
                    _notyfService.Success("Thêm phản hồi thành công");
                    return RedirectToAction(nameof(Index));
                }
                ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
                
            }
            catch
            {
                return RedirectToAction("tao-review-moi.html");
            }
            return View(comment);

        }

        // GET: Comments/Edit/5
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

        // POST: Comments/Edit/5
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
            ViewData["ProductId"] = new SelectList(_context.Products, "ProductId", "ProductName", comment.ProductId);
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
            return View(comment);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comment);
            await _context.SaveChangesAsync();
            _notyfService.Success("Xóa phản hồi thành công");
            return RedirectToAction(nameof(Index));
  
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }       

    }
}
