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

/// <summary>
/// 加入房间请求
/// </summary>
public class JoinRoomRequest
{
    public string Code { get; set; } = string.Empty;
    public string PlayerName { get; set; } = string.Empty;
}

/// <summary>
/// 加入房间响应
/// </summary>
public class JoinRoomResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public DateTime JoinTime { get; set; }
}

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
    public int ScoreValue { get; set; }
}

/// <summary>
/// 排名项
/// </summary>
public class RankItem
{
    public int PlayerId { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int TotalScore { get; set; }
    public int Rank { get; set; }
}

/// <summary>
/// 房间排名响应
/// </summary>
public class RoomRankResponse
{
    public int RoomId { get; set; }
    public string RoomName { get; set; } = string.Empty;
    public List<RankItem> Rankings { get; set; } = new List<RankItem>();
}

/// <summary>
/// 通用 API 响应
/// </summary>
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public T? Data { get; set; }

    public static ApiResponse<T> SuccessResult(T data, string message = "操作成功")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static ApiResponse<T> FailResult(string message)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Data = default
        };
    }
}
