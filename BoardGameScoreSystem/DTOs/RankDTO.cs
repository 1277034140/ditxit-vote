namespace BoardGameScoreSystem.DTOs;

/// <summary>
/// 排行榜项
/// </summary>
public class RankItem
{
    public int Rank { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int RoundCount { get; set; }
}

/// <summary>
/// 房间排行榜响应
/// </summary>
public class RoomRankResponse
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public List<RankItem> Rankings { get; set; } = new List<RankItem>();
}
