# 开发准备与 Sprint 0 规划

> 前置文档：`docs/07-系统设计说明.md`、`docs/08-数据库设计.md`、`docs/09-关键链路详细设计.md`
> 本文件直接服务下一步：开发起步与项目骨架
> 读者：软件实习生（需要知道怎么搭开发环境、怎么切分支、怎么提 PR）

---

## 1. 仓库结构

### 1.1 远端仓库

| 项目 | 地址 |
|------|------|
| GitHub | `https://github.com/zyer205/Library-book` |
| 默认分支 | `main` |

### 1.2 本地仓库初始化

```bash
# 在项目根目录执行
git init
git remote add origin https://github.com/zyer205/Library-book.git
git add .
git commit -m "chore: init project with design docs and sprint planning"
git push -u origin main
```

> 当前仓库属于设计阶段产物提交。后续 Sprint 的代码文件将在对应分支中开发。

### 1.3 当前推送状态

| 项目 | 状态 |
|------|------|
| 本地仓库 | ✅ 已初始化，已创建初始 commit（45 个文件，不含 .omo/） |
| 远程仓库 | ✅ 已推送至 `https://github.com/zyer205/Library-book.git` `main` 分支 |

> 初始推送时如遇代理问题，尝试清除 Git 代理配置后重试：
> ```bash
> git config --global --unset http.proxy
> git config --global --unset https.proxy
> git push -u origin main
> ```

---

## 2. 分支策略

采用简化的 Git Flow（适合单人/双人课堂项目）：

### 2.1 分支命名规范

| 分支类型 | 命名格式 | 示例 | 说明 |
|---------|---------|------|------|
| 主分支 | `main` | `main` | 稳定可演示版本，只接受合并请求 |
| 开发分支 | `dev` | `dev` | 日常开发集成分支，从 main 创建 |
| 功能分支 | `feat/<模块>-<简述>` | `feat/seat-list`、`feat/admin-login` | 在 dev 上创建，完成后合并回 dev |
| 修复分支 | `fix/<简述>` | `fix/conflict-detection` | Bug 修复分支 |
| 文档分支 | `docs/<简述>` | `docs/readme-update` | 文档更新分支 |

### 2.2 分支协作流程

```
main ────●─────────●──────────●──────────●─────────
         │         │          │          │
         ├── dev ──┼────●─────┼────●─────┼────●───
         │         │    │     │    │     │    │
         │         │  feat/   │  feat/   │  fix/
         │         │  seat-   │  admin-  │  bug1
         │         │  list    │  login   │
```

**开发流程：**
1. 从 `main` 创建 `dev` 分支
2. 从 `dev` 创建 `feat/<模块>` 分支
3. 在 `feat/<模块>` 上开发
4. 开发完成 → 合并回 `dev`
5. 一个 Sprint 结束时 → `dev` 合并到 `main`（可演示状态）

> 单人开发可以直接在 `dev` 上开发，省略功能分支。但建议至少使用 `main` + `dev` 两条分支。

---

## 3. 提交规范

### 3.1 提交信息格式

```
<type>: <简短描述>

- <可选详细说明>
- <可选改动文件>
```

### 3.2 提交类型

| 类型 | 用途 | 示例 |
|------|------|------|
| `feat` | 新功能 | `feat: add seat list page with floor filter` |
| `fix` | 缺陷修复 | `fix: conflict detection off-by-one error` |
| `chore` | 项目配置、依赖 | `chore: init project with dotnet new mvc` |
| `docs` | 文档变更 | `docs: update README with demo credentials` |
| `refactor` | 重构（非功能变更） | `refactor: extract IsOccupiedNow to service` |
| `style` | 代码格式（非逻辑变更） | `style: fix indentation in SeatController` |
| `test` | 测试相关 | `test: add TC03 reservation success path` |

### 3.3 提交频率建议

- 每完成一个可编译状态 → 提交一次
- 每完成一张任务卡 → 至少提交一次
- 提交信息用英文或中文均可，保持一种语言

---

## 4. Sprint 0 目标

### 4.1 Sprint 0 范围

搭建项目骨架 + 数据库初始化 + 首次运行验证。

**起止时间：** 设计文档完成后 → 确认 `dotnet run` 可启动、数据库已建表

### 4.2 Sprint 0 交付物

