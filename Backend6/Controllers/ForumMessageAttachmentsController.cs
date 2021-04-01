using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend6.Data;
using Backend6.Models;

namespace Backend6.Controllers
{
    public class ForumMessageAttachmentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumMessageAttachmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ForumMessageAttachments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumMessageAttachments.Include(f => f.ForumMessage);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumMessageAttachments/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments
                .Include(f => f.ForumMessage)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessageAttachment == null)
            {
                return NotFound();
            }

            return View(forumMessageAttachment);
        }

        // GET: ForumMessageAttachments/Create
        public IActionResult Create()
        {
            ViewData["ForumMessageId"] = new SelectList(_context.ForumMessages, "Id", "CreatorId");
            return View();
        }

        // POST: ForumMessageAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ForumMessageId,Created,FileName,FilePath")] ForumMessageAttachment forumMessageAttachment)
        {
            if (ModelState.IsValid)
            {
                forumMessageAttachment.Id = Guid.NewGuid();
                _context.Add(forumMessageAttachment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ForumMessageId"] = new SelectList(_context.ForumMessages, "Id", "CreatorId", forumMessageAttachment.ForumMessageId);
            return View(forumMessageAttachment);
        }

        // GET: ForumMessageAttachments/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments.SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessageAttachment == null)
            {
                return NotFound();
            }
            ViewData["ForumMessageId"] = new SelectList(_context.ForumMessages, "Id", "CreatorId", forumMessageAttachment.ForumMessageId);
            return View(forumMessageAttachment);
        }

        // POST: ForumMessageAttachments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,ForumMessageId,Created,FileName,FilePath")] ForumMessageAttachment forumMessageAttachment)
        {
            if (id != forumMessageAttachment.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumMessageAttachment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumMessageAttachmentExists(forumMessageAttachment.Id))
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
            ViewData["ForumMessageId"] = new SelectList(_context.ForumMessages, "Id", "CreatorId", forumMessageAttachment.ForumMessageId);
            return View(forumMessageAttachment);
        }

        // GET: ForumMessageAttachments/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments
                .Include(f => f.ForumMessage)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessageAttachment == null)
            {
                return NotFound();
            }

            return View(forumMessageAttachment);
        }

        // POST: ForumMessageAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forumMessageAttachment = await _context.ForumMessageAttachments.SingleOrDefaultAsync(m => m.Id == id);
            _context.ForumMessageAttachments.Remove(forumMessageAttachment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumMessageAttachmentExists(Guid id)
        {
            return _context.ForumMessageAttachments.Any(e => e.Id == id);
        }
    }
}
