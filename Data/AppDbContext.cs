using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Reflection.Metadata;

namespace TamboliyaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        public DbSet<SideOfDodecahedron> SideOfDodecahedrons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SideOfDodecahedron>(entity => { entity.Property(e => e.Number).IsRequired(); });
            modelBuilder.Entity<SideOfDodecahedron>(entity => { entity.Property(e => e.Color).IsRequired(); });

            #region SideOfDodecahedronSeed
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 1, Number = 1, Color = DodecahedronColor.Red });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 2, Number = 2, Color = DodecahedronColor.Blue });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 3, Number = 3, Color = DodecahedronColor.Yellow });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 4, Number = 4, Color = DodecahedronColor.Yellow });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 5, Number = 5, Color = DodecahedronColor.Green });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 6, Number = 6, Color = DodecahedronColor.Blue });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 7, Number = 7, Color = DodecahedronColor.Green });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 8, Number = 8, Color = DodecahedronColor.Red });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 9, Number = 9, Color = DodecahedronColor.Red });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 10, Number = 10, Color = DodecahedronColor.Green });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 11, Number = 11, Color = DodecahedronColor.Yellow });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 12, Number = 12, Color = DodecahedronColor.Blue });
            #endregion

        }
    }
}
