namespace BoardGameScoreSystem.DTOs;

/// <summary>
/// 创建房间请求
/// </summary>
public class CreateRoomRequest
{
    public string Name { get; set; } = string.Empty;
}

/// <summary>
/// 创建房间响应
/// </summary>
public class CreateRoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public DateTime CreateTime { get; set; }
}
