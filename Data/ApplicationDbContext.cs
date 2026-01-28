using LinkwellProductionSystem.Models;
using LinkwellProductionSystem.ViewModels;
using Microsoft.EntityFrameworkCore;
using LinkwellProductionSystem.Core.Entities;

namespace LinkwellProductionSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<ModelStationWorkInstruction> ModelStationWorkInstruction { get; set; } = null!;

        public DbSet<Category> Category { get; set; }

        public DbSet<WorkInstruction> WorkInstruction { get; set; } = null!;
        public DbSet<Model> Models { get; set; }
        public DbSet<ModelStationMap> ModelStationMap { get; set; } = null!;
        public DbSet<Station> Stations { get; set; } = null!;
        public DbSet<AppUser> AppUsers { get; set; } = null!;

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
            // ===============================
            // ModelStationMap (STRING BASED)
            // ===============================
            modelBuilder.Entity<ModelStationMap>()
        .HasKey(x => new { x.ModelId, x.StationId });


            // ===============================
            // AppUser → Station (VALID FK)
            // ===============================
            modelBuilder.Entity<AppUser>()
                .HasOne(u => u.Station)
                .WithMany()
                .HasForeignKey(u => u.StationId);

            // ===============================
            // Station
            // ===============================
            modelBuilder.Entity<Station>()
                .HasKey(s => s.Id);

            modelBuilder.Entity<Station>()
                .HasIndex(s => s.StationCode)
                .IsUnique();

            // ===============================
            // Other configs
            // ===============================
            modelBuilder.Entity<ModelStage>()
                .HasKey(ms => new { ms.ModelId, ms.StageId });

            modelBuilder.Entity<CategoryVM>().HasNoKey();
            modelBuilder.Entity<StationVM>().HasNoKey();

            base.OnModelCreating(modelBuilder);
        }

    }
}