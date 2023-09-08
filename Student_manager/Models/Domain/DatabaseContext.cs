using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Student_manager.Models.Domain
{
    public class DatabaseContext:IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) {

        }

        public DbSet<Class>?Classes { get; set; }
        public DbSet<TokenInfo> TokenInfo { get; set; }
        //public DbSet<Subject> Subjects { get; set; }
        //public DbSet<SubjectInfo> SubjectInfos { get; set; }

    }
}
