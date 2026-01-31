-- ============================================================
-- 死亡竞赛 (Deathmatch) 数据库表结构 SQL
-- 版本: 2.0
-- 数据库: MySQL 8.0+
-- 字符集: utf8mb4
-- ============================================================

-- 创建数据库（如果不存在）
CREATE DATABASE IF NOT EXISTS deathmatch
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_unicode_ci;

USE deathmatch;

-- ============================================================
-- 玩家配置表
-- 存储玩家的装备、角色和连杀模式配置
-- ============================================================
CREATE TABLE IF NOT EXISTS configs (
    user_id VARCHAR(64) PRIMARY KEY COMMENT '玩家唯一标识符（Steam ID等）',
    primary_weapon INT NOT NULL DEFAULT 0 COMMENT '主武器类型',
    secondary_weapon INT NOT NULL DEFAULT 0 COMMENT '副武器类型',
    tertiary_weapon INT NOT NULL DEFAULT 0 COMMENT '第三武器类型',
    rage_enabled BOOLEAN NOT NULL DEFAULT FALSE COMMENT '是否启用狂暴模式',
    role INT NOT NULL DEFAULT 0 COMMENT '角色类型',
    killstreak_mode VARCHAR(64) NOT NULL DEFAULT '' COMMENT '连杀奖励模式',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='玩家配置表';

-- ============================================================
-- 排名表
-- 存储玩家的Glicko-2排名数据
-- ============================================================
CREATE TABLE IF NOT EXISTS ranks (
    user_id VARCHAR(64) PRIMARY KEY COMMENT '玩家唯一标识符',
    state INT NOT NULL DEFAULT 0 COMMENT '排名状态: 0=未排名, 1=定级中, 2=已排名',
    placement_matches INT NOT NULL DEFAULT 0 COMMENT '定级赛场次',
    rating FLOAT NOT NULL DEFAULT 1500 COMMENT 'Glicko-2评分',
    rd FLOAT NOT NULL DEFAULT 350 COMMENT '评分偏差',
    rv FLOAT NOT NULL DEFAULT 0.06 COMMENT '评分波动性',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='玩家排名表';

-- ============================================================
-- 经验表
-- 存储玩家的经验值和等级信息
-- ============================================================
CREATE TABLE IF NOT EXISTS experiences (
    user_id VARCHAR(64) PRIMARY KEY COMMENT '玩家唯一标识符',
    value INT NOT NULL DEFAULT 0 COMMENT '当前经验值',
    level INT NOT NULL DEFAULT 0 COMMENT '等级',
    stage INT NOT NULL DEFAULT 0 COMMENT '阶段',
    tier INT NOT NULL DEFAULT 0 COMMENT '段位',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='玩家经验表';

-- ============================================================
-- 排行榜表
-- 存储玩家的排行榜统计数据
-- ============================================================
CREATE TABLE IF NOT EXISTS leader_board (
    user_id VARCHAR(64) PRIMARY KEY COMMENT '玩家唯一标识符',
    total_kills INT NOT NULL DEFAULT 0 COMMENT '总击杀数',
    highest_killstreak INT NOT NULL DEFAULT 0 COMMENT '最高连杀数',
    killstreak_tag VARCHAR(64) NOT NULL DEFAULT '' COMMENT '最高连杀时使用的连杀模式',
    total_play_time INT NOT NULL DEFAULT 0 COMMENT '总游玩时间（秒）',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP COMMENT '更新时间'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='排行榜表';

-- ============================================================
-- 回合表
-- 存储每个游戏回合的信息
-- ============================================================
CREATE TABLE IF NOT EXISTS rounds (
    round_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '回合唯一标识符',
    start_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '回合开始时间',
    end_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '回合结束时间',
    max_players INT NOT NULL DEFAULT 0 COMMENT '回合最大玩家数'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='游戏回合表';

-- ============================================================
-- 追踪表
-- 用于跟踪玩家会话
-- ============================================================
CREATE TABLE IF NOT EXISTS tracking (
    tracking_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '追踪唯一标识符'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='追踪表';

-- ============================================================
-- 用户表
-- 存储用户基本信息和追踪关联
-- ============================================================
CREATE TABLE IF NOT EXISTS users (
    user_id VARCHAR(64) PRIMARY KEY COMMENT '玩家唯一标识符',
    tracking_id BIGINT COMMENT '关联的追踪ID',
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP COMMENT '创建时间',
    FOREIGN KEY (tracking_id) REFERENCES tracking(tracking_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='用户表';

-- ============================================================
-- 会话表
-- 存储玩家的游戏会话信息
-- ============================================================
CREATE TABLE IF NOT EXISTS sessions (
    session_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '会话唯一标识符',
    tracking_id BIGINT COMMENT '关联的追踪ID',
    nickname VARCHAR(128) NOT NULL DEFAULT '*未连接' COMMENT '玩家昵称',
    connect_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '连接时间',
    disconnect_time DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP COMMENT '断开时间',
    round_id BIGINT COMMENT '关联的回合ID',
    FOREIGN KEY (tracking_id) REFERENCES tracking(tracking_id) ON DELETE CASCADE,
    FOREIGN KEY (round_id) REFERENCES rounds(round_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='游戏会话表';

-- ============================================================
-- 装备表
-- 存储玩家装备配置详情
-- ============================================================
CREATE TABLE IF NOT EXISTS loadouts (
    loadout_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '装备配置唯一标识符',
    killstreak_mode VARCHAR(64) NOT NULL DEFAULT '' COMMENT '连杀奖励模式',
    primary_weapon INT NOT NULL DEFAULT 0 COMMENT '主武器类型',
    primary_attachment_code INT UNSIGNED NOT NULL DEFAULT 0 COMMENT '主武器配件代码',
    secondary_weapon INT NOT NULL DEFAULT 0 COMMENT '副武器类型',
    secondary_attachment_code INT UNSIGNED NOT NULL DEFAULT 0 COMMENT '副武器配件代码',
    tertiary_weapon INT NOT NULL DEFAULT 0 COMMENT '第三武器类型',
    tertiary_attachment_code INT UNSIGNED NOT NULL DEFAULT 0 COMMENT '第三武器配件代码'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='装备配置表';

-- ============================================================
-- 生命周期表
-- 存储玩家每次生命周期的数据
-- ============================================================
CREATE TABLE IF NOT EXISTS lives (
    life_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '生命周期唯一标识符',
    session_id BIGINT COMMENT '关联的会话ID',
    role INT NOT NULL DEFAULT 0 COMMENT '角色类型',
    shots INT NOT NULL DEFAULT 0 COMMENT '射击次数',
    time FLOAT NOT NULL DEFAULT 0 COMMENT '生存时间',
    loadout_id BIGINT COMMENT '关联的装备配置ID',
    death_kill_id BIGINT COMMENT '死亡时的击杀记录ID',
    FOREIGN KEY (session_id) REFERENCES sessions(session_id) ON DELETE CASCADE,
    FOREIGN KEY (loadout_id) REFERENCES loadouts(loadout_id) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='生命周期表';

-- ============================================================
-- 击杀表
-- 存储击杀记录
-- ============================================================
CREATE TABLE IF NOT EXISTS kills (
    kill_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '击杀记录唯一标识符',
    life_id BIGINT COMMENT '关联的生命周期ID',
    time FLOAT NOT NULL DEFAULT 0 COMMENT '击杀时间',
    hitbox INT NOT NULL DEFAULT 0 COMMENT '命中部位: 0=躯干, 1=头部, 2=四肢',
    weapon INT NOT NULL DEFAULT 0 COMMENT '使用的武器类型',
    attachment_code INT UNSIGNED NOT NULL DEFAULT 0 COMMENT '武器配件代码',
    FOREIGN KEY (life_id) REFERENCES lives(life_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='击杀记录表';

-- ============================================================
-- 命中表
-- 存储命中/受击记录
-- ============================================================
CREATE TABLE IF NOT EXISTS hits (
    hit_id BIGINT AUTO_INCREMENT PRIMARY KEY COMMENT '命中记录唯一标识符',
    life_id BIGINT COMMENT '关联的生命周期ID',
    hit_type ENUM('delt', 'received') NOT NULL COMMENT '命中类型: delt=造成伤害, received=受到伤害',
    health TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '命中时的生命值',
    damage TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '造成的伤害',
    hitbox TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '命中部位',
    weapon TINYINT UNSIGNED NOT NULL DEFAULT 0 COMMENT '使用的武器',
    FOREIGN KEY (life_id) REFERENCES lives(life_id) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci COMMENT='命中记录表';

-- ============================================================
-- 添加外键约束（生命周期表的死亡击杀引用）
-- ============================================================
-- 注意：此外键需要在kills表创建后添加
ALTER TABLE lives ADD CONSTRAINT fk_death_kill 
    FOREIGN KEY (death_kill_id) REFERENCES kills(kill_id) ON DELETE SET NULL;

-- ============================================================
-- 创建索引以优化查询性能
-- ============================================================

-- 会话表索引
CREATE INDEX idx_sessions_tracking ON sessions(tracking_id);
CREATE INDEX idx_sessions_round ON sessions(round_id);
CREATE INDEX idx_sessions_connect_time ON sessions(connect_time);

-- 生命周期表索引
CREATE INDEX idx_lives_session ON lives(session_id);
CREATE INDEX idx_lives_loadout ON lives(loadout_id);

-- 击杀表索引
CREATE INDEX idx_kills_life ON kills(life_id);
CREATE INDEX idx_kills_weapon ON kills(weapon);

-- 命中表索引
CREATE INDEX idx_hits_life ON hits(life_id);
CREATE INDEX idx_hits_type ON hits(hit_type);

-- 排行榜索引
CREATE INDEX idx_leader_board_kills ON leader_board(total_kills DESC);
CREATE INDEX idx_leader_board_killstreak ON leader_board(highest_killstreak DESC);
CREATE INDEX idx_leader_board_play_time ON leader_board(total_play_time DESC);

-- 排名表索引
CREATE INDEX idx_ranks_rating ON ranks(rating DESC);
CREATE INDEX idx_ranks_state ON ranks(state);

-- 经验表索引
CREATE INDEX idx_experiences_tier ON experiences(tier DESC, stage DESC, level DESC);

-- ============================================================
-- 视图：排行榜综合视图
-- ============================================================
CREATE OR REPLACE VIEW v_leaderboard_full AS
SELECT 
    lb.user_id,
    u.tracking_id,
    lb.total_kills,
    lb.highest_killstreak,
    lb.killstreak_tag,
    lb.total_play_time,
    r.state AS rank_state,
    r.rating AS rank_rating,
    e.value AS exp_value,
    e.level AS exp_level,
    e.stage AS exp_stage,
    e.tier AS exp_tier
FROM leader_board lb
LEFT JOIN users u ON lb.user_id = u.user_id
LEFT JOIN ranks r ON lb.user_id = r.user_id
LEFT JOIN experiences e ON lb.user_id = e.user_id;

-- ============================================================
-- 存储过程：清理旧数据
-- ============================================================
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS cleanup_old_sessions(IN days_old INT)
BEGIN
    -- 删除指定天数之前的会话及其关联数据
    DELETE FROM sessions 
    WHERE disconnect_time < DATE_SUB(NOW(), INTERVAL days_old DAY);
    
    -- 清理孤立的回合记录
    DELETE FROM rounds 
    WHERE round_id NOT IN (SELECT DISTINCT round_id FROM sessions WHERE round_id IS NOT NULL);
    
    -- 清理孤立的追踪记录
    DELETE FROM tracking 
    WHERE tracking_id NOT IN (SELECT DISTINCT tracking_id FROM sessions WHERE tracking_id IS NOT NULL)
    AND tracking_id NOT IN (SELECT DISTINCT tracking_id FROM users WHERE tracking_id IS NOT NULL);
    
    -- 清理孤立的装备记录
    DELETE FROM loadouts 
    WHERE loadout_id NOT IN (SELECT DISTINCT loadout_id FROM lives WHERE loadout_id IS NOT NULL);
END //
DELIMITER ;

-- ============================================================
-- 存储过程：重建排行榜
-- ============================================================
DELIMITER //
CREATE PROCEDURE IF NOT EXISTS rebuild_leaderboard(IN begin_epoch DATETIME, IN end_epoch DATETIME)
BEGIN
    -- 清空现有排行榜数据
    TRUNCATE TABLE leader_board;
    
    -- 重新计算并插入排行榜数据
    INSERT INTO leader_board (user_id, total_kills, highest_killstreak, killstreak_tag, total_play_time)
    SELECT 
        u.user_id,
        COALESCE(SUM(kill_count), 0) AS total_kills,
        COALESCE(MAX(ks_count), 0) AS highest_killstreak,
        '' AS killstreak_tag,
        COALESCE(SUM(play_time), 0) AS total_play_time
    FROM users u
    LEFT JOIN (
        SELECT 
            s.tracking_id,
            COUNT(k.kill_id) AS kill_count,
            0 AS ks_count,
            TIMESTAMPDIFF(SECOND, s.connect_time, s.disconnect_time) AS play_time
        FROM sessions s
        LEFT JOIN lives l ON s.session_id = l.session_id
        LEFT JOIN kills k ON l.life_id = k.life_id
        WHERE s.connect_time BETWEEN begin_epoch AND end_epoch
        GROUP BY s.session_id
    ) stats ON u.tracking_id = stats.tracking_id
    GROUP BY u.user_id;
END //
DELIMITER ;

-- ============================================================
-- 完成
-- ============================================================
SELECT '数据库表结构创建完成！' AS message;
