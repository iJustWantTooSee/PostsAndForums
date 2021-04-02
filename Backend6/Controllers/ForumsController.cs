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
    public class ForumsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissions;

        public ForumsController(ApplicationDbContext context,
             UserManager<ApplicationUser> userManager,
             IUserPermissionsService userPermissions)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissions = userPermissions;
        }

        // GET: Forums
        //public async Task<IActionResult> Index()
        //{
        //    var applicationDbContext = _context.Forums.Include(f => f.ForumCategory);
        //    return View(await applicationDbContext.ToListAsync());
        //}

        // GET: Forums/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .Include(f => f.ForumCategory)
                .Include(t=> t.ForumTopics)
                .ThenInclude(x=>x.Creator)
                .Include(f=>f.ForumTopics)
                .ThenInclude(m=>m.ForumMessages)
                .SingleOrDefaultAsync(m => m.Id == id);

            if (forum == null)
            {
                return NotFound();
            }

            return View(forum);
        }

        // GET: Forums/Create
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Create(Guid? categoryId)
        {
            if (categoryId == null)
            {
                return this.NotFound();
            }

            var forumCategory = await this._context.ForumCategories.SingleOrDefaultAsync(x => x.Id == categoryId);

            if (forumCategory == null)
            {
                return this.NotFound();
            }
            ViewBag.ForumCategory = forumCategory;
            return View(new ForumModel());
        }

        // POST: Forums/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Create(Guid categoryId, ForumModel model)
        {
            if (categoryId == null)
            {
                return this.NotFound();
            }

            var forumCategory = await this._context.ForumCategories.SingleOrDefaultAsync(x => x.Id == categoryId);

            if (forumCategory == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                var forum = new Forum
                {
                    Id = Guid.NewGuid(),
                    ForumCategoryId = forumCategory.Id,
                    Name = model.Name,
                    Description = model.Description
                };
                _context.Add(forum);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ForumCategories");
            }
            ViewBag.ForumCategory = forumCategory;
            return this.View(model);
        }

        // GET: Forums/Edit/5
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.Id == id);
            if (forum == null)
            {
                return NotFound();
            }

            var model = new ForumModel
            {
                Name = forum.Name,
                Description = forum.Description
            };

            ViewBag.Forum = forum;
            return View(model);
        }

        // POST: Forums/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Edit(Guid? id, ForumModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.Id == id);
          
            if (forum == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                forum.Name = model.Name;
                forum.Description = model.Description;
                _context.Update(forum);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "ForumCategories");
            }
            ViewBag.Forum = forum;
            return View(model);
        }

        // GET: Forums/Delete/5
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forum = await _context.Forums
                .Include(f => f.ForumCategory)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forum == null)
            {
                return NotFound();
            }
            ViewBag.Forum = forum;
            return View(forum);
        }

        // POST: Forums/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forum = await _context.Forums.SingleOrDefaultAsync(m => m.Id == id);
            _context.Forums.Remove(forum);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ForumCategories");
        }


    }
}
