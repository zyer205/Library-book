# 图书馆座位预约系统

> 一个面向高校图书馆的轻量座位预约 Web 系统。
> 学生可以在线查看座位余量、预约座位、取消预约；管理员可以管理座位、查看和处理预约记录。

---

## 技术栈

| 层 | 选型 | 版本要求 |
|----|------|---------|
| 后端框架 | ASP.NET Core MVC | .NET 8 |
| 视图引擎 | Razor | .NET 8 内建 |
| ORM | Entity Framework Core | 8.x |
| 数据库 | SQL Server LocalDB | 随 VS2022 安装 |
| 前端样式 | Bootstrap 5 | 5.3.2 CDN |
| 前端交互 | 原生 JavaScript | — |
| IDE | Visual Studio 2022 | 17.8+ |

---

## 目录结构

### 当前已存在（设计阶段产物）

```
LibrarySeatSystem/
├── docs/                              # 设计文档（1~10 系列）
│   ├── 01-项目立项单.md               # 项目立项与目标
│   ├── 02-需求分析与MVP确认.md         # 需求分析与 MVP 范围确认
│   ├── 03-PRD-Lite.md                 # 轻量 PRD
│   ├── 04-页面树与业务流程.md          # 页面关系与流程
│   ├── 05-页面卡与UI规范.md            # UI 规范与页面卡
│   ├── 06-静态原型与原型评审.md        # 静态原型与评审结果
│   ├── 07-系统设计说明.md              # 系统分层设计（Controller / Service / DbContext / View）
│   ├── 08-数据库设计.md                # 数据库表结构与索引
│   ├── 09-关键链路详细设计.md          # 主链路伪代码与边界
│   ├── 10-开发准备与Sprint0.md         # Sprint 0 规划与开发准备（当前）
│   └── 项目任务板与迭代记录.md         # 任务板与迭代跟踪（当前）
├── prototype/                          # 静态原型 HTML
│   └── static-v1/                      #   原型 v1 文件
└── README.md                           # 本文件
```

### 后续计划 / 待生成（Sprint 0 产物）

```
LibrarySeatSystem/
├── LibrarySeatSystem.sln               # 解决方案文件
├── LibrarySeatSystem/                  # Web 项目目录
│   ├── Program.cs                      # 应用入口
│   ├── appsettings.json                # 连接字符串与配置
│   ├── Controllers/                    # 控制器层
│   │   ├── HomeController.cs
│   │   ├── SeatsController.cs
│   │   ├── ReservationController.cs
│   │   ├── AccountController.cs
│   │   └── Admin/
│   │       ├── LoginController.cs
│   │       ├── ReservationsController.cs
│   │       └── SeatsController.cs
│   ├── Models/                         # 数据层
│   │   ├── Entities/                   #   实体类
│   │   │   ├── Seat.cs
│   │   │   ├── Reservation.cs
│   │   │   └── Admin.cs
│   │   ├── ViewModels/                 #   视图模型
│   │   │   ├── HomeIndexViewModel.cs
│   │   │   ├── SeatsIndexViewModel.cs
│   │   │   ├── SeatDetailViewModel.cs
│   │   │   ├── ReservationCreateViewModel.cs
│   │   │   ├── MyReservationsViewModel.cs
│   │   │   ├── LoginViewModel.cs
│   │   │   ├── AdminReservationsViewModel.cs
│   │   │   └── AdminSeatsViewModel.cs
│   │   └── AppDbContext.cs
│   ├── Services/                       # 业务逻辑层
│   │   ├── ISeatService.cs
│   │   ├── SeatService.cs
│   │   ├── IReservationService.cs
│   │   └── ReservationService.cs
│   ├── Views/                          # Razor 视图
│   │   ├── Shared/_Layout.cshtml
│   │   ├── Home/Index.cshtml
│   │   ├── Seats/Index.cshtml
│   │   ├── Seats/Detail.cshtml
│   │   ├── Reservation/Create.cshtml
│   │   ├── Reservation/My.cshtml
│   │   └── Admin/
│   │       ├── Shared/_Layout.cshtml
│   │       ├── Login.cshtml
│   │       ├── Reservations.cshtml
│   │       ├── Seats.cshtml
│   │       └── Statistics.cshtml (P2)
│   ├── wwwroot/
│   │   └── css/site.css
│   └── Data/
│       └── SeedData.cs
```

---

## 运行前提

在本地运行本系统需要先安装以下软件：

| 软件 | 版本要求 | 用途 |
|------|---------|------|
| Visual Studio 2022 | 17.8+（社区版即可） | 开发 IDE，自带项目模板 |
| .NET SDK | 8.0 | ASP.NET Core 运行时 |
| SQL Server LocalDB | 随 VS2022 自动安装 | 本地数据库 |
| Git | 最新版 | 版本控制 |

**验证安装命令：**

```bash
dotnet --version               # 应显示 8.x
dotnet --list-sdks              # 确认 8.0 SDK 存在
```

---

## 当前阶段

**开发准备与 Sprint 0 规划阶段** — 已完成全部设计文档输出（doc 01~10），待执行 Sprint 0（项目骨架搭建）。

> 具体任务分配见 `docs/项目任务板与迭代记录.md`，Sprint 0 详细规划见 `docs/10-开发准备与Sprint0.md`。

---

## 已实现范围

<!-- 后续每完成一个 Sprint 在此追加范围记录 -->

| Sprint | 范围 | 完成日期 |
|--------|------|---------|
| Sprint 0 | 项目骨架搭建、数据库初始化、Seed Data | — |
| Sprint 1 | 用户端核心功能（身份切换、座位列表、预约提交、我的预约、取消） | — |
| Sprint 2 | 管理端功能（登录、预约管理、座位管理） | — |
| Sprint 3 | 联调、缺陷修复、静态原型审计修复项落实 | — |
| Sprint 2 | 管理端功能（登录、预约管理、座位管理、**统计页**） | — |
| Sprint 3 | 联调、缺陷修复、静态原型审计修复项落实 | — |
| Sprint 4（缓冲） | 样式调整、部署文档 | — |

---

## 数据库初始化方式

<!-- Sprint 0 执行后更新具体命令 -->

```bash
# 在程序启动时自动执行迁移 + Seed Data
Update-Database                   # 或
dotnet ef database update
```

Seed Data 包含：
- 管理员 1 个：`admin` / `admin123`
- 座位 15~20 个，覆盖 2~3 个楼层
- 体验账号（前端硬编码）：学生 A、学生 B、学生 C

---

## 演示账号

| 身份 | 用户名 | 密码 | 说明 |
|------|--------|------|------|
| 学生 A | 首页点击"学生 A"卡片 | 无密码 | 体验账号，前端硬编码 |
| 学生 B | 首页点击"学生 B"卡片 | 无密码 | 体验账号，前端硬编码 |
| 学生 C | 首页点击"学生 C"卡片 | 无密码 | 体验账号，前端硬编码 |
| 管理员 | `admin` | `admin123` | 写入 `Admins` 表 |

---

## 已知限制

<!-- 开发过程中持续更新 -->

- 密码明文存储（仅课堂演示阶段，生产环境必须用 bcrypt）
- 预约冲突检测存在竞态条件（低并发环境下可接受）
- 座位占闲状态不实时推送，刷新页面后重新计算
- 管理端手机端仅做最低适配（`overflow-x: auto` 横向滚动）

---

## 许可证

课堂项目，仅用于教学演示。
