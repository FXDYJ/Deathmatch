using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MySqlConnector;
using LabApi.Features.Console;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace TheRiptide
{
    /// <summary>
    /// MySQL数据库配置
    /// </summary>
    public class DatabaseConfig
    {
        /// <summary>
        /// 数据库服务器地址
        /// </summary>
        public string Host { get; set; } = "localhost";
        
        /// <summary>
        /// 数据库端口
        /// </summary>
        public int Port { get; set; } = 3306;
        
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; } = "deathmatch";
        
        /// <summary>
        /// 数据库用户名
        /// </summary>
        public string Username { get; set; } = "root";
        
        /// <summary>
        /// 数据库密码
        /// </summary>
        public string Password { get; set; } = "";
    }

    /// <summary>
    /// 数据库管理类 - 使用MySQL数据库
    /// </summary>
    public class Database
    {
        #region 数据模型

        /// <summary>
        /// 玩家配置
        /// </summary>
        public class Config
        {
            public string UserId { get; set; }
            public ItemType Primary { get; set; } = ItemType.None;
            public ItemType Secondary { get; set; } = ItemType.None;
            public ItemType Tertiary { get; set; } = ItemType.None;
            public bool RageEnabled { get; set; } = false;
            public RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
            public string KillstreakMode { get; set; } = "";
        }

        /// <summary>
        /// 命中记录
        /// </summary>
        public class Hit
        {
            public long HitId { get; set; }
            public byte Health { get; set; } = 0;
            public byte Damage { get; set; } = 0;
            public byte Hitbox { get; set; } = 0;
            public byte Weapon { get; set; } = 0;
        }

        /// <summary>
        /// 装备配置
        /// </summary>
        public class Loadout : IEquatable<Loadout>
        {
            public long LoadoutId { get; set; }
            public string KillstreakMode { get; set; } = "";
            public ItemType Primary { get; set; } = ItemType.None;
            public uint PrimaryAttachmentCode { get; set; } = 0;
            public ItemType Secondary { get; set; } = ItemType.None;
            public uint SecondaryAttachmentCode { get; set; } = 0;
            public ItemType Tertiary { get; set; } = ItemType.None;
            public uint TertiaryAttachmentCode { get; set; } = 0;

            public bool Equals(Loadout other)
            {
                return KillstreakMode == other.KillstreakMode &&
                    Primary == other.Primary &&
                    PrimaryAttachmentCode == other.PrimaryAttachmentCode &&
                    Secondary == other.Secondary &&
                    SecondaryAttachmentCode == other.SecondaryAttachmentCode &&
                    Tertiary == other.Tertiary &&
                    TertiaryAttachmentCode == other.TertiaryAttachmentCode;
            }
        }

        /// <summary>
        /// 击杀记录
        /// </summary>
        public class Kill
        {
            public long KillId { get; set; }
            public float Time { get; set; } = UnityEngine.Time.time;
            public HitboxType Hitbox { get; set; } = HitboxType.Body;
            public ItemType Weapon { get; set; } = ItemType.None;
            public uint AttachmentCode { get; set; } = 0;
        }

        /// <summary>
        /// 生命周期记录
        /// </summary>
        public class Life
        {
            public long LifeId { get; set; }
            public RoleTypeId Role { get; set; } = RoleTypeId.ClassD;
            public int Shots { get; set; } = 0;
            public float Time { get; set; } = UnityEngine.Time.time;
            public Loadout Loadout { get; set; } = null;
            public List<Kill> Kills { get; set; } = new List<Kill>();
            public List<Hit> Delt { get; set; } = new List<Hit>();
            public List<Hit> Received { get; set; } = new List<Hit>();
            public Kill Death { get; set; } = null;
        }

        /// <summary>
        /// 回合记录
        /// </summary>
        public class Round
        {
            public long RoundId { get; set; }
            public DateTime Start { get; set; } = DateTime.Now;
            public DateTime End { get; set; } = DateTime.Now;
            public int MaxPlayers { get; set; } = 0;
        }

        /// <summary>
        /// 会话记录
        /// </summary>
        public class Session
        {
            public long SessionId { get; set; }
            public string Nickname { get; set; } = "*未连接";
            public DateTime Connect { get; set; } = DateTime.Now;
            public DateTime Disconnect { get; set; } = DateTime.Now;
            public Round Round { get; set; } = null;
            public List<Life> Lives { get; set; } = new List<Life>();
        }

        /// <summary>
        /// 追踪记录
        /// </summary>
        public class Tracking
        {
            public long TrackingId { get; set; }
            public List<Session> Sessions { get; set; } = new List<Session>();
        }

        /// <summary>
        /// 用户记录
        /// </summary>
        public class User
        {
            public string UserId { get; set; }
            public Tracking Tracking { get; set; } = new Tracking();
        }

        /// <summary>
        /// 排名状态
        /// </summary>
        public enum RankState { Unranked, Placement, Ranked };

        /// <summary>
        /// 排名记录
        /// </summary>
        public class Rank
        {
            public string UserId { get; set; }
            public RankState State { get; set; } = RankState.Unranked;
            public int PlacementMatches { get; set; } = 0;
            public float Rating { get; set; } = 0;
            public float Rd { get; set; } = 0;
            public float Rv { get; set; } = 0;
        }

        /// <summary>
        /// 经验记录
        /// </summary>
        public class Experience
        {
            public string UserId { get; set; }
            public int Value { get; set; } = 0;
            public int Level { get; set; } = 0;
            public int Stage { get; set; } = 0;
            public int Tier { get; set; } = 0;
        }

        /// <summary>
        /// 排行榜记录
        /// </summary>
        public class LeaderBoard
        {
            public string UserId { get; set; }
            public int TotalKills { get; set; } = 0;
            public int HighestKillstreak { get; set; } = 0;
            public string KillstreakTag { get; set; } = "";
            public int TotalPlayTime { get; set; } = 0;
        }

        #endregion

        private static Database instance = null;
        public static Database Singleton
        { 
            get 
            {
                if (instance == null)
                    instance = new Database();
                return instance;
            }
        }

        private string connectionString;
        private DatabaseConfig dbConfig = new DatabaseConfig();

        private Database()
        {
        }

        /// <summary>
        /// 获取数据库连接字符串
        /// </summary>
        private string GetConnectionString()
        {
            return $"Server={dbConfig.Host};Port={dbConfig.Port};Database={dbConfig.Database};User={dbConfig.Username};Password={dbConfig.Password};";
        }

        /// <summary>
        /// 加载数据库配置并初始化连接
        /// </summary>
        public void Load(string configPath)
        {
            connectionString = GetConnectionString();
            Logger.Info($"数据库连接已初始化: {dbConfig.Host}:{dbConfig.Port}/{dbConfig.Database}");
            
            // 尝试创建表结构
            InitializeTables();
        }

        /// <summary>
        /// 卸载数据库连接
        /// </summary>
        public void UnLoad()
        {
            Logger.Info("数据库连接已关闭");
        }

        /// <summary>
        /// 初始化数据库表结构
        /// </summary>
        private void InitializeTables()
        {
            try
            {
                using (var connection = new MySqlConnection(connectionString))
                {
                    connection.Open();
                    
                    // 执行建表SQL
                    string createTablesSql = GetCreateTablesSql();
                    using (var command = new MySqlCommand(createTablesSql, connection))
                    {
                        command.ExecuteNonQuery();
                    }
                    
                    Logger.Info("数据库表结构初始化完成");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"初始化数据库表结构失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取建表SQL语句
        /// </summary>
        public static string GetCreateTablesSql()
        {
            return @"
-- 死亡竞赛数据库表结构 SQL

-- 玩家配置表
CREATE TABLE IF NOT EXISTS configs (
    user_id VARCHAR(64) PRIMARY KEY,
    primary_weapon INT NOT NULL DEFAULT 0,
    secondary_weapon INT NOT NULL DEFAULT 0,
    tertiary_weapon INT NOT NULL DEFAULT 0,
    rage_enabled BOOLEAN NOT NULL DEFAULT FALSE,
    role INT NOT NULL DEFAULT 0,
    killstreak_mode VARCHAR(64) NOT NULL DEFAULT ''
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 排名表
CREATE TABLE IF NOT EXISTS ranks (
    user_id VARCHAR(64) PRIMARY KEY,
    state INT NOT NULL DEFAULT 0,
    placement_matches INT NOT NULL DEFAULT 0,
    rating FLOAT NOT NULL DEFAULT 0,
    rd FLOAT NOT NULL DEFAULT 0,
    rv FLOAT NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 经验表
CREATE TABLE IF NOT EXISTS experiences (
    user_id VARCHAR(64) PRIMARY KEY,
    value INT NOT NULL DEFAULT 0,
    level INT NOT NULL DEFAULT 0,
    stage INT NOT NULL DEFAULT 0,
    tier INT NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 排行榜表
CREATE TABLE IF NOT EXISTS leader_board (
    user_id VARCHAR(64) PRIMARY KEY,
    total_kills INT NOT NULL DEFAULT 0,
    highest_killstreak INT NOT NULL DEFAULT 0,
    killstreak_tag VARCHAR(64) NOT NULL DEFAULT '',
    total_play_time INT NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 回合表
CREATE TABLE IF NOT EXISTS rounds (
    round_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    start_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    end_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    max_players INT NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 用户表
CREATE TABLE IF NOT EXISTS users (
    user_id VARCHAR(64) PRIMARY KEY,
    tracking_id BIGINT
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 追踪表
CREATE TABLE IF NOT EXISTS tracking (
    tracking_id BIGINT AUTO_INCREMENT PRIMARY KEY
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 会话表
CREATE TABLE IF NOT EXISTS sessions (
    session_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    tracking_id BIGINT,
    nickname VARCHAR(128) NOT NULL DEFAULT '*未连接',
    connect_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    disconnect_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
    round_id BIGINT,
    FOREIGN KEY (tracking_id) REFERENCES tracking(tracking_id) ON DELETE CASCADE,
    FOREIGN KEY (round_id) REFERENCES rounds(round_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 装备表
CREATE TABLE IF NOT EXISTS loadouts (
    loadout_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    killstreak_mode VARCHAR(64) NOT NULL DEFAULT '',
    primary_weapon INT NOT NULL DEFAULT 0,
    primary_attachment_code INT UNSIGNED NOT NULL DEFAULT 0,
    secondary_weapon INT NOT NULL DEFAULT 0,
    secondary_attachment_code INT UNSIGNED NOT NULL DEFAULT 0,
    tertiary_weapon INT NOT NULL DEFAULT 0,
    tertiary_attachment_code INT UNSIGNED NOT NULL DEFAULT 0
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 生命周期表
CREATE TABLE IF NOT EXISTS lives (
    life_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    session_id BIGINT,
    role INT NOT NULL DEFAULT 0,
    shots INT NOT NULL DEFAULT 0,
    time FLOAT NOT NULL DEFAULT 0,
    loadout_id BIGINT,
    death_kill_id BIGINT,
    FOREIGN KEY (session_id) REFERENCES sessions(session_id) ON DELETE CASCADE,
    FOREIGN KEY (loadout_id) REFERENCES loadouts(loadout_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 击杀表
CREATE TABLE IF NOT EXISTS kills (
    kill_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    life_id BIGINT,
    time FLOAT NOT NULL DEFAULT 0,
    hitbox INT NOT NULL DEFAULT 0,
    weapon INT NOT NULL DEFAULT 0,
    attachment_code INT UNSIGNED NOT NULL DEFAULT 0,
    FOREIGN KEY (life_id) REFERENCES lives(life_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 命中表
CREATE TABLE IF NOT EXISTS hits (
    hit_id BIGINT AUTO_INCREMENT PRIMARY KEY,
    life_id BIGINT,
    hit_type ENUM('delt', 'received') NOT NULL,
    health TINYINT UNSIGNED NOT NULL DEFAULT 0,
    damage TINYINT UNSIGNED NOT NULL DEFAULT 0,
    hitbox TINYINT UNSIGNED NOT NULL DEFAULT 0,
    weapon TINYINT UNSIGNED NOT NULL DEFAULT 0,
    FOREIGN KEY (life_id) REFERENCES lives(life_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- 添加外键约束（生命周期表的死亡击杀引用）
ALTER TABLE lives ADD CONSTRAINT fk_death_kill 
    FOREIGN KEY (death_kill_id) REFERENCES kills(kill_id) ON DELETE SET NULL;

-- 创建索引
CREATE INDEX idx_sessions_tracking ON sessions(tracking_id);
CREATE INDEX idx_lives_session ON lives(session_id);
CREATE INDEX idx_kills_life ON kills(life_id);
CREATE INDEX idx_hits_life ON hits(life_id);
CREATE INDEX idx_leader_board_kills ON leader_board(total_kills);
CREATE INDEX idx_leader_board_killstreak ON leader_board(highest_killstreak);
CREATE INDEX idx_ranks_rating ON ranks(rating);
";
        }

        #region 配置操作

        /// <summary>
        /// 加载玩家配置
        /// </summary>
        public void LoadConfig(int playerId, string oderId)
        {
            DbDelayedAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT * FROM configs WHERE user_id = @userId";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", oderId);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                if (await reader.ReadAsync())
                                {
                                    // 配置加载成功，应用到玩家
                                    Timing.CallDelayed(0.0f, () =>
                                    {
                                        // 这里需要根据实际情况应用配置
                                        Logger.Debug($"已加载玩家配置: {oderId}");
                                    });
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"加载玩家配置失败: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// 保存玩家配置
        /// </summary>
        public void SaveConfig(string oderId, Config config)
        {
            DbAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = @"INSERT INTO configs (user_id, primary_weapon, secondary_weapon, tertiary_weapon, rage_enabled, role, killstreak_mode)
                                      VALUES (@userId, @primary, @secondary, @tertiary, @rage, @role, @killstreak)
                                      ON DUPLICATE KEY UPDATE 
                                      primary_weapon = @primary, secondary_weapon = @secondary, tertiary_weapon = @tertiary,
                                      rage_enabled = @rage, role = @role, killstreak_mode = @killstreak";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", oderId);
                            command.Parameters.AddWithValue("@primary", (int)config.Primary);
                            command.Parameters.AddWithValue("@secondary", (int)config.Secondary);
                            command.Parameters.AddWithValue("@tertiary", (int)config.Tertiary);
                            command.Parameters.AddWithValue("@rage", config.RageEnabled);
                            command.Parameters.AddWithValue("@role", (int)config.Role);
                            command.Parameters.AddWithValue("@killstreak", config.KillstreakMode);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"保存玩家配置失败: {ex.Message}");
                }
            });
        }

        #endregion

        #region 排名操作

        /// <summary>
        /// 加载玩家排名
        /// </summary>
        public void LoadRank(string oderId, Action<Rank> callback)
        {
            DbDelayedAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT * FROM ranks WHERE user_id = @userId";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", oderId);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                Rank rank = null;
                                if (await reader.ReadAsync())
                                {
                                    rank = new Rank
                                    {
                                        UserId = reader.GetString("user_id"),
                                        State = (RankState)reader.GetInt32("state"),
                                        PlacementMatches = reader.GetInt32("placement_matches"),
                                        Rating = reader.GetFloat("rating"),
                                        Rd = reader.GetFloat("rd"),
                                        Rv = reader.GetFloat("rv")
                                    };
                                }
                                Timing.CallDelayed(0.0f, () => callback?.Invoke(rank));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"加载玩家排名失败: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// 保存玩家排名
        /// </summary>
        public void SaveRank(Rank rank)
        {
            DbAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = @"INSERT INTO ranks (user_id, state, placement_matches, rating, rd, rv)
                                      VALUES (@userId, @state, @matches, @rating, @rd, @rv)
                                      ON DUPLICATE KEY UPDATE 
                                      state = @state, placement_matches = @matches, rating = @rating, rd = @rd, rv = @rv";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", rank.UserId);
                            command.Parameters.AddWithValue("@state", (int)rank.State);
                            command.Parameters.AddWithValue("@matches", rank.PlacementMatches);
                            command.Parameters.AddWithValue("@rating", rank.Rating);
                            command.Parameters.AddWithValue("@rd", rank.Rd);
                            command.Parameters.AddWithValue("@rv", rank.Rv);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"保存玩家排名失败: {ex.Message}");
                }
            });
        }

        #endregion

        #region 经验操作

        /// <summary>
        /// 加载玩家经验
        /// </summary>
        public void LoadExperience(string oderId, Action<Experience> callback)
        {
            DbDelayedAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = "SELECT * FROM experiences WHERE user_id = @userId";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", oderId);
                            using (var reader = await command.ExecuteReaderAsync())
                            {
                                Experience xp = null;
                                if (await reader.ReadAsync())
                                {
                                    xp = new Experience
                                    {
                                        UserId = reader.GetString("user_id"),
                                        Value = reader.GetInt32("value"),
                                        Level = reader.GetInt32("level"),
                                        Stage = reader.GetInt32("stage"),
                                        Tier = reader.GetInt32("tier")
                                    };
                                }
                                Timing.CallDelayed(0.0f, () => callback?.Invoke(xp));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"加载玩家经验失败: {ex.Message}");
                }
            });
        }

        /// <summary>
        /// 保存玩家经验
        /// </summary>
        public void SaveExperience(string oderId, Experience xp)
        {
            DbAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = @"INSERT INTO experiences (user_id, value, level, stage, tier)
                                      VALUES (@userId, @value, @level, @stage, @tier)
                                      ON DUPLICATE KEY UPDATE 
                                      value = @value, level = @level, stage = @stage, tier = @tier";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", oderId);
                            command.Parameters.AddWithValue("@value", xp.Value);
                            command.Parameters.AddWithValue("@level", xp.Level);
                            command.Parameters.AddWithValue("@stage", xp.Stage);
                            command.Parameters.AddWithValue("@tier", xp.Tier);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"保存玩家经验失败: {ex.Message}");
                }
            });
        }

        #endregion

        #region 排行榜操作

        /// <summary>
        /// 更新排行榜
        /// </summary>
        public void UpdateLeaderBoard(string oderId, LeaderBoard lb)
        {
            DbAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        string sql = @"INSERT INTO leader_board (user_id, total_kills, highest_killstreak, killstreak_tag, total_play_time)
                                      VALUES (@userId, @kills, @killstreak, @tag, @time)
                                      ON DUPLICATE KEY UPDATE 
                                      total_kills = @kills, highest_killstreak = @killstreak, 
                                      killstreak_tag = @tag, total_play_time = @time";
                        using (var command = new MySqlCommand(sql, connection))
                        {
                            command.Parameters.AddWithValue("@userId", oderId);
                            command.Parameters.AddWithValue("@kills", lb.TotalKills);
                            command.Parameters.AddWithValue("@killstreak", lb.HighestKillstreak);
                            command.Parameters.AddWithValue("@tag", lb.KillstreakTag);
                            command.Parameters.AddWithValue("@time", lb.TotalPlayTime);
                            await command.ExecuteNonQueryAsync();
                        }
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"更新排行榜失败: {ex.Message}");
                }
            });
        }

        #endregion

        #region 数据删除

        /// <summary>
        /// 删除玩家数据
        /// </summary>
        public void DeleteData(string oderId)
        {
            DbAsync(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        
                        // 删除所有相关表中的数据
                        string[] tables = { "configs", "experiences", "ranks", "leader_board", "users" };
                        foreach (string table in tables)
                        {
                            string sql = $"DELETE FROM {table} WHERE user_id = @userId";
                            using (var command = new MySqlCommand(sql, connection))
                            {
                                command.Parameters.AddWithValue("@userId", oderId);
                                await command.ExecuteNonQueryAsync();
                            }
                        }
                        
                        Logger.Info($"已删除玩家数据: {oderId}");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"删除玩家数据失败: {ex.Message}");
                }
            });
        }

        #endregion

        #region 异步辅助方法

        /// <summary>
        /// 异步执行数据库操作
        /// </summary>
        private void DbAsync(Func<Task> action)
        {
            Task.Run(async () =>
            {
                try
                {
                    await action();
                }
                catch (Exception ex)
                {
                    Logger.Error($"数据库错误: {ex}");
                }
            });
        }

        /// <summary>
        /// 延迟异步执行数据库操作
        /// </summary>
        private void DbDelayedAsync(Func<Task> action)
        {
            Timing.CallDelayed(0.0f, () =>
            {
                Task.Run(async () =>
                {
                    try
                    {
                        await action();
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"数据库错误: {ex}");
                    }
                });
            });
        }

        /// <summary>
        /// 公共异步执行方法
        /// </summary>
        public void Async(Action<MySqlConnection> action)
        {
            Task.Run(async () =>
            {
                try
                {
                    using (var connection = new MySqlConnection(connectionString))
                    {
                        await connection.OpenAsync();
                        action(connection);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"数据库错误: {ex}");
                }
            });
        }

        /// <summary>
        /// 检查点（MySQL不需要此操作，保留接口兼容性）
        /// </summary>
        public void Checkpoint()
        {
            // MySQL 不需要像 LiteDB 那样的检查点操作
        }

        #endregion
    }
}
