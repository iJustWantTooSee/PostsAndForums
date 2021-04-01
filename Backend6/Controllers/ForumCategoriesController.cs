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
    public class ForumCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IUserPermissionsService userPermissions;

        public ForumCategoriesController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IUserPermissionsService userPermissions)
        {
            _context = context;
            this.userManager = userManager;
            this.userPermissions = userPermissions;
        }

        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var items = await this._context.ForumCategories
                .Include(x => x.Forums)
                .ToListAsync();
            return this.View(items);
        }

        // GET: ForumCategories/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumCategory = await _context.ForumCategories
                .Include(x => x.Forums)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumCategory == null)
            {
                return NotFound();
            }

            return View(forumCategory);
        }

        // GET: ForumCategories/Create
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public IActionResult Create()
        {
            return View(new ForumCategoryModel());
        }

        // POST: ForumCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Create(ForumCategoryModel model)
        {
            var user = await this.userManager.GetUserAsync(this.HttpContext.User);
            if (ModelState.IsValid && user != null)
            {
                var forumCategory = new ForumCategory
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name
                };
                _context.Add(forumCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: ForumCategories/Edit/5
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumCategory = await _context.ForumCategories.SingleOrDefaultAsync(m => m.Id == id);
            if (forumCategory == null)
            {
                return NotFound();
            }

            var model = new ForumCategoryModel
            {
                Name = forumCategory.Name
            };
            ViewBag.ForumCategory = forumCategory;
            return View(model);
        }

        // POST: ForumCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Edit(Guid? id, ForumCategoryModel model)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumCategory = await this._context.ForumCategories.SingleOrDefaultAsync(x => x.Id == id);

            if (forumCategory == null)
            {
                return this.NotFound();
            }

            if (ModelState.IsValid)
            {
                forumCategory.Name = model.Name;
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ForumCategory = forumCategory;
            return View(forumCategory);
        }

        // GET: ForumCategories/Delete/5
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumCategory = await _context.ForumCategories
                .Include(x=>x.Forums)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (forumCategory == null)
            {
                return NotFound();
            }
            ViewBag.ForumCategory = forumCategory;
            return View(forumCategory);
        }

        // POST: ForumCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = ApplicationRoles.Administrators)]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var forumCategory = await _context.ForumCategories.SingleOrDefaultAsync(m => m.Id == id);
            _context.ForumCategories.Remove(forumCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
