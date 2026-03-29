using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameScoreSystem.Models;

/// <summary>
/// 玩家
/// </summary>
public class Player
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public DateTime JoinTime { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("RoomId")]
    public virtual Room? Room { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
