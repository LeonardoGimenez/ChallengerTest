using ChallengerTest.Models;
using Microsoft.EntityFrameworkCore;

namespace ChallengerTest.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Permission> Permissions { get; set; }
    }

}