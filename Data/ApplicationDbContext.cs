using LinkwellProductionSystem.Models;
using LinkwellProductionSystem.ViewModels;
using Microsoft.EntityFrameworkCore;


namespace LinkwellProductionSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Model> Models { get; set; } = null!;

        public DbSet<ModelStationMap> ModelStationMap { get; set; } = null!;
        public DbSet<Stage> Stages { get; set; } = null!;
        public DbSet<ModelStage> ModelStages { get; set; } = null!;
        public DbSet<WorkInstruction> WorkInstructions { get; set; } = null!;
        public DbSet<Station> Stations { get; set; } = null!;
        public DbSet<AppUser> AppUsers { get; set; } = null!;
        public DbSet<DailyProduction> DailyProductions { get; set; } = null!;
        public DbSet<HourlyProduction> HourlyProductions { get; set; } = null!;

        public DbSet<StationVM> StationVMs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var connectionString = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .AddEnvironmentVariables()
                    .Build()
                    .GetConnectionString("Con");

                optionsBuilder.UseSqlServer(connectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ModelStage>()
                .HasKey(ms => new { ms.ModelId, ms.StageId });

            modelBuilder.Entity<StationVM>().HasNoKey();

            // Seed Admin + Incharge users with BCrypt hashed passwords
            modelBuilder.Entity<AppUser>().HasData(
                new AppUser
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$Wj8i7g5v5j5m9k3n7p2q8e9r0t1y2u3i4o5p6a7s8d9f0g1h2j3k4l", // password: admin123
                    FullName = "Administrator",
                    Role = "Admin"
                },
                new AppUser
                {
                    Id = 2,
                    Username = "assy01",
                    PasswordHash = "$2a$11$ZmNlMjQwOWRkMjE0YjYxOOU5ZjQ5ZmU5NjY0NjY0NjY0NjY0NjY0Ng==", // password: pass123
                    FullName = "John - ASSY01",
                    StationId = 1,
                    Role = "Incharge"
                }
            );

            // Seed some stations
            modelBuilder.Entity<Station>().HasData(
                new Station { Id = 1, StationCode = "ASSY01", StationName = "Assembly Line 01", Location = "Plant A" },
                new Station { Id = 2, StationCode = "QC01", StationName = "Quality Check 01", Location = "Plant A" }
            );
        }
    }
}