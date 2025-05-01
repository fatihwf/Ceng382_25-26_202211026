using Microsoft.EntityFrameworkCore;
using App.Models;
namespace App.Data
{
 public class SchoolDbContext : DbContext
 {
    public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
    : base(options)
 {
 }
 public DbSet<Class> Classes { get; set; }
 }
}
