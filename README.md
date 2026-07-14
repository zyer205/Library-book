# 图书馆座位预约系统

> 一个面向高校图书馆的轻量座位预约 Web 系统。
> 学生可以在线查看座位余量、预约座位、取消预约；管理员可以管理座位、查看和处理预约记录。
> 本系统为课堂演示项目，使用 ASP.NET Core MVC + EF Core + SQL Server LocalDB 实现。

---

## 技术栈

| 层 | 选型 | 版本 |
|----|------|------|
| 后端框架 | ASP.NET Core MVC | .NET 9 |
| 视图引擎 | Razor | .NET 9 内建 |
| ORM | Entity Framework Core | 9.x Code First |
| 数据库 | SQL Server LocalDB | 随 VS2022 安装 |
| 前端样式 | Bootstrap 5 | 5.3.2（CDN） |
| 前端交互 | 原生 JavaScript | — |
| 自动化测试 | Playwright | 1.61.1（可选） |

---

## 功能清单

### 用户端（学生）

| 功能 | 说明 |
|------|------|
| 首页身份切换 | 点击用户卡片切换体验账号 |
| 仪表板概览 | 已登录后显示总座位数、空闲数、今日预约数 |
| 座位列表 | 显示所有启用座位的编号、楼层、区域、占用状态；支持楼层筛选 |
| 座位详情 | 显示当天 08:00–22:00 每小时的占用情况 |
| 预约提交 | 选择时段提交预约；检测时段冲突；已停用座位不可预约 |
| 我的预约 | 列出当前用户所有预约记录（倒序）；支持取消"待使用"预约 |
| 取消预约 | 弹窗确认后取消，释放时段 |

### 管理端（管理员）

| 功能 | 说明 |
|------|------|
| 管理员登录 | 用户名/密码登录；错误密码提示 |
| 预约管理列表 | 全部预约 + 状态下拉筛选（待使用/已完成/已取消） |
| 标记完成 | 将"待使用"预约标记为"已完成" |
| 管理端取消 | 管理员取消"待使用"预约 |
| 座位管理列表 | 全部座位（含已禁用）表格展示 |
| 新增座位 | Modal 表单新增；重复编号检测 |
| 编辑座位 | 加载现有数据 → 修改保存 |
| 启用/禁用座位 | IsEnabled 切换；停用后用户端不可见 |
| 统计数据 | 今日预约数、有效预约数、座位使用率、历史总预约 |

### 异常处理

| 场景 | 处理方式 |
|------|---------|
| 重复预约同一时段 | 冲突检测 → "该时段已被预约" |
| 预约已停用座位 | POST 层 Service 校验 → "该座位已停用，无法预约" |
| 非法取消他人预约 | UserName 归属检查 → "无权操作此预约" |
| 取消已完成/已取消预约 | 状态校验 → "只有待使用状态的预约可以取消" |
| Session 超时（管理端） | 重定向 `/Admin/Login?timeout=1` → 显示超时消息 |
| Session 超时（用户端） | 跳转首页 + TempData 提示"会话已超时" |
| 未登录访问管理端 | 拦截 → `/Admin/Login?timeout=1` |

---

## 页面清单

| # | 页面 | 路由 | 身份 | 说明 |
|---|------|------|------|------|
| 1 | 首页（身份选择） | `GET /` | 游客 | 显示 3 张体验账号卡片 |
| 2 | 首页（仪表板） | `GET /` | 已登录 | 显示总座位数、空闲数、今日预约数 |
| 3 | 座位列表 | `GET /Seats` | 任意 | 座位卡片网格 + 楼层筛选 |
| 4 | 座位详情 | `GET /Seats/Detail/{id}` | 任意 | 时段占用图 |
| 5 | 预约提交 | `GET /Reservation/Create?seatId={id}` | 已登录 | 时段选择 + 日期选择 |
| 6 | 我的预约 | `GET /Reservation/My` | 已登录 | 预约记录列表 + 取消按钮 |
| 7 | 管理员登录 | `GET /Admin/Login` | 未登录 | 用户名/密码表单 |
| 8 | 预约管理 | `GET /Admin/Reservations` | 管理员 | 全部预约表格 + 状态下拉 |
| 9 | 座位管理 | `GET /Admin/Seats` | 管理员 | 全部座位表格 + 新增/编辑/禁用 |
| 10 | 座位编辑 | `GET /Admin/Seats/Edit/{id}` | 管理员 | 编辑表单 |
| 11 | 统计 | `GET /Admin/Statistics` | 管理员 | 4 项 KPI 卡片 |

