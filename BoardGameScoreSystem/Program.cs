using BoardGameScoreSystem.Data;
using BoardGameScoreSystem.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 添加服务到容器
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 配置 SQLite 数据库
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<GameDbContext>(options =>
    options.UseSqlite(connectionString));

// 注册服务
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IRoundService, RoundService>();
builder.Services.AddScoped<IScoreService, ScoreService>();

var app = builder.Build();

// 配置 HTTP 请求管道
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();

// 数据库初始化 - 确保数据库创建并应用迁移
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<GameDbContext>();
    
    // 确保数据库创建
    dbContext.Database.EnsureCreated();
    
    // 输出数据库路径
    var dbPath = dbContext.Database.GetDbConnection().DataSource;
    Console.WriteLine($"数据库已创建: {dbPath}");
}

app.Run();
