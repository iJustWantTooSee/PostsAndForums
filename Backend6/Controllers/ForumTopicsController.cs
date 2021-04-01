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
    public class ForumTopicsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ForumTopicsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: ForumTopics
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumTopics.Include(f => f.Creator).Include(f => f.Forum);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumTopics/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }

            return View(forumTopic);
        }

        // GET: ForumTopics/Create
        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Description");
            return View();
        }

        // POST: ForumTopics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatorId,ForumId,Created,Name")] ForumTopic forumTopic)
        {
            if (ModelState.IsValid)
            {
                forumTopic.Id = Guid.NewGuid();
                _context.Add(forumTopic);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", forumTopic.CreatorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Description", forumTopic.ForumId);
            return View(forumTopic);
        }

        // GET: ForumTopics/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", forumTopic.CreatorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Description", forumTopic.ForumId);
            return View(forumTopic);
        }

        // POST: ForumTopics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CreatorId,ForumId,Created,Name")] ForumTopic forumTopic)
        {
            if (id != forumTopic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumTopic);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumTopicExists(forumTopic.Id))
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
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", forumTopic.CreatorId);
            ViewData["ForumId"] = new SelectList(_context.Forums, "Id", "Description", forumTopic.ForumId);
            return View(forumTopic);
        }

        // GET: ForumTopics/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }

            return View(forumTopic);
        }

        // POST: ForumTopics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == id);
            _context.ForumTopics.Remove(forumTopic);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumTopicExists(Guid id)
        {
            return _context.ForumTopics.Any(e => e.Id == id);
        }
    }
}