---

## 运行步骤

### 前提软件

| 软件 | 用途 | 验证命令 |
|------|------|---------|
| .NET SDK 9.0 | ASP.NET Core 运行时 | `dotnet --version` |
| Visual Studio 2022 (17.8+) | 开发 IDE（可选） | — |
| SQL Server LocalDB | 本地数据库 | 随 VS2022 自动安装 |
| Git | 版本控制 | `git --version` |
| Node.js 18+ | Playwright 自动化测试（可选） | `node --version` |

### 运行方式

```bash
# 1. 克隆仓库
git clone <仓库地址>
cd LibrarySeatSystem

# 2. 启动应用（首次启动自动建库 + 种子数据）
cd src/LibrarySeatReservation.Web
dotnet run --urls http://localhost:5000

# 3. 打开浏览器访问
#    http://localhost:5000
```

**注意**：默认端口为 `http://localhost:5264`（由 `Properties/launchSettings.json` 定义）。
使用 `--urls http://localhost:5000` 可指定为 5000 端口。

### 运行 Playwright 自动化测试（可选）

```bash
# 安装依赖（项目根目录）
npm install
npx playwright install chromium

# 运行全部测试
npm test

# 运行指定测试文件
npm run test:user    # 用户端烟雾测试
npm run test:admin   # 管理端烟雾测试
npm run test:cross   # 交叉流程测试

# 运行脚本烟雾测试（需先启动应用）
npm run smoke
```

---

## 数据库初始化方式

本系统使用 **EF Core Code First + 自动迁移**：

1. 首次 `dotnet run` 时自动执行迁移，创建 3 张表（Seats、Reservations、Admins）
2. 迁移完成后自动调用 `SeedData.InitAsync()` 写入种子数据
3. 如数据库中已有数据则跳过（幂等）

```bash
# 如需手动重建数据库：
cd src/LibrarySeatReservation.Web
dotnet ef database update
```

详细数据库说明见 `database/README.md` 和 `docs/08-数据库设计.md`。

---

## 种子数据

### 管理员（Admins 表）

| 用户名 | 密码 |
|--------|------|
| admin | admin123 |

### 座位（Seats 表，20 个）

| 楼层 | 区域 | 编号范围 | 数量 |
|------|------|---------|------|
| 2 楼 | 自习区 | ZX-01 ~ ZX-05 | 5 |
| 2 楼 | A 区 | A-01 ~ A-05 | 5 |
| 3 楼 | 期刊阅览区 | QK-01 ~ QK-05 | 5 |
| 3 楼 | B 区 | B-01 ~ B-05 | 5 |

所有座位初始为 **启用** 状态。

### 体验账号（前端硬编码，不存 DB）

| 卡片名称 | 说明 |
|---------|------|
| 学生 A | 点击卡片 → POST /Account/Switch → 写入 Session → 跳转座位列表 |
| 学生 B | 同上 |
| 学生 C | 同上 |

---

## 演示账号

| 身份 | 操作方式 |
|------|---------|
| 学生 A | 首页点击"学生 A"卡片（无密码） |
| 学生 B | 首页点击"学生 B"卡片（无密码） |
| 学生 C | 首页点击"学生 C"卡片（无密码） |
| 管理员 | 访问 `/Admin/Login` → 输入 admin / admin123 |

---

## 项目目录说明

