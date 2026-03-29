using Microsoft.EntityFrameworkCore;
using BoardGameScoreSystem.Models;

namespace BoardGameScoreSystem.Data;

/// <summary>
/// 应用程序数据库上下文
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Room> Rooms { get; set; } = null!;
    public DbSet<Player> Players { get; set; } = null!;
    public DbSet<Round> Rounds { get; set; } = null!;
    public DbSet<Score> Scores { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Room 配置
        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasIndex(r => r.Code).IsUnique();
        });

        // Player 配置
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasOne(p => p.Room)
                .WithMany(r => r.Players)
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Round 配置
        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasOne(r => r.Room)
                .WithMany(ro => ro.Rounds)
                .HasForeignKey(r => r.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(r => new { r.RoomId, r.RoundNumber }).IsUnique();
        });

        // Score 配置
        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasOne(s => s.Round)
                .WithMany(r => r.Scores)
                .HasForeignKey(s => s.RoundId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(s => s.Player)
                .WithMany(p => p.Scores)
                .HasForeignKey(s => s.PlayerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(s => new { s.RoundId, s.PlayerId }).IsUnique();
        });
    }
}
