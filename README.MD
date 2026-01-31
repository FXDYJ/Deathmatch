# 死亡竞赛游戏模式 (Deathmatch Gamemode)

基于 LabAPI 框架的 SCP: Secret Laboratory 死亡竞赛游戏模式插件。

## 功能特性

- 即时重生
- 完全可自定义的装备配置
- 角色选择系统
- 自定义连杀奖励选择
- 安全出生点
- 击杀信息显示与连杀公告
- 动态设施规模（根据玩家数量开关房间）
- 排名与经验系统
- 统计数据追踪
- 多列排序排行榜
- 武器与配件黑名单
- 先加入的玩家可选择设施起始区域
- Alt 键切换全局语音聊天
- 玩家离开时保存装备、角色和偏好设置
- 回合结束时显示统计数据
- 自动清理物品/尸体

## 安装方法

### 手动安装

1. 将 `Deathmatch.dll` 放入 `PluginAPI/plugins/你的服务器端口号/` 目录
2. 将 `dependencies` 文件夹中的所有依赖放入 `PluginAPI/plugins/你的服务器端口号/dependencies/` 目录

### 依赖项

- [MySqlConnector](https://mysqlconnector.net/) - MySQL 数据库连接器
- [Glicko2.dll](https://github.com/ikhanage/Glicko2) - 排名系统
- [0Harmony.dll](https://github.com/pardeike/Harmony) - Harmony 补丁库

## 数据库配置

本插件使用 MySQL 数据库存储玩家数据。请按以下步骤配置：

1. 安装 MySQL 8.0 或更高版本
2. 创建数据库：
   ```sql
   CREATE DATABASE deathmatch CHARACTER SET utf8mb4 COLLATE utf8mb4_unicode_ci;
   ```
3. 运行 `database_schema.sql` 脚本创建表结构
4. 在插件配置中设置数据库连接信息

### 数据库表结构

详细的表结构请参阅 `database_schema.sql` 文件，包含以下主要表：

- `configs` - 玩家配置
- `ranks` - 玩家排名
- `experiences` - 玩家经验
- `leader_board` - 排行榜
- `rounds` - 游戏回合
- `sessions` - 游戏会话
- `lives` - 生命周期
- `kills` - 击杀记录
- `hits` - 命中记录

## 配置文件

插件会生成以下配置文件：

- `main_config.yml` - 主配置
- `rooms_config.yml` - 房间配置
- `killstreak_config.yml` - 连杀奖励配置
- `loadout_config.yml` - 装备配置
- `lobby_config.yml` - 大厅配置
- `experience_config.yml` - 经验配置
- `rank_config.yml` - 排名配置
- `tracking_config.yml` - 追踪配置
- `translation_config.yml` - 翻译配置
- `attachment_blacklist_config.yml` - 配件黑名单配置
- `voice_chat_config.yml` - 语音聊天配置
- `cleanup_config.yml` - 清理配置
- `leader_board.yml` - 排行榜配置

## 框架要求

- LabAPI (SCP:SL 官方插件框架)
- .NET Framework 4.8

## 更新日志

### v2.0.0
- 迁移至 LabAPI 框架
- 使用 MySQL 数据库替代 LiteDB
- 完全汉化所有文本
- 移除 CedMod Event 支持，仅作为独立插件运行
- 代码优化与重构

## 贡献者

- The Riptide - 原作者
- 社区贡献者

## 许可证

本项目遵循开源许可证。
