# BoardGameScoreSystem - 桌游积分系统

## 项目介绍

这是一个基于 ASP.NET Core 8.0 + Entity Framework Core + SQLite 的桌游积分系统 WebAPI 项目。

## 技术栈

- .NET 8.0
- ASP.NET Core WebAPI
- Entity Framework Core 8.0
- SQLite 数据库
- Code First 迁移
- Swagger API 文档

## 项目结构

```
BoardGameScoreSystem/
├── Controllers/          # API 控制器
│   ├── RoomController.cs
│   ├── RoundController.cs
│   └── ScoreController.cs
├── Data/                 # 数据库上下文
│   └── GameDbContext.cs
├── DTOs/                 # 数据传输对象
│   └── DTOs.cs
├── Entities/             # 实体类
│   ├── Room.cs
│   ├── Player.cs
│   ├── Round.cs
│   └── Score.cs
├── Services/             # 业务逻辑层
│   └── GameServices.cs
├── appsettings.json      # 配置文件
├── appsettings.Development.json
├── Program.cs            # 入口程序
└── BoardGameScoreSystem.csproj
```

## 数据库设计

### Room (房间表)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| Name | string | 房间名称 |
| Code | string | 房间邀请码(6位) |
| CreateTime | DateTime | 创建时间 |

### Player (玩家表)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| RoomId | int | 外键-房间ID |
| Name | string | 玩家名称 |
| JoinTime | DateTime | 加入时间 |

### Round (回合表)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| RoomId | int | 外键-房间ID |
| RoundNumber | int | 回合号 |
| CreateTime | DateTime | 创建时间 |

### Score (积分表)
| 字段 | 类型 | 说明 |
|------|------|------|
| Id | int | 主键 |
| RoundId | int | 外键-回合ID |
| PlayerId | int | 外键-玩家ID |
| ScoreValue | int | 积分值 |

## API 接口说明

### 1. 创建房间

**请求**
```
POST /api/room/create
Content-Type: application/json

{
  "name": "三国杀房间"
}
```

**响应**
```json
{
  "success": true,
  "message": "房间创建成功",
  "data": {
    "id": 1,
    "name": "三国杀房间",
    "code": "ABC123",
    "createTime": "2024-01-01T10:00:00"
  }
}
```

---

### 2. 玩家加入房间

**请求**
```
POST /api/room/join
Content-Type: application/json

{
  "code": "ABC123",
  "playerName": "玩家张三"
}
```

**响应**
```json
{
  "success": true,
  "message": "加入房间成功",
  "data": {
    "id": 1,
    "name": "玩家张三",
    "roomId": 1,
    "joinTime": "2024-01-01T10:05:00"
  }
}
```

---

### 3. 创建回合

**请求**
```
POST /api/round/create
Content-Type: application/json

{
  "roomId": 1
}
```

**响应**
```json
{
  "success": true,
  "message": "回合创建成功",
  "data": {
    "id": 1,
    "roomId": 1,
    "roundNumber": 1,
    "createTime": "2024-01-01T10:10:00"
  }
}
```

---

### 4. 提交积分

**请求**
```
POST /api/score/add
Content-Type: application/json

{
  "roundId": 1,
  "playerId": 1,
  "scoreValue": 100
}
```

**响应**
```json
{
  "success": true,
  "message": "积分提交成功",
  "data": {
    "id": 1,
    "roundId": 1,
    "playerId": 1,
    "scoreValue": 100
  }
}
```

---

### 5. 获取房间排名

**请求**
```
GET /api/room/rank/1
```

**响应**
```json
{
  "success": true,
  "message": "操作成功",
  "data": {
    "roomId": 1,
    "roomName": "三国杀房间",
    "rankings": [
      {
        "playerId": 1,
        "playerName": "玩家张三",
        "totalScore": 250,
        "rank": 1
      },
      {
        "playerId": 2,
        "playerName": "玩家李四",
        "totalScore": 180,
        "rank": 2
      }
    ]
  }
}
```

## 完整使用流程示例

### 步骤1: 创建房间
```bash
curl -X POST http://localhost:5000/api/room/create \
  -H "Content-Type: application/json" \
  -d '{"name":"桌游房间"}'
```

返回:
```json
{
  "success": true,
  "message": "房间创建成功",
  "data": {
    "id": 1,
    "name": "桌游房间",
    "code": "XYZ789",
    "createTime": "2024-01-01T12:00:00"
  }
}
```

### 步骤2: 玩家加入房间
```bash
curl -X POST http://localhost:5000/api/room/join \
  -H "Content-Type: application/json" \
  -d '{"code":"XYZ789","playerName":"小明"}'
```

```bash
curl -X POST http://localhost:5000/api/room/join \
  -H "Content-Type: application/json" \
  -d '{"code":"XYZ789","playerName":"小红"}'
```

### 步骤3: 创建回合
```bash
curl -X POST http://localhost:5000/api/round/create \
  -H "Content-Type: application/json" \
  -d '{"roomId":1}'
```

### 步骤4: 提交积分
```bash
curl -X POST http://localhost:5000/api/score/add \
  -H "Content-Type: application/json" \
  -d '{"roundId":1,"playerId":1,"scoreValue":100}'
```

```bash
curl -X POST http://localhost:5000/api/score/add \
  -H "Content-Type: application/json" \
  -d '{"roundId":1,"playerId":2,"scoreValue":80}'
```

### 步骤5: 查看排名
```bash
curl -X GET http://localhost:5000/api/room/rank/1
```

## 运行项目

### 1. 还原依赖
```bash
cd BoardGameScoreSystem
dotnet restore
```

### 2. 运行项目
```bash
dotnet run
```

### 3. 访问 Swagger 文档
```
http://localhost:5000/swagger
```

## 注意事项

1. 数据库会在首次运行时自动创建 (`BoardGameScore.db`)
2. 房间邀请码为6位随机字母数字组合
3. 积分按总分降序排列计算排名
4. 所有 API 都返回统一的 JSON 响应格式

## 许可证

MIT License
