using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TaskOne.Models;

namespace TaskOne.Data
{
    public class OrganizationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public OrganizationDbContext(DbContextOptions<OrganizationDbContext> options, IConfiguration configuration) : base(options)
        {
            _configuration = configuration;
        }

        public DbSet<Organization> Organizations { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                optionsBuilder.UseNpgsql(connectionString, o => o.CommandTimeout(180));
            }
        }
    }
}
