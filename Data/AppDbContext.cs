using Microsoft.EntityFrameworkCore;

namespace TamboliyaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<SideOfDodecahedron> SideOfDodecahedrons { get; set; } = null!;
        public DbSet<ActualPositionOnTheMap> ActualPositionsOnTheMap { get; set; } = null!;
        public DbSet<Game> Games { get; set; } = null!;
        public DbSet<InitialGameData> InitialGamesData { get; set; } = null!;
        public DbSet<GameLog> GameLogs { get; set; } = null!;



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SideOfDodecahedron>(entity => { entity.Property(e => e.Number).IsRequired(); });
            modelBuilder.Entity<SideOfDodecahedron>(entity => { entity.Property(e => e.Color).IsRequired(); });
            modelBuilder.Entity<Game>()
                .HasOne(x => x.InitialGameData)
                .WithOne(x => x.Game);
            modelBuilder.Entity<Game>()
                .HasOne(x => x.ActualPosition)
                .WithOne(x => x.Game);


            #region SideOfDodecahedronSeed
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 1, Number = 1, Color = Color.Red });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 2, Number = 2, Color = Color.Blue });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 3, Number = 3, Color = Color.Yellow });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 4, Number = 4, Color = Color.Yellow });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 5, Number = 5, Color = Color.Green });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 6, Number = 6, Color = Color.Blue });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 7, Number = 7, Color = Color.Green });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 8, Number = 8, Color = Color.Red });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 9, Number = 9, Color = Color.Red });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 10, Number = 10, Color = Color.Green });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 11, Number = 11, Color = Color.Yellow });
            modelBuilder.Entity<SideOfDodecahedron>().HasData(new SideOfDodecahedron { Id = 12, Number = 12, Color = Color.Blue });
            #endregion

        }
    }
}
