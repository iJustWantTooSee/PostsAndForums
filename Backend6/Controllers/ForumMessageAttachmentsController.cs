using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Backend6.Data;
using Backend6.Models;
using Backend6.Models.ForumViewModels;
using Backend6.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Net.Http.Headers;

namespace Backend6.Controllers
{
    [Authorize]
    public class ForumMessageAttachmentsController : Controller
    {
        private static readonly HashSet<String> AllowedExtensions = new HashSet<String> { ".jpg", ".jpeg", ".png", ".gif" };

        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissions;
        private readonly IHostingEnvironment hostingEnvironment;

        public ForumMessageAttachmentsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
             IUserPermissionsService userPermissions,
              IHostingEnvironment hostingEnvironment)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissions = userPermissions;
            this.hostingEnvironment = hostingEnvironment;
        }

        

        // GET: ForumMessageAttachments/Create
        [Authorize]
        public async  Task<IActionResult> Create(Guid? messageId, Guid? forumTopicId)
        {
            if (messageId == null || forumTopicId == null)
            {
                return this.NotFound();
            }

            var message = await this._context.ForumMessages.SingleOrDefaultAsync(x => x.Id == messageId);
            var forumTopic = await this._context.ForumTopics.SingleOrDefaultAsync(x => x.Id == forumTopicId);

            if (message == null || forumTopic == null || !this.userPermissions.CanEditForumMessage(message))
            {
                return this.NotFound();
            }

            ViewBag.ForumMessage = message;
            ViewBag.ForumTopic = forumTopic;
            return View(new ForumMessageAttachmentModel());
        }

        // POST: ForumMessageAttachments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Guid? messageId, Guid? forumTopicId, ForumMessageAttachmentModel model)
        {
            if (messageId == null || forumTopicId == null)
            {
                return this.NotFound();
            }

            var message = await this._context.ForumMessages.SingleOrDefaultAsync(x => x.Id == messageId);
            var forumTopic = await this._context.ForumTopics.SingleOrDefaultAsync(x => x.Id == forumTopicId);

            if (message == null || forumTopic == null || !this.userPermissions.CanEditForumMessage(message))
            {
                return this.NotFound();
            }

            if (model.File == null)
            {
                this.ModelState.AddModelError(nameof(model.File), "This is empty");
                ViewBag.ForumMessage = message;
                ViewBag.ForumTopic = forumTopic;
                return View(model);
            }

            var fileName = Path.GetFileName(ContentDispositionHeaderValue.Parse(model.File.ContentDisposition).FileName.Value.Trim('"'));
            var fileExt = Path.GetExtension(fileName);
            if (!ForumMessageAttachmentsController.AllowedExtensions.Contains(fileExt))
            {
                this.ModelState.AddModelError(nameof(model.File), "This file type is prohibited");
            }


            if (ModelState.IsValid)
            {
                var forumMessageAttachment = new ForumMessageAttachment
                {
                    Id = Guid.NewGuid(),
                    ForumMessageId = message.Id
                };

                var attachmentPath = Path.Combine(this.hostingEnvironment.WebRootPath, "attachments", forumMessageAttachment.Id.ToString("N") + fileExt);
                forumMessageAttachment.FilePath = $"/attachments/{forumMessageAttachment.Id:N}{fileExt}";
                using (var fileStream = new FileStream(attachmentPath, FileMode.CreateNew, FileAccess.ReadWrite, FileShare.Read))
                {
                    await model.File.CopyToAsync(fileStream);
                }

                forumMessageAttachment.FileName = forumMessageAttachment.FilePath;
                _context.Add(forumMessageAttachment);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "ForumTopics", new { id = forumTopic.Id });
            }
            ViewBag.ForumMessage = message;
            ViewBag.ForumTopic = forumTopic;
            return View(model);
        }


        // GET: ForumMessageAttachments/Delete/5
        public async Task<IActionResult> Delete(Guid? id, Guid? forumMessageId, Guid? forumTopicId)
        {
            if (id == null || forumMessageId ==null || forumTopicId == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments
                .Include(f => f.ForumMessage)
                .SingleOrDefaultAsync(m => m.Id == id);
            var forumTopic = await this._context.ForumTopics.SingleOrDefaultAsync(x => x.Id == forumTopicId);
            var forumMessage = await this._context.ForumMessages.SingleOrDefaultAsync(x => x.Id == forumMessageId);
            if (forumMessageAttachment == null || forumTopicId == null || !this.userPermissions.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }

            ViewBag.ForumMessage = forumMessage;
            ViewBag.ForumTopic = forumTopic;

            return View(forumMessageAttachment);
        }

        // POST: ForumMessageAttachments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid? id, Guid? forumMessageId, Guid? forumTopicId)
        {
            if (id == null || forumMessageId == null || forumTopicId == null)
            {
                return NotFound();
            }

            var forumMessageAttachment = await _context.ForumMessageAttachments
                .Include(f => f.ForumMessage)
                .SingleOrDefaultAsync(m => m.Id == id);
            var forumTopic = await this._context.ForumTopics.SingleOrDefaultAsync(x => x.Id == forumTopicId);
            var forumMessage = await this._context.ForumMessages.SingleOrDefaultAsync(x => x.Id == forumMessageId);
            if (forumMessageAttachment == null || forumTopicId == null || !this.userPermissions.CanEditForumMessage(forumMessage))
            {
                return NotFound();
            }
            ViewBag.ForumMessage = forumMessage;
            ViewBag.ForumTopic = forumTopic;
            var attachmentPath = Path.Combine(this.hostingEnvironment.WebRootPath, "attachments", forumMessageAttachment.Id.ToString("N") + Path.GetExtension(forumMessageAttachment.FilePath));
            System.IO.File.Delete(attachmentPath);
            _context.ForumMessageAttachments.Remove(forumMessageAttachment);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "ForumTopics", new { id = forumTopic.Id });
        }
    }
}
