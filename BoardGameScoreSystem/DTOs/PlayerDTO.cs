namespace BoardGameScoreSystem.DTOs;

/// <summary>
/// 玩家加入房间请求
/// </summary>
public class JoinRoomRequest
{
    public string RoomCode { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
}

/// <summary>
/// 玩家加入房间响应
/// </summary>
public class JoinRoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public DateTime JoinTime { get; set; }
}
