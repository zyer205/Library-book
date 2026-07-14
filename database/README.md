# 数据库初始化说明

## 技术选型

| 项目 | 选择 |
|------|------|
| ORM | Entity Framework Core 9.x Code First |
| 数据库 | SQL Server LocalDB（随 Visual Studio 2022 自动安装） |
| 迁移方式 | 自动迁移（首次启动时执行） |

---

## 首次建库建表

本系统使用 EF Core Code First 方式，数据库和表结构在应用**首次启动时自动创建**，无需手动执行 SQL 脚本。

启动后 EF Core 会自动：
1. 检查 `__EFMigrationsHistory` 表是否存在
2. 如不存在，按迁移文件创建全部 3 张表（Seats、Reservations、Admins）
3. 执行 `SeedData.InitAsync()` 写入种子数据

```bash
# 如需手动重建数据库（在项目目录下执行）：
cd src/LibrarySeatReservation.Web
dotnet ef database update
```

---

## 表结构（3 张表）

### Seats（座位表）

| 列名 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | int | PK, 自增 | 座位 ID |
| Floor | nvarchar(20) | NOT NULL | 楼层（如"2 楼"） |
| Area | nvarchar(50) | NOT NULL | 区域（如"自习区"） |
| SeatNumber | nvarchar(20) | NOT NULL, UNIQUE | 座位编号（如"ZX-01"） |
| IsEnabled | bit | DEFAULT 1 | 是否启用 |

### Reservations（预约表）

| 列名 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | int | PK, 自增 | 预约 ID |
| SeatId | int | FK → Seats.Id, RESTRICT | 座位 ID |
| UserName | nvarchar(50) | NOT NULL | 预约用户名 |
| StartTime | datetime2 | NOT NULL | 开始时间 |
| EndTime | datetime2 | NOT NULL | 结束时间 |
| Status | nvarchar(20) | DEFAULT '待使用' | 状态：待使用/已完成/已取消 |
| CreatedAt | datetime2 | DEFAULT GETDATE() | 创建时间 |

### Admins（管理员表）

| 列名 | 类型 | 约束 | 说明 |
|------|------|------|------|
| Id | int | PK, 自增 | 管理员 ID |
| Username | nvarchar(50) | NOT NULL, UNIQUE | 用户名 |
| Password | nvarchar(100) | NOT NULL | 密码（明文，仅演示阶段） |

---

## 种子数据

程序首次启动时自动写入以下数据（如已有数据则跳过）：

### 管理员

| 用户名 | 密码 |
|--------|------|
| admin | admin123 |

### 座位（20 个）

| 楼层 | 区域 | 座位编号 | 数量 |
|------|------|---------|------|
| 2 楼 | 自习区 | ZX-01 ~ ZX-05 | 5 |
| 2 楼 | A 区 | A-01 ~ A-05 | 5 |
| 3 楼 | 期刊阅览区 | QK-01 ~ QK-05 | 5 |
| 3 楼 | B 区 | B-01 ~ B-05 | 5 |

所有座位初始状态均为 **启用**（IsEnabled = 1）。

### 体验账号

体验账号不存入数据库，由前端页面 `Views/Home/Index.cshtml` 硬编码 3 张用户卡片：

| 姓名 | 角色 |
|------|------|
| 学生 A | 普通学生 |
| 学生 B | 普通学生 |
| 学生 C | 普通学生 |

---

## 代码位置

| 文件 | 说明 |
|------|------|
| `Models/Entities/Seat.cs` | 座位实体类 |
| `Models/Entities/Reservation.cs` | 预约实体类 |
| `Models/Entities/Admin.cs` | 管理员实体类 |
| `Models/AppDbContext.cs` | EF Core DbContext + Fluent API |
| `Data/SeedData.cs` | 种子数据初始化逻辑 |
| `Migrations/` | EF Core 迁移文件 |

---

## 索引说明

| 表 | 索引列 | 用途 |
|----|--------|------|
| Seats | SeatNumber (UNIQUE) | 防止重复编号 |
| Seats | Floor, IsEnabled | 楼层筛选 + 启用状态过滤 |
| Reservations | SeatId, StartTime, EndTime, Status | 冲突检测查询优化 |
| Reservations | UserName, CreatedAt DESC | 我的预约按时间倒序 |
| Reservations | Status | 状态筛选 |
| Reservations | CreatedAt | 今日预约计数 |

---

> 详细数据库设计见 `docs/08-数据库设计.md`。
