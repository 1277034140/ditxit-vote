using BoardGameScoreSystem.DTOs;

namespace BoardGameScoreSystem.Services;

/// <summary>
/// 游戏服务接口
/// </summary>
public interface IGameService
{
    // 房间相关
    Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request);
    Task<JoinRoomResponse?> JoinRoomAsync(JoinRoomRequest request);
    Task<RoomRankResponse?> GetRoomRankAsync(int roomId);

    // 回合相关
    Task<CreateRoundResponse?> CreateRoundAsync(int roomId);

    // 积分相关
    Task<AddScoreResponse?> AddScoreAsync(AddScoreRequest request);
}
