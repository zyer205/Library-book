using Microsoft.EntityFrameworkCore;
using LibrarySeatReservation.Web.Models.Entities;

namespace LibrarySeatReservation.Web.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Seat> Seats => Set<Seat>();
    public DbSet<Reservation> Reservations => Set<Reservation>();
    public DbSet<Admin> Admins => Set<Admin>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Seat>(entity =>
        {
            entity.ToTable("Seats");
            entity.HasKey(s => s.Id);
            entity.Property(s => s.Floor).HasMaxLength(20).IsRequired();
            entity.Property(s => s.Area).HasMaxLength(50).IsRequired();
            entity.Property(s => s.SeatNumber).HasMaxLength(20).IsRequired();
            entity.Property(s => s.IsEnabled).HasDefaultValue(true);
            entity.HasIndex(s => s.SeatNumber).IsUnique();
            entity.HasIndex(s => new { s.Floor, s.IsEnabled });
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.ToTable("Reservations");
            entity.HasKey(r => r.Id);
            entity.Property(r => r.UserName).HasMaxLength(50).IsRequired();
            entity.Property(r => r.StartTime).IsRequired();
            entity.Property(r => r.EndTime).IsRequired();
            entity.Property(r => r.Status).HasMaxLength(20).HasDefaultValue("待使用").IsRequired();
            entity.Property(r => r.CreatedAt).HasDefaultValueSql("GETDATE()");

            entity.HasOne(r => r.Seat)
                  .WithMany(s => s.Reservations)
                  .HasForeignKey(r => r.SeatId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(r => new { r.SeatId, r.StartTime, r.EndTime, r.Status })
                  .HasDatabaseName("IX_Reservation_SeatId_TimeRange");
            entity.HasIndex(r => new { r.UserName, r.CreatedAt })
                  .IsDescending(false, true)
                  .HasDatabaseName("IX_Reservation_UserName_CreatedAt");
            entity.HasIndex(r => r.Status)
                  .HasDatabaseName("IX_Reservation_Status");
            entity.HasIndex(r => r.CreatedAt)
                  .HasDatabaseName("IX_Reservation_CreatedAt");
        });

        modelBuilder.Entity<Admin>(entity =>
        {
            entity.ToTable("Admins");
            entity.HasKey(a => a.Id);
            entity.Property(a => a.Username).HasMaxLength(50).IsRequired();
            entity.Property(a => a.Password).HasMaxLength(100).IsRequired();
            entity.HasIndex(a => a.Username).IsUnique();
        });
    }
}
