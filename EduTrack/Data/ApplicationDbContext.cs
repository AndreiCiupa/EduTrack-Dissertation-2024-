using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using EduTrack.Models;

namespace EduTrack.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<EduTrack.Models.Subject> Subject { get; set; } = default!;
        public DbSet<EduTrack.Models.Teacher> Teacher { get; set; } = default!;
        public DbSet<EduTrack.Models.Student> Student { get; set; } = default!;
        public DbSet<EduTrack.Models.Mark> Mark { get; set; } = default!;
    }
}
