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
    public class ForumTopicsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissions;

        public ForumTopicsController(ApplicationDbContext context,
             UserManager<ApplicationUser> userManager,
             IUserPermissionsService userPermissions)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissions = userPermissions;
        }

        // GET: ForumTopics
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.ForumTopics.Include(f => f.Creator).Include(f => f.Forum);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: ForumTopics/Details/5

        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(f => f.Creator)
                .Include(f => f.Forum)
                .Include(f=>f.ForumMessages)
                .ThenInclude(f=>f.Creator)
                .Include(f=>f.ForumMessages)
                .ThenInclude(f=>f.ForumMessageAttachments)
                .Include(f=>f.Creator)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null)
            {
                return NotFound();
            }

            return View(forumTopic);
        }

        // GET: ForumTopics/Create
        [Authorize]
        public async Task<IActionResult> Create(Guid forumId)
        {
            if (forumId == null)
            {
                return this.NotFound();
            }

            var forum = await this._context.Forums.SingleOrDefaultAsync(x => x.Id == forumId);

            if (forum == null)
            {
                return this.NotFound();
            }

            ViewBag.Forum = forum;

            return View(new ForumTopicModel());
        }

        // POST: ForumTopics/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Guid forumId, ForumTopicModel model)
        {
            if (forumId == null)
            {
                return this.NotFound();
            }

            var forum = await this._context.Forums
                .SingleOrDefaultAsync(x => x.Id == forumId);

            if (forum == null)
            {
                return this.NotFound();
            }

            var user = await this.userManager.GetUserAsync(this.HttpContext.User);

            if (ModelState.IsValid && user != null)
            {
                var now = DateTime.UtcNow;

                var forumTopic = new ForumTopic
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    Created = now,
                    CreatorId = user.Id,
                    ForumId = forum.Id
                };
                _context.Add(forumTopic);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Forums", new { id = forum.Id });
            }
            ViewBag.Forum = forum;
            return View(model);
        }

        // GET: ForumTopics/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(x => x.Creator)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (forumTopic == null && !userPermissions.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }

            var model = new ForumTopicModel
            {
                Name = forumTopic.Name
            };

            ViewBag.ForumTopic = forumTopic;
            return View(model);
        }

        // POST: ForumTopics/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id, ForumTopicModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics
                .Include(x => x.Creator)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (forumTopic == null && !userPermissions.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                forumTopic.Name = model.Name;
                forumTopic.Created = DateTime.UtcNow;
                _context.Update(forumTopic);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", "Forums", new { id = forumTopic.ForumId });
            }
            ViewBag.ForumTopic = forumTopic;
            return View(model);
        }

        // GET: ForumTopics/Delete/5
        [Authorize]
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
            if (forumTopic == null || !this.userPermissions.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }

            ViewBag.ForumTopic = forumTopic;

            return View(forumTopic);
        }

        // POST: ForumTopics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == id);
            if (forumTopic == null || !this.userPermissions.CanEditForumTopic(forumTopic))
            {
                return NotFound();
            }
            _context.ForumTopics.Remove(forumTopic);
            await _context.SaveChangesAsync();
            ViewBag.ForumTopic = forumTopic;
            return RedirectToAction("Details", "Forums", new { id = forumTopic.ForumId });
        }
    }
}
