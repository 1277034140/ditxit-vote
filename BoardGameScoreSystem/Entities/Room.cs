using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardGameScoreSystem.Entities;

/// <summary>
/// 房间实体
/// </summary>
[Table("Rooms")]
public class Room
{
    [Key]
    public int Id { get; set; }

    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(20)]
    public string Code { get; set; } = string.Empty;

    public DateTime CreateTime { get; set; } = DateTime.Now;

    // 导航属性
    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
    public virtual ICollection<Round> Rounds { get; set; } = new List<Round>();
}
