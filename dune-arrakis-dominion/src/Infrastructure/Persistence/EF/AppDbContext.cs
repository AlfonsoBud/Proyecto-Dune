using Microsoft.EntityFrameworkCore;
using Infrastructure.Persistence.EF.Entities;

namespace Infrastructure.Persistence.EF
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<GameEntity> Games { get; set; }
        public DbSet<EnclaveEntity> Enclaves { get; set; }
        public DbSet<InstallationEntity> Installations { get; set; }
        public DbSet<CreatureEntity> Creatures { get; set; }
        public DbSet<GameEventEntity> GameEvents { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GameEntity>().HasKey(g => g.Id);
            modelBuilder.Entity<EnclaveEntity>().HasKey(e => e.Id);
            modelBuilder.Entity<InstallationEntity>().HasKey(i => i.Id);
            modelBuilder.Entity<CreatureEntity>().HasKey(c => c.Id);
            modelBuilder.Entity<GameEventEntity>().HasKey(ev => ev.Id);

            modelBuilder.Entity<GameEntity>()
                .HasMany(g => g.Enclaves)
                .WithOne(e => e.Game)
                .HasForeignKey(e => e.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EnclaveEntity>()
                .HasMany(e => e.Installations)
                .WithOne(i => i.Enclave)
                .HasForeignKey(i => i.EnclaveId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<InstallationEntity>()
                .HasMany(i => i.Creatures)
                .WithOne(c => c.Installation)
                .HasForeignKey(c => c.InstallationId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<GameEntity>()
                .HasMany(g => g.Events)
                .WithOne(ev => ev.Game)
                .HasForeignKey(ev => ev.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}