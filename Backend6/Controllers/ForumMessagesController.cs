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


        // GET: ForumMessages/Create
        [Authorize]
        public async Task<IActionResult> Create(Guid? forumTopicId)
        {
            if (forumTopicId == null)
            {
                return this.NotFound();
            }

            var forumTopic = await this._context.ForumTopics.SingleOrDefaultAsync(x => x.Id == forumTopicId);

            if (forumTopic == null)
            {
                return this.NotFound();
            }

            ViewBag.ForumTopic = forumTopic;

            return View(new ForumMessageModel());
        }

        // POST: ForumMessages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create(Guid? forumTopicId, ForumMessageModel model)
        {
            if (forumTopicId == null)
            {
                return this.NotFound();
            }

            var forumTopic = await this._context.ForumTopics.SingleOrDefaultAsync(x => x.Id == forumTopicId);

            if (forumTopic == null)
            {
                return this.NotFound();
            }

            var user = await this.userManager.GetUserAsync(this.HttpContext.User);

            if (ModelState.IsValid && user != null)
            {
                var now = DateTime.UtcNow;
                var forumMessage = new ForumMessage
                {
                    Id = Guid.NewGuid(),
                    CreatorId = user.Id,
                    ForumTopicId = forumTopic.Id,
                    Text = model.Text,
                    Created = now,
                    Modified = now
                };
                _context.Add(forumMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new { id = forumTopic.Id });
            }
            ViewBag.ForumTopic = forumTopic;
            return View(model);
        }

        // GET: ForumMessages/Edit/5
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id, Guid? forumTopicId)
        {
            if (id == null || forumTopicId == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages.SingleOrDefaultAsync(m => m.Id == id);

            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == forumTopicId);

            if (forumMessage == null || forumTopic == null || !this.userPermissions.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }

            var model = new ForumMessageModel
            {
                Text = forumMessage.Text
            };

            ViewBag.ForumTopic = forumTopic;
            ViewBag.ForumMessage = forumMessage;
            return View(model);
        }

        // POST: ForumMessages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Edit(Guid? id, Guid? forumTopicId, ForumMessageModel model)
        {
            if (id == null || forumTopicId == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages.SingleOrDefaultAsync(m => m.Id == id);

            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == forumTopicId);

            if (forumMessage == null || forumTopic == null || !this.userPermissions.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                forumMessage.Modified = DateTime.UtcNow;
                forumMessage.Text = model.Text;
                _context.Update(forumMessage);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new { id = forumTopic.Id });
            }
            ViewBag.ForumTopic = forumTopic;
            ViewBag.ForumMessage = forumMessage;
            return View(model);
        }

        // GET: ForumMessages/Delete/5
        [Authorize]
        public async Task<IActionResult> Delete(Guid? id, Guid? forumTopicId)
        {
            if (id == null || forumTopicId == null)
            {
                return NotFound();
            }

            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == forumTopicId);


            var forumMessage = await _context.ForumMessages
                .Include(f => f.Creator)
                .Include(f => f.ForumTopic)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (forumMessage == null || forumTopic == null || !this.userPermissions.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }

            ViewBag.ForumTopic = forumTopic;
            ViewBag.ForumMessage = forumMessage;
            return View(forumMessage);
        }

        // POST: ForumMessages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> DeleteConfirmed(Guid? id, Guid? forumTopicId)
        {
            if (id == null || forumTopicId == null)
            {
                return NotFound();
            }

            var forumMessage = await _context.ForumMessages.SingleOrDefaultAsync(m => m.Id == id);

            var forumTopic = await _context.ForumTopics.SingleOrDefaultAsync(m => m.Id == forumTopicId);

            if (forumMessage == null || forumTopic == null || !this.userPermissions.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }
            ViewBag.ForumTopic = forumTopic;
            ViewBag.ForumMessage = forumMessage;
            _context.ForumMessages.Remove(forumMessage);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ForumTopics", new { id = forumTopic.Id });
        }
    }
}
