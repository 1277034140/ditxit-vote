using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameScoreSystem.Models;

/// <summary>
/// 回合
/// </summary>
public class Round
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public int RoundNumber { get; set; }

    public DateTime CreateTime { get; set; } = DateTime.Now;

    // 导航属性
    [ForeignKey("RoomId")]
    public virtual Room? Room { get; set; }

    public virtual ICollection<Score> Scores { get; set; } = new List<Score>();
}
