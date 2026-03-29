namespace BoardGameScoreSystem.DTOs;

/// <summary>
/// 提交积分请求
/// </summary>
public class AddScoreRequest
{
    public int RoundId { get; set; }
    public int PlayerId { get; set; }
    public int ScoreValue { get; set; }
}

/// <summary>
/// 提交积分响应
/// </summary>
public class AddScoreResponse
{
    public int Id { get; set; }
    public int RoundId { get; set; }
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int ScoreValue { get; set; }
}
