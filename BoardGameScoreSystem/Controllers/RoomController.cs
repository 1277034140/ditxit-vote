using BoardGameScoreSystem.DTOs;
using BoardGameScoreSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameScoreSystem.Controllers;

/// <summary>
/// 房间控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoomController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="request">创建房间请求</param>
    /// <returns>房间信息</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateRoom([FromBody] CreateRoomRequest request)
    {
        try
        {
            var result = await _roomService.CreateRoomAsync(request);
            return Ok(ApiResponse<CreateRoomResponse>.SuccessResult(result, "房间创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CreateRoomResponse>.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// 玩家加入房间
    /// </summary>
    /// <param name="request">加入房间请求</param>
    /// <returns>玩家信息</returns>
    [HttpPost("join")]
    public async Task<IActionResult> JoinRoom([FromBody] JoinRoomRequest request)
    {
        try
        {
            var result = await _roomService.JoinRoomAsync(request);
            return Ok(ApiResponse<JoinRoomResponse>.SuccessResult(result, "加入房间成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<JoinRoomResponse>.FailResult(ex.Message));
        }
    }

    /// <summary>
    /// 获取房间排名
    /// </summary>
    /// <param name="roomId">房间ID</param>
    /// <returns>房间排名信息</returns>
    [HttpGet("rank/{roomId}")]
    public async Task<IActionResult> GetRoomRank(int roomId)
    {
        try
        {
            var result = await _roomService.GetRoomRankAsync(roomId);
            return Ok(ApiResponse<RoomRankResponse>.SuccessResult(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<RoomRankResponse>.FailResult(ex.Message));
        }
    }
}
