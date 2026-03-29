using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameScoreSystem.Entities;

/// <summary>
/// 积分实体
/// </summary>
[Table("Scores")]
public class Score
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int RoundId { get; set; }

    [Required]
    public int PlayerId { get; set; }

    [Required]
    public int ScoreValue { get; set; }

    // 导航属性
    [ForeignKey("RoundId")]
    public virtual Round? Round { get; set; }

    [ForeignKey("PlayerId")]
    public virtual Player? Player { get; set; }
}
