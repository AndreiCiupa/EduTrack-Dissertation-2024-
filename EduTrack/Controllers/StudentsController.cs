using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EduTrack.Data;
using EduTrack.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace EduTrack.Controllers
{
    [Authorize]
    public class StudentsController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ApplicationDbContext _context;

        public StudentsController(UserManager<IdentityUser> userManager, ApplicationDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        // GET: Students
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(await _context.Student.ToListAsync());
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacher = await _context.Teacher
                    .Include(t => t.Students)
                    .FirstOrDefaultAsync(t => t.Email == user.Email);
                //.Include(t => t.Subjects)
                //.ThenInclude(s => s.Students)
                //.FirstOrDefaultAsync(t => t.Email == user.Email);

                if (teacher == null)
                {
                    return Forbid();
                }

                // Get students associated with the logged in teacher
                //var students = await _context.Student
                //                            .Where(s => s.Teachers.Any(t => t.Email == user.Email))
                //                            .ToListAsync();
                //var students = await _context.Student
                //                            .Where(s => s.Teachers.Any(t => t.Email == user.Email))
                //                            .ToListAsync();
                var students = teacher.Students.ToList();
                return View(students);
            }
            return Forbid();
        }

        // GET: Students/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            var user = await _userManager.GetUserAsync(User);
            if (User.IsInRole("Admin"))
            {
                return View(student);
            }
            else if (User.IsInRole("Teacher"))
            {
                var teacher = await _context.Teacher.FirstOrDefaultAsync(t => t.Email == user.Email);
                if (teacher == null || !student.Teachers.Any(t => t.Id == teacher.Id))
                {
                    return Forbid();
                }
                return View(student);
            }
            return Forbid();
        }

        // GET: Students/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Students/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Age,Address,Email,Password")] Student student)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = student.Email, Email = student.Email };
                var result = await _userManager.CreateAsync(user, student.Password);
                if (result.Succeeded)
                {
                    _context.Add(student);
                    await _context.SaveChangesAsync();
                    await _userManager.AddToRoleAsync(user, "Student");
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
            return View(student);
        }

        // GET: Students/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FindAsync(id);
            if (student == null)
            {
                return NotFound();
            }
            return View(student);
        }

        // POST: Students/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FirstName,LastName,Age,Address,Email,Password")] Student student)
        {
            if (id != student.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingStudent = await _context.Student.FindAsync(id);
                    if (existingStudent == null)
                    {
                        return NotFound();
                    }

                    var user = await _userManager.FindByEmailAsync(existingStudent.Email);
                    if (user == null)
                    {
                        return NotFound();
                    }

                    user.UserName = student.Email;
                    user.Email = student.Email;

                    var updateUserResult = await _userManager.UpdateAsync(user);
                    if (!updateUserResult.Succeeded)
                    {
                        foreach (var error in updateUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(student);
                    }

                    if (!string.IsNullOrEmpty(student.Password))
                    {
                        var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                        var resetPasswordResult = await _userManager.ResetPasswordAsync(user, token, student.Password);
                        if (!resetPasswordResult.Succeeded)
                        {
                            foreach (var error in resetPasswordResult.Errors)
                            {
                                ModelState.AddModelError(string.Empty, error.Description);
                            }
                            return View(student);
                        }
                    }

                    existingStudent.FirstName = student.FirstName;
                    existingStudent.LastName = student.LastName;
                    existingStudent.Age = student.Age;
                    existingStudent.Address = student.Address;
                    existingStudent.Email = student.Email;

                    _context.Update(existingStudent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExists(student.Id))
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
            return View(student);
        }

        // GET: Students/EnrollStudents/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollStudents(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student
                .Include(s => s.Subjects)
                .ThenInclude(sub => sub.Teachers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var allSubjects = await _context.Subject.Include(s => s.Teachers).ToListAsync();
            var assignedSubjects = student.Subjects.Select(s => s.Id).ToList();
            var assignedTeachers = student.Subjects.ToDictionary(s => s.Id, s => s.Teachers.FirstOrDefault()?.Id); // Dicționar pentru a memora profesorii alocați pentru fiecare materie

            var viewModel = new EnrollStudentsViewModel
            {
                StudentId = student.Id,
                StudentName = $"{student.FirstName} {student.LastName}",
                Subjects = allSubjects.Select(s => new SelectListItem
                {
                    Value = s.Id.ToString(),
                    Text = s.Name,
                    Selected = assignedSubjects.Contains(s.Id)
                }).ToList(),
                SelectedSubjectIds = assignedSubjects,
                Teachers = new List<SelectListItem>(),
                /*SelectedTeacherId = 0*/ // Inițializăm cu 0 pentru a nu selecta niciun profesor
            };

            return View(viewModel);
        }



        // POST: Students/EnrollStudents/5
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> EnrollStudents(int id, EnrollStudentsViewModel model)
        //{
        //    var student = await _context.Student
        //        .Include(s => s.Subjects)
        //        .Include(s => s.Teachers)
        //        .FirstOrDefaultAsync(m => m.Id == id);

        //    if (student == null)
        //    {
        //        return NotFound();
        //    }

        //    var selectedSubjects = await _context.Subject
        //        .Include(s => s.Teachers)
        //        .Where(s => model.SelectedSubjectIds.Contains(s.Id))
        //        .ToListAsync();

        //    //student.Subjects.Clear();

        //    //student.Subjects.AddRange(selectedSubjects);

        //    // Selectăm profesorul pentru toate materiile selectate
        //    //var selectedTeacherId = model.SelectedTeacherId;
        //    //foreach (var subject in selectedSubjects)
        //    //{
        //    //    var teacher = subject.Teachers.FirstOrDefault(t => t.Id == selectedTeacherId);
        //    //    if (teacher != null)
        //    //    {
        //    //        subject.Teachers.Clear();
        //    //        subject.Teachers.Add(teacher);
        //    //    }
        //    //}
        //    var selectedTeacherId = model.SelectedTeacherId;
        //    foreach (var subject in selectedSubjects)
        //    {
        //        var teacher = subject.Teachers.FirstOrDefault(t => t.Id == selectedTeacherId); // Get the first selected teacher (assuming single selection)
        //        //var selectedTeacher = await _context.Teacher.FindAsync(selectedTeacherId);
        //        if (teacher != null)
        //        {
        //            subject.Teachers.Add(teacher); // Associate teacher with subject
        //            student.Teachers.Add(teacher); // Associate teacher with student
        //        }
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EnrollStudents(int id, EnrollStudentsViewModel model)
        {
            var student = await _context.Student
                .Include(s => s.Subjects)
                .ThenInclude(sub => sub.Teachers)
                .Include(s => s.Teachers)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            var selectedSubjects = await _context.Subject
                .Include(s => s.Teachers)
                .Where(s => model.SelectedSubjectIds.Contains(s.Id))
                .ToListAsync();

            // Clear existing subjects and associated teachers for the student
            //student.Subjects.Clear();
            foreach (var subject in selectedSubjects)
            {
                student.Subjects.Add(subject); // Associate selected subjects with student
            }

            // Associate selected teacher with each subject
            var selectedTeacherId = model.SelectedTeacherId;
            //foreach (var subject in selectedSubjects)
            //{
            //    var teacher = subject.Teachers.FirstOrDefault(t => t.Id == selectedTeacherId);
            //    if (teacher != null)
            //    {
            //        /*subject.Teachers.Clear();*/ // Clear existing teachers for the subject (if needed)
            //        subject.Teachers.Add(teacher); // Add selected teacher to the subject
            //    }
            //}

            // Associate selected teacher with the student directly
            var selectedTeacher = await _context.Teacher.FindAsync(selectedTeacherId);
            if (selectedTeacher != null)
            {
                /*student.Teachers.Clear();*/ // Clear existing teachers for the student (if needed)
                student.Teachers.Add(selectedTeacher); // Add selected teacher to the student
                selectedTeacher.Students.Add(student);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetTeachersForSubjects(List<int> subjectIds)
        {
            var teachers = await _context.Subject
                .Where(s => subjectIds.Contains(s.Id))
                .SelectMany(s => s.Teachers)
                .Distinct()
                .Select(t => new SelectListItem
                {
                    Value = t.Id.ToString(),
                    Text = $"{t.FirstName} {t.LastName}"
                })
                .ToListAsync();

            return Json(teachers);
        }


        // GET: Students/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Student.FirstOrDefaultAsync(m => m.Id == id);
            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: Students/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Student.FindAsync(id);
            if (student != null)
            {
                var user = await _userManager.FindByEmailAsync(student.Email);
                if (user != null)
                {
                    var deleteUserResult = await _userManager.DeleteAsync(user);
                    if (!deleteUserResult.Succeeded)
                    {
                        foreach (var error in deleteUserResult.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                        return View(student);
                    }
                }

                _context.Student.Remove(student);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool StudentExists(int id)
        {
            return _context.Student.Any(e => e.Id == id);
        }
    }
}

