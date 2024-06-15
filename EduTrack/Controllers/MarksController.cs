using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EduTrack.Data;
using EduTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

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
            var user = await _userManager.GetUserAsync(User);

            if (User.IsInRole("Admin"))
            {
                var marks = await _context.Mark
                    .Include(m => m.Student)
                    .Include(m => m.Subject)
                    .Include(m => m.Teacher)
                    .ToListAsync();
                return View(marks);
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
                if (teacher == null)
                {
                    return Forbid(); // or handle accordingly if teacher not found
                }

                var marks = await _context.Mark
                    .Where(m => m.TeacherId == teacher.Id)
                    .Include(m => m.Student)
                    .Include(m => m.Subject)
                    .Include(m => m.Teacher)
                    .ToListAsync();
                return View(marks);
            }
            else if (User.IsInRole("Student"))
            {
                var student = await _context.Student.FirstOrDefaultAsync(s => s.Email == user.Email);
                if (student == null)
                {
                    return Forbid(); // or handle accordingly if teacher not found
                }

                var marks = await _context.Mark
                    .Where(m => m.StudentId == student.Id)
                    .Include(m => m.Student)
                    .Include(m => m.Subject)
                    .Include(m => m.Teacher)
                    .ToListAsync();
                return View(marks);
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Marks/Details/5
        [Authorize(Roles = "Admin,Teacher,Student")]
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

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Teacher"))
            {
                var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
                if (teacher == null || mark.TeacherId != teacher.Id)
                {
                    return Forbid();
                }
            }
            else if (User.IsInRole("Student"))
            {
                var student = await _context.Student.FirstOrDefaultAsync(s => s.Email == user.Email);
                if (student == null || mark.StudentId != student.Id)
                {
                    return Forbid();
                }
            }

            return View(mark);
        }

        // GET: Marks/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Email");  // Display email for clarity
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name");  // Display name for clarity
            return View();
        }

        // POST: Marks/Create
        [HttpPost]
        [Authorize(Roles = "Teacher")]
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
        [Authorize(Roles = "Teacher")]
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

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
            if (teacher == null || mark.TeacherId != teacher.Id)
            {
                return Forbid();
            }

            ViewData["StudentId"] = new SelectList(_context.Student, "Id", "Email", mark.StudentId);
            ViewData["SubjectId"] = new SelectList(_context.Subject, "Id", "Name", mark.SubjectId);
            return View(mark);
        }

        // POST: Marks/Edit/5
        [HttpPost]
        [Authorize(Roles = "Teacher")]
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

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
            if (teacher == null || existingMark.TeacherId != teacher.Id)
            {
                return Forbid();
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
        [Authorize(Roles = "Teacher")]
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

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
            if (teacher == null || mark.TeacherId != teacher.Id)
            {
                return Forbid();
            }

            return View(mark);
        }

        // POST: Marks/Delete/5
        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Teacher")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mark = await _context.Mark.FindAsync(id);
            if (mark != null)
            {
                var user = await _userManager.GetUserAsync(User);
                var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
                if (teacher == null || mark.TeacherId != teacher.Id)
                {
                    return Forbid();
                }

                _context.Mark.Remove(mark);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool MarkExists(int id)
        {
            return _context.Mark.Any(e => e.Id == id);
        }
    }
}
