using Microsoft.EntityFrameworkCore;
using rest_api_dotnet_template.Models;

namespace rest_api_dotnet_template.Data
{
    public class ApiDbContext : DbContext
    {
        public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
          
            modelBuilder.ApplyConfiguration(new UserConfiguration());
        }

        public DbSet<Usuario> Usuarios { get; set; }
    }
}