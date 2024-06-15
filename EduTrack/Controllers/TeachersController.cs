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
using static System.Formats.Asn1.AsnWriter;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace EduTrack.Controllers
{
    public class TeachersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public TeachersController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Teachers
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teacher.ToListAsync());
        }

        // GET: Teachers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        // GET: Teachers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Teachers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Address,Email,Password")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                // Creează un nou utilizator
                var user = new IdentityUser
                {
                    UserName = teacher.Email,
                    Email = teacher.Email
                };

                var result = await _userManager.CreateAsync(user, teacher.Password);
                if (result.Succeeded)
                {
                    // Adaugă profesorul în context
                    _context.Add(teacher);
                    await _context.SaveChangesAsync();

                    // Adaugă utilizatorul la rolul Teacher
                    await _userManager.AddToRoleAsync(user, "Teacher");

                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(teacher);
        }


        // GET: Teachers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Address,Email,Password")] Teacher teacher)
        {
            if (id != teacher.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Găsește profesorul existent din baza de date
                    var existingTeacher = await _context.Teacher.FindAsync(id);
                    if (existingTeacher == null)
                    {
                        return NotFound();
                    }

                    // Găsește utilizatorul asociat
                    var user = await _userManager.FindByEmailAsync(existingTeacher.Email);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    // Actualizează utilizatorul
                    user.UserName = teacher.Email;
                    user.Email = teacher.Email;

                    var updateUserResult = await _userManager.UpdateAsync(user);
                    if (!updateUserResult.Succeeded)
                    {
                        foreach (var error in updateUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(teacher);
                    }

                    // Dacă parola a fost schimbată, actualizeaz-o
                    if (!string.IsNullOrEmpty(teacher.Password))
                    {
                        var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                        if (!removePasswordResult.Succeeded)
                        {
                            foreach (var error in removePasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(teacher);
                        }

                        var addPasswordResult = await _userManager.AddPasswordAsync(user, teacher.Password);
                        if (!addPasswordResult.Succeeded)
                        {
                            foreach (var error in addPasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(teacher);
                        }
                    }

                    // Actualizează proprietățile profesorului
                    existingTeacher.FirstName = teacher.FirstName;
                    existingTeacher.LastName = teacher.LastName;
                    existingTeacher.Address = teacher.Address;
                    existingTeacher.Email = teacher.Email;

                    _context.Update(existingTeacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.Id))
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
            return View(teacher);
        }

        // GET: Teachers/AssignSubjects/5
        public async Task<IActionResult> AssignSubjects(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            var allSubjects = await _context.Subject.ToListAsync();
            var assignedSubjects = teacher.Subjects.Select(s => s.Id).ToList();

            var viewModel = new AssignSubjectsViewModel
            {
                TeacherId = teacher.Id,
                TeacherName = $"{teacher.FirstName} {teacher.LastName}",
                Subjects = allSubjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = assignedSubjects.Contains(s.Id)
                }).ToList()
            };

            return View(viewModel);
        }

        // POST: Teachers/AssignSubjects/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignSubjects(int id, AssignSubjectsViewModel model)
        {
            var teacher = await _context.Teacher
                .Include(t => t.Subjects)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            var selectedSubjectIds = model.SubjectIds;
            var selectedSubjects = await _context.Subject.Where(s => selectedSubjectIds.Contains(s.Id)).ToListAsync();

            teacher.Subjects.Clear();
            teacher.Subjects.AddRange(selectedSubjects);

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Teachers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher != null)
            {
                // Găsește utilizatorul asociat folosind email-ul
                var user = await _userManager.FindByEmailAsync(teacher.Email);
                if (user != null)
                {
                    // Șterge utilizatorul
                    var deleteUserResult = await _userManager.DeleteAsync(user);
                    if (!deleteUserResult.Succeeded)
                    {
                        foreach (var error in deleteUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(teacher); // Sau redirecționează către o altă acțiune, dacă este necesar
                    }
                }

                // Șterge profesorul
                _context.Teacher.Remove(teacher);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }


        private bool TeacherExists(int id)
        {
            return _context.Teacher.Any(e => e.Id == id);
        }
    }
}
