using BoardGameScoreSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameScoreSystem.Data;

/// <summary>
/// 游戏积分系统数据库上下文
/// </summary>
public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
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
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Code).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.Code).IsUnique();
        });

        // Player 配置
        modelBuilder.Entity<Player>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
            entity.HasOne(e => e.Room)
                  .WithMany(r => r.Players)
                  .HasForeignKey(e => e.RoomId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Round 配置
        modelBuilder.Entity<Round>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Room)
                  .WithMany(r => r.Rounds)
                  .HasForeignKey(e => e.RoomId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Score 配置
        modelBuilder.Entity<Score>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ScoreValue).IsRequired();
            entity.HasOne(e => e.Round)
                  .WithMany(r => r.Scores)
                  .HasForeignKey(e => e.RoundId)
                  .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(e => e.Player)
                  .WithMany(p => p.Scores)
                  .HasForeignKey(e => e.PlayerId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
