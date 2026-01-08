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

        public DbSet<CategoryVM> CategoryVM { get; set; }

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

            modelBuilder.Entity<AppUser>()
    .HasOne(u => u.Station)
    .WithMany()
    .HasForeignKey(u => u.StationId);


            modelBuilder.Entity<Station>()
    .HasKey(s => s.Id);   // PK

            modelBuilder.Entity<Station>()
                .HasIndex(s => s.StationCode)
                .IsUnique();

            modelBuilder.Entity<ModelStationMap>()
                .HasOne(ms => ms.Station)
                .WithMany()
                .HasForeignKey(ms => ms.StationCode)
                .HasPrincipalKey(s => s.StationCode);




            modelBuilder.Entity<ModelStage>()
                .HasKey(ms => new { ms.ModelId, ms.StageId });

            modelBuilder.Entity<CategoryVM>().HasNoKey();


            modelBuilder.Entity<StationVM>().HasNoKey();


        }
    }
}