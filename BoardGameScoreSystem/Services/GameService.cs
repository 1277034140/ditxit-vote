using Microsoft.EntityFrameworkCore;
using BoardGameScoreSystem.Data;
using BoardGameScoreSystem.DTOs;
using BoardGameScoreSystem.Models;

namespace BoardGameScoreSystem.Services;

/// <summary>
/// 游戏服务实现
/// </summary>
public class GameService : IGameService
{
    private readonly AppDbContext _context;
    private static readonly Random _random = new();

    public GameService(AppDbContext context)
    {
        _context = context;
    }

    #region 房间相关

    /// <summary>
    /// 创建房间
    /// </summary>
    public async Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request)
    {
        var room = new Room
        {
            Name = request.Name,
            Code = GenerateRoomCode(),
            CreateTime = DateTime.Now
        };

        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        return new CreateRoomResponse
        {
            Id = room.Id,
            Name = room.Name,
            Code = room.Code,
            CreateTime = room.CreateTime
        };
    }

    /// <summary>
    /// 玩家加入房间
    /// </summary>
    public async Task<JoinRoomResponse?> JoinRoomAsync(JoinRoomRequest request)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Code == request.RoomCode);
        if (room == null)
        {
            return null;
        }

        var player = new Player
        {
            RoomId = room.Id,
            Name = request.PlayerName,
            JoinTime = DateTime.Now
        };

        _context.Players.Add(player);
        await _context.SaveChangesAsync();

        return new JoinRoomResponse
        {
            Id = player.Id,
            Name = player.Name,
            RoomId = player.RoomId,
            RoomName = room.Name,
            JoinTime = player.JoinTime
        };
    }

    /// <summary>
    /// 获取房间排行榜
    /// </summary>
    public async Task<RoomRankResponse?> GetRoomRankAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            return null;
        }

        var rankings = await _context.Players
            .Where(p => p.RoomId == roomId)
            .Select(p => new RankItem
            {
                PlayerId = p.Id,
                PlayerName = p.Name,
                TotalScore = p.Scores.Sum(s => s.ScoreValue),
                RoundCount = p.Scores.Count
            })
            .OrderByDescending(r => r.TotalScore)
            .ToListAsync();

        // 添加排名
        int rank = 1;
        foreach (var item in rankings)
        {
            item.Rank = rank++;
        }

        return new RoomRankResponse
        {
            RoomId = room.Id,
            RoomName = room.Name,
            Rankings = rankings
        };
    }

    #endregion

    #region 回合相关

    /// <summary>
    /// 创建回合
    /// </summary>
    public async Task<CreateRoundResponse?> CreateRoundAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            return null;
        }

        // 获取当前最大回合号
        var lastRound = await _context.Rounds
            .Where(r => r.RoomId == roomId)
            .OrderByDescending(r => r.RoundNumber)
            .FirstOrDefaultAsync();

        var roundNumber = (lastRound?.RoundNumber ?? 0) + 1;

        var round = new Round
        {
            RoomId = roomId,
            RoundNumber = roundNumber,
            CreateTime = DateTime.Now
        };

        _context.Rounds.Add(round);
        await _context.SaveChangesAsync();

        return new CreateRoundResponse
        {
            Id = round.Id,
            RoomId = round.RoomId,
            RoundNumber = round.RoundNumber,
            CreateTime = round.CreateTime
        };
    }

    #endregion

    #region 积分相关

    /// <summary>
    /// 提交积分
    /// </summary>
    public async Task<AddScoreResponse?> AddScoreAsync(AddScoreRequest request)
    {
        // 验证回合是否存在
        var round = await _context.Rounds
            .Include(r => r.Room)
            .FirstOrDefaultAsync(r => r.Id == request.RoundId);
        if (round == null)
        {
            return null;
        }

        // 验证玩家是否属于该房间
        var player = await _context.Players
            .FirstOrDefaultAsync(p => p.Id == request.PlayerId && p.RoomId == round.RoomId);
        if (player == null)
        {
            return null;
        }

        // 检查是否已经提交过该回合的积分
        var existingScore = await _context.Scores
            .FirstOrDefaultAsync(s => s.RoundId == request.RoundId && s.PlayerId == request.PlayerId);

        if (existingScore != null)
        {
            // 更新积分
            existingScore.ScoreValue = request.ScoreValue;
        }
        else
        {
            // 创建新积分
            var score = new Score
            {
                RoundId = request.RoundId,
                PlayerId = request.PlayerId,
                ScoreValue = request.ScoreValue
            };
            _context.Scores.Add(score);
        }

        await _context.SaveChangesAsync();

        return new AddScoreResponse
        {
            Id = existingScore?.Id ?? 0,
            RoundId = request.RoundId,
            PlayerId = request.PlayerId,
            PlayerName = player.Name,
            ScoreValue = request.ScoreValue
        };
    }

    #endregion

    #region 辅助方法

    /// <summary>
    /// 生成房间邀请码
    /// </summary>
    private static string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var code = new char[6];
        for (int i = 0; i < 6; i++)
        {
            code[i] = chars[_random.Next(chars.Length)];
        }
        return new string(code);
    }

    #endregion
}
