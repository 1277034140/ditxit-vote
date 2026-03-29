namespace BoardGameScoreSystem.DTOs;

/// <summary>
/// 创建回合请求
/// </summary>
public class CreateRoundRequest
{
    public int RoomId { get; set; }
}

/// <summary>
/// 创建回合响应
/// </summary>
public class CreateRoundResponse
{
    public int Id { get; set; }
    public int RoomId { get; set; }
    public int RoundNumber { get; set; }
    public DateTime CreateTime { get; set; }
}