```
LibrarySeatSystem/                     # 仓库根目录
├── README.md                          # 本文件（项目说明）
├── .gitignore                         # Git 忽略规则
├── package.json                       # npm 项目配置（Playwright 测试）
├── playwright.config.ts               # Playwright 测试配置
├── e2e/                               # Playwright 端到端测试
│   ├── smoke-basic.spec.ts            #   基础烟雾测试
│   ├── smoke-user.spec.ts             #   用户端烟雾测试
│   ├── smoke-admin.spec.ts            #   管理端烟雾测试
│   ├── smoke-cross.spec.ts            #   交叉流程测试
│   └── compatibility.spec.ts          #   兼容性测试
├── scripts/
│   └── smoke-test.ps1                 #   PowerShell 脚本烟雾测试
├── database/
│   └── README.md                      #   数据库初始化说明
├── docs/                              # 设计文档与开发记录
│   ├── 01-项目立项单.md ~ 16-*.md     #   Sprint 0~4 全量文档（含审计）
│   ├── 17-交付说明与项目复盘.md        #   交付总结
│   └── 项目任务板与迭代记录.md         #   迭代跟踪
├── prototype/                         # 静态原型 HTML
├── src/
│   └── LibrarySeatReservation.Web/    # Web 项目
│       ├── Program.cs                 #   应用入口
│       ├── appsettings.json           #   连接字符串与配置
│       ├── Controllers/               #   控制器层
│       │   ├── HomeController.cs
│       │   ├── SeatsController.cs
│       │   ├── ReservationController.cs
│       │   ├── AccountController.cs
│       │   └── Admin/
│       │       ├── AdminSeatsController.cs
│       │       ├── LoginController.cs
│       │       ├── ReservationsController.cs
│       │       └── StatisticsController.cs
│       ├── Models/                    #   数据层
│       │   ├── Entities/              #     实体类
│       │   ├── ViewModels/            #     视图模型
│       │   └── AppDbContext.cs
│       ├── Services/                  #   业务逻辑层
│       │   ├── ISeatService.cs
│       │   ├── SeatService.cs
│       │   ├── IReservationService.cs
│       │   └── ReservationService.cs
│       ├── Views/                     #   Razor 视图
│       │   ├── Home/Index.cshtml
│       │   ├── Seats/Index.cshtml
│       │   ├── Seats/Detail.cshtml
│       │   ├── Reservation/Create.cshtml
│       │   ├── Reservation/My.cshtml
│       │   └── Admin/
│       │       ├── Shared/_Layout.cshtml
│       │       ├── Login.cshtml
│       │       ├── Reservations.cshtml
│       │       ├── Seats.cshtml
│       │       ├── SeatEdit.cshtml
│       │       └── Statistics.cshtml
│       ├── wwwroot/css/site.css       #   自定义样式
│       └── Data/SeedData.cs           #   种子数据
└── LibrarySeatReservation.sln         #   解决方案文件
```

---

## 已知限制

- **密码明文存储**：管理员密码以明文存入 Admins 表。仅限课堂演示，生产环境必须用 bcrypt。
- **预约冲突检测存在竞态条件**：低并发场景下可接受。高并发需加数据库级锁或使用 `Serializable` 事务隔离级别。
- **座位占闲状态不实时推送**：刷新页面后重新计算。无 WebSocket / SignalR。
- **管理端手机端适配（补充验证）**：`table-responsive` 横向滚动和侧边栏折叠已实现，但手机端交互细节（sidebar 折叠确认）为 P2 未完成项。
- **Playwright 自动化测试使用 Chromium**：因本机 headless msedge 受 GPU 策略限制，改用 Playwright 内置 Chromium（渲染引擎一致）。
- **体验账号为前端硬编码**：学生 A/B/C 不存入数据库，切换通过 Session 实现。重启后原 Session 丢失，需重新选身份。
- **无用户注册/密码功能**：体验账号仅用于演示，无真实用户认证。

---

## 自动化测试

| 测试类型 | 用例数 | 通过率 | 执行方式 |
|---------|--------|--------|---------|
| Playwright 自动点击 | 23 | **100%** | `npm test` |
| 脚本烟雾测试 | 12 | **100%** | `npm run smoke`（需先启动应用） |
| 兼容性（移动端 375×812） | 3 | **100%** | `npx playwright test e2e/compatibility.spec.ts` |
| 兼容性（桌面端 1280×720） | 2 | **100%** | 同上 |
| 构建 | — | **0 错误 / 0 警告** | `dotnet build` |

---

## 最终提交

| 项目 | 值 |
|------|-----|
| 最终提交 | `c3bbfc4` |
| 最终标签 | `v1.0-final` |
| 提交日期 | 2026-07-14 |

---

## 许可证

课堂项目，仅用于教学演示。