| # | 交付物 | 验收标准 |
|---|--------|---------|
| 1 | `.sln` 解决方案文件 | Visual Studio 可打开，包含 Web 项目 |
| 2 | `.csproj` 项目文件 | NuGet 包（EF Core SqlServer）已安装 |
| 3 | `Program.cs` | 注册了 DbContext + Session + Service 层，中间件管道完整 |
| 4 | `appsettings.json` | 包含 LocalDB 连接字符串 |
| 5 | Entity 类（3 个） | Seat.cs、Reservation.cs、Admin.cs 字段与 doc 08 一致 |
| 6 | `AppDbContext` | Fluent API 配置了主键、索引、默认值、外键 Restrict |
| 7 | 首次迁移 | `Add-Migration InitialCreate` 成功，生成的迁移文件与 doc 08 DDL 一致 |
| 8 | 首次建表 | `Update-Database` 成功，数据库 3 张表已创建 |
| 9 | `SeedData.cs` | 写入 1 个管理员 + 15~20 个座位 |
| 10 | `dotnet build` 通过 | 无编译错误 |
| 11 | `dotnet run` 通过 | 浏览器可访问首页 |

### 4.3 Sprint 0 具体步骤

```
第 1 步：创建解决方案和项目
  dotnet new sln -n LibrarySeatSystem
  dotnet new mvc -n LibrarySeatSystem
  dotnet sln add LibrarySeatSystem/LibrarySeatSystem.csproj

第 2 步：安装 NuGet 包
  cd LibrarySeatSystem
  dotnet add package Microsoft.EntityFrameworkCore.SqlServer

第 3 步：创建目录结构
  mkdir Models/Entities Models/ViewModels Services Data
  mkdir Controllers/Admin
  mkdir Views/Admin/Shared
  mkdir wwwroot/css

第 4 步：编写 Entity 类（3 个文件）
  → docs/08-数据库设计.md §4.1~4.3

第 5 步：编写 AppDbContext
  → docs/08-数据库设计.md §4 Fluent API 配置

第 6 步：配置 Program.cs
  → 注册 DbContext（连接字符串）
  → 注册 Session 服务 + 中间件
  → 注册 Service 到 DI 容器
  → 调用 SeedData.Initialize()

第 7 步：配置 appsettings.json
  ConnectionStrings.DefaultConnection = "Server=(localdb)\MSSQLLocalDB;Database=LibrarySeatDb;"

第 8 步：迁移
  dotnet ef migrations add InitialCreate
  dotnet ef database update

第 9 步：编写 SeedData.cs
  → 写入 1 个管理员
  → 写入 15~20 个座位

第 10 步：验证
  dotnet build
  dotnet run
  → 浏览器访问 http://localhost:5000
  → 确认首页正常渲染
  → 确认数据库表中已有 Seed Data
```

---

## 5. Sprint 1~4 主 Sprint 粗计划

### 5.1 总体时间线

```
Sprint 0 ─── 开发准备与骨架搭建
   │           交付: 可运行的空项目 + 数据库
   ▼
Sprint 1 ─── 用户端核心功能（P0）
   │           交付: 完整用户预约主链路可走通
   ▼
Sprint 2 ─── 管理端功能（P0+P1）
│           交付: 管理员可登录、管理预约、管理座位、查看统计页
   ▼
Sprint 3 ─── 联调与缺陷修复
   │           交付: 全链路稳定、审计修复项落实
   ▼
Sprint 4 ─── 缓冲 Sprint（P2 功能 + 样式 + 文档）
              交付: 统计页（如有余力）、文档补充
```

### 5.2 Sprint 1：用户端核心功能

| 项目 | 内容 |
|------|------|
| **目标** | 完整走通用户预约主链路（身份切换 → 座列表 → 详情 → 预约 → 我的预约 → 取消） |
| **阶段最低完成线** | 浏览器可完成一次完整预约流程并取消 |
| **允许迭代轮次** | 多轮。每完成一个子模块可 commit 一次，Sprint 结束时 dev → main |
| **对应 doc** | `docs/07` §5 首页模块 + 座位模块 + 预约模块；`docs/09` §2 |
| **优先级** | P0 |

**初步任务拆分：**

| 任务 | 关联编号 | 对应 Service |
|------|---------|-------------|
| 首页身份选择 + 切换用户 | T13-01~02 | AccountService（或直接在 AccountController 中） |
| 座位列表（含占闲状态） | T13-03 | ISeatService |
| 座位详情（含时段图） | T13-04 | ISeatService |
| 预约提交 + 冲突检测 | T13-05 | IReservationService |
| 我的预约 | T13-06 | IReservationService |
| 取消预约 | T13-07 | IReservationService |

