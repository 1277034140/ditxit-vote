using BoardGameScoreSystem.DTOs;
using BoardGameScoreSystem.Services;
using Microsoft.AspNetCore.Mvc;

namespace BoardGameScoreSystem.Controllers;

/// <summary>
/// 积分控制器
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ScoreController : ControllerBase
{
    private readonly IScoreService _scoreService;

    public ScoreController(IScoreService scoreService)
    {
        _scoreService = scoreService;
    }

    /// <summary>
    /// 提交积分
    /// </summary>
    /// <param name="request">积分请求</param>
    /// <returns>积分信息</returns>
    [HttpPost("add")]
    public async Task<IActionResult> AddScore([FromBody] AddScoreRequest request)
    {
        try
        {
            var result = await _scoreService.AddScoreAsync(request);
            return Ok(ApiResponse<AddScoreResponse>.SuccessResult(result, "积分提交成功"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AddScoreResponse>.FailResult(ex.Message));
        }
    }
}
