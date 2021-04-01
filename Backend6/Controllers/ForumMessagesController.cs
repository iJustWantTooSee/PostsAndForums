using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend6.Data;
using Backend6.Models;
using Backend6.Models.ForumViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Backend6.Services;

namespace Backend6.Controllers
{
    [Authorize]
    public class ForumMessagesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissions;

        public ForumMessagesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
             IUserPermissionsService userPermissions)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissions = userPermissions;
        }

        // GET: ForumMessages
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.ForumMessages.Include(f => f.Creator).Include(f => f.ForumTopic);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: ForumMessages/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id, Guid? forumId)
        {
            if (id == null || forumId == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages
                .Include(f => f.Creator)
                .Include(f => f.ForumTopic)
                .Include(f=>f.ForumMessageAttachments)
                .SingleOrDefaultAsync(m => m.Id == id);
            var forum = await this._context.Forums.SingleOrDefaultAsync(x => x.Id == forumId);

            if (forumMessage == null || forum == null)
            {
                return NotFound();
            }

            return View(forumMessage);
        }

        // GET: ForumMessages/Create
        public IActionResult Create()
        {
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id");
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "CreatorId");
            return View();
        }

        // POST: ForumMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CreatorId,ForumTopicId,Created,Modified,Text")] ForumMessage forumMessage)
        {
            if (ModelState.IsValid)
            {
                forumMessage.Id = Guid.NewGuid();
                _context.Add(forumMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", forumMessage.CreatorId);
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "CreatorId", forumMessage.ForumTopicId);
            return View(forumMessage);
        }

        // GET: ForumMessages/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages.SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessage == null)
            {
                return NotFound();
            }
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", forumMessage.CreatorId);
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "CreatorId", forumMessage.ForumTopicId);
            return View(forumMessage);
        }

        // POST: ForumMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,CreatorId,ForumTopicId,Created,Modified,Text")] ForumMessage forumMessage)
        {
            if (id != forumMessage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumMessage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumMessageExists(forumMessage.Id))
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
            ViewData["CreatorId"] = new SelectList(_context.Users, "Id", "Id", forumMessage.CreatorId);
            ViewData["ForumTopicId"] = new SelectList(_context.ForumTopics, "Id", "CreatorId", forumMessage.ForumTopicId);
            return View(forumMessage);
        }

        // GET: ForumMessages/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages
                .Include(f => f.Creator)
                .Include(f => f.ForumTopic)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumMessage == null)
            {
                return NotFound();
            }

            return View(forumMessage);
        }

        // POST: ForumMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forumMessage = await _context.ForumMessages.SingleOrDefaultAsync(m => m.Id == id);
            _context.ForumMessages.Remove(forumMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumMessageExists(Guid id)
        {
            return _context.ForumMessages.Any(e => e.Id == id);
        }
    }
}