### 5.3 Sprint 2：管理端功能

| 项目 | 内容 |
|------|------|
| **目标** | 管理员可登录、查看/筛选/标记完成/取消预约、管理座位 |
| **阶段最低完成线** | 管理员登录后可完成一次"标记完成"和一次"新增座位" |
| **允许迭代轮次** | 多轮 |
| **对应 doc** | `docs/07` §5 管理端模块；`docs/09` §3 |
| **优先级** | P0 |

**初步任务拆分：**

| 任务 | 关联编号 | 对应 Service |
|------|---------|-------------|
| 管理员登录 + 退出 | T14-01 | Admin 表直接查询 |
| 预约管理（列表 + 状态筛选） | T14-02 | IReservationService.GetAllAsync |
| 标记完成/取消（管理员） | T14-03 | IReservationService.MarkDoneAsync / AdminCancelAsync |
| 座位管理（列表） | T14-04 | ISeatService |
| 新增座位 | T14-05 | ISeatService.CreateAsync |
| 编辑座位 | T14-06 | ISeatService.UpdateAsync |
| 启用/禁用座位 | T14-07 | ISeatService.ToggleEnabledAsync |
| **统计页** | **T14-08** | **IReservationService 聚合查询（今日预约数/有效预约数/使用率）** |

### 5.4 Sprint 3：联调与缺陷修复

| 项目 | 内容 |
|------|------|
| **目标** | 全链路可走通，审计修复项验证，已知 bug 修复 |
| **阶段最低完成线** | 走完 docs/09 §4.5 全部 13 个测试用例，全部通过 |
| **允许迭代轮次** | 多轮 |
| **优先级** | P0（修复）/ P1（优化） |

**主要工作：**

- 执行 13 个测试用例（TC01~TC13），逐项验证
- 落实 docs/07 §10.4 的 6 项静态原型审计修复
- N+1 查询优化（批量 `GetOccupiedSeatIdsAsync`）
- 异常场景补充处理
- Session 超时与权限控制验证
- 管理端手机端适配验证

### 5.5 Sprint 4：缓冲 Sprint（样式 + 文档）

| 项目 | 内容 |
|------|------|
| **目标** | UI 样式调整、文档完善 |
| **阶段最低完成线** | 只要 Sprint 3 产出稳定，Sprint 4 为空跑或不跑 |
| **允许迭代轮次** | 多轮 |
| **优先级** | P2 |

**候选工作：**

- Bootstrap 样式微调
- README 更新为最终版本
- 部署说明文档
- 扩展练习（按实际地图排列座位等）

---

## 6. 里程碑节点

不超过 4 个里程碑：

| 里程碑 | 关联 Sprint | 完成标志 | 预计节点 |
|--------|-----------|---------|---------|
| **M1：骨架就绪** | Sprint 0 | `dotnet run` 可启动，3 张表已建，Seed Data 就绪 | Sprint 0 结束时 |
| **M2：用户端可用** | Sprint 1 | 用户可完成"身份切换→浏览→预约→取消"全链路 | Sprint 1 结束时 |
| **M3：管理端可用** | Sprint 2 | 管理员可登录、管理预约、管理座位、查看统计页 | Sprint 2 结束时 |
| **M4：系统可演示** | Sprint 3 + 4 | 全链路稳定、13 个 TC 通过、审计修复落实、可交付演示 | Sprint 4 结束时 |

---

## 7. 默认补足项 / 当前假设

| # | 假设 | 说明 |
|---|------|------|
| H1 | 开发机已安装 Visual Studio 2022 和 SQL Server LocalDB | 如果未安装，Sprint 0 第 0 步为安装环境 |
| H2 | 每次 `git commit` 前确保代码可编译 | 原则：不提交不可编译的代码 |
| H3 | 每个 Sprint 结束时 `dev` 分支应处于可演示状态 | 即使功能未全部完成，首页必须可访问 |
| H4 | 任务卡编号如 T13-01 中的 "13" 代表 Sprint 1 | T10 系列预留给设计阶段，T12=Sprint 0，T13=Sprint 1，依此类推 |
| H5 | 团队规模为 1 人（实习生单人开发） | 分支策略和协作流程按单人简化 |
