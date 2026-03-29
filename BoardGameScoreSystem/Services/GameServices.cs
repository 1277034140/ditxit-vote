using BoardGameScoreSystem.Data;
using BoardGameScoreSystem.DTOs;
using BoardGameScoreSystem.Entities;
using Microsoft.EntityFrameworkCore;

namespace BoardGameScoreSystem.Services;

/// <summary>
/// 房间服务接口
/// </summary>
public interface IRoomService
{
    Task<CreateRoomResponse> CreateRoomAsync(CreateRoomRequest request);
    Task<JoinRoomResponse> JoinRoomAsync(JoinRoomRequest request);
    Task<RoomRankResponse> GetRoomRankAsync(int roomId);
}

/// <summary>
/// 回合服务接口
/// </summary>
public interface IRoundService
{
    Task<CreateRoundResponse> CreateRoundAsync(CreateRoundRequest request);
}

/// <summary>
/// 积分服务接口
/// </summary>
public interface IScoreService
{
    Task<AddScoreResponse> AddScoreAsync(AddScoreRequest request);
}

/// <summary>
/// 房间服务实现
/// </summary>
public class RoomService : IRoomService
{
    private readonly GameDbContext _context;

    public RoomService(GameDbContext context)
    {
        _context = context;
    }

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
    public async Task<JoinRoomResponse> JoinRoomAsync(JoinRoomRequest request)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Code == request.Code);
        if (room == null)
        {
            throw new Exception("房间不存在");
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
            JoinTime = player.JoinTime
        };
    }

    /// <summary>
    /// 获取房间排名
    /// </summary>
    public async Task<RoomRankResponse> GetRoomRankAsync(int roomId)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == roomId);
        if (room == null)
        {
            throw new Exception("房间不存在");
        }

        var rankings = await _context.Players
            .Where(p => p.RoomId == roomId)
            .Select(p => new RankItem
            {
                PlayerId = p.Id,
                PlayerName = p.Name,
                TotalScore = p.Scores.Sum(s => (int?)s.ScoreValue) ?? 0
            })
            .OrderByDescending(r => r.TotalScore)
            .ToListAsync();

        // 设置排名
        for (int i = 0; i < rankings.Count; i++)
        {
            rankings[i].Rank = i + 1;
        }

        return new RoomRankResponse
        {
            RoomId = roomId,
            RoomName = room.Name,
            Rankings = rankings
        };
    }

    /// <summary>
    /// 生成房间邀请码
    /// </summary>
    private string GenerateRoomCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 6)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}

/// <summary>
/// 回合服务实现
/// </summary>
public class RoundService : IRoundService
{
    private readonly GameDbContext _context;

    public RoundService(GameDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 创建回合
    /// </summary>
    public async Task<CreateRoundResponse> CreateRoundAsync(CreateRoundRequest request)
    {
        var room = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == request.RoomId);
        if (room == null)
        {
            throw new Exception("房间不存在");
        }

        // 获取当前最大回合号
        var maxRoundNumber = await _context.Rounds
            .Where(r => r.RoomId == request.RoomId)
            .MaxAsync(r => (int?)r.RoundNumber) ?? 0;

        var round = new Round
        {
            RoomId = request.RoomId,
            RoundNumber = maxRoundNumber + 1,
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
}

/// <summary>
/// 积分服务实现
/// </summary>
public class ScoreService : IScoreService
{
    private readonly GameDbContext _context;

    public ScoreService(GameDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// 添加积分
    /// </summary>
    public async Task<AddScoreResponse> AddScoreAsync(AddScoreRequest request)
    {
        var round = await _context.Rounds.FirstOrDefaultAsync(r => r.Id == request.RoundId);
        if (round == null)
        {
            throw new Exception("回合不存在");
        }

        var player = await _context.Players.FirstOrDefaultAsync(p => p.Id == request.PlayerId);
        if (player == null)
        {
            throw new Exception("玩家不存在");
        }

        // 验证玩家是否属于该房间
        if (player.RoomId != round.RoomId)
        {
            throw new Exception("玩家不属于该房间");
        }

        var score = new Score
        {
            RoundId = request.RoundId,
            PlayerId = request.PlayerId,
            ScoreValue = request.ScoreValue
        };

        _context.Scores.Add(score);
        await _context.SaveChangesAsync();

        return new AddScoreResponse
        {
            Id = score.Id,
            RoundId = score.RoundId,
            PlayerId = score.PlayerId,
            ScoreValue = score.ScoreValue
        };
    }
}
