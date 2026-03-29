using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameScoreSystem.Entities;

/// <summary>
/// 玩家实体
/// </summary>
[Table("Players")]
public class Player
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    public DateTime JoinTime { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("RoomId")]
    public virtual Room? Room { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
