using BoardGameScoreSystem.DTOs;
using BoardGameScoreSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameScoreSystem.Controllers;

/// <summary>
/// 回合控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class RoundController : ControllerBase
{
    private readonly IRoundService _roundService;

    public RoundController(IRoundService roundService)
    {
        _roundService = roundService;
    }

    /// <summary>
    /// 创建回合
    /// </summary>
    /// <param name="request">创建回合请求</param>
    /// <returns>回合信息</returns>
    [HttpPost("create")]
    public async Task<IActionResult> CreateRound([FromBody] CreateRoundRequest request)
    {
        try
        {
            var result = await _roundService.CreateRoundAsync(request);
            return Ok(ApiResponse<CreateRoundResponse>.SuccessResult(result, "回合创建成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<CreateRoundResponse>.FailResult(ex.Message));
        }
    }
}
