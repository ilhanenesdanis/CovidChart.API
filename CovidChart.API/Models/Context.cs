using Microsoft.EntityFrameworkCore;

namespace CovidChart.API.Models
{
    public class Context:DbContext
    {
        public Context(DbContextOptions<Context> options):base(options)
        {
        }
        public DbSet<Covid> Covids { get; set; }
    }
}
