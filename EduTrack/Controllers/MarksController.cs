using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EduTrack.Data;
using EduTrack.Models;
using Microsoft.AspNetCore.Identity;

namespace EduTrack.Controllers
{
    public class MarksController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public MarksController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Marks
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Mark.Include(m => m.Student).Include(m => m.Subject).Include(m => m.Teacher);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Marks/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Mark
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // GET: Marks/Create
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Email");  // Display email for clarity
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");  // Display name for clarity
            return View();
        }

        // POST: Marks/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value,StudentId,SubjectId")] Mark mark)
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);

            if (teacher == null)
            {
                return Forbid();  // or handle accordingly if the teacher is not found
            }

            if (ModelState.IsValid)
            {
                mark.TeacherId = teacher.Id;
                mark.DateTime = DateTime.Now;
                _context.Add(mark);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        // GET: Marks/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Mark.FindAsync(id);
            if (mark == null)
            {
                return NotFound();
            }
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        // POST: Marks/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value,StudentId,SubjectId")] Mark mark)
        {
            if (id != mark.Id)
            {
                return NotFound();
            }

            var existingMark = await _context.Mark.FindAsync(id);
            if (existingMark == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    existingMark.Value = mark.Value;
                    existingMark.StudentId = mark.StudentId;
                    existingMark.SubjectId = mark.SubjectId;
                    _context.Update(existingMark);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MarkExists(mark.Id))
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
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        // GET: Marks/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mark = await _context.Mark
                .Include(m => m.Student)
                .Include(m => m.Subject)
                .Include(m => m.Teacher)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (mark == null)
            {
                return NotFound();
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Mark.FindAsync(id);
            if (mark != null)
            {
                _context.Mark.Remove(mark);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MarkExists(int id)
        {
            return _context.Mark.Any(e => e.Id == id);
        }
    }
}
