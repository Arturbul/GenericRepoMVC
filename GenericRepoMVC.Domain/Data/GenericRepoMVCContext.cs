using GenericRepoMVC.Domain.Models;
using Microsoft.EntityFrameworkCore;
namespace GenericRepoMVC.Domain.Data
{
    public class GenericRepoMVCContext : DbContext
    {
        public GenericRepoMVCContext(DbContextOptions<GenericRepoMVCContext> options) : base(options) { }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Person> Persons { get; set; }
    }
}