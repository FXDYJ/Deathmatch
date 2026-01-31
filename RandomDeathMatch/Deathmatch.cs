using System;
using System.Collections.Generic;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Console;
using LabApi.Loader.Features.Plugins;
using PlayerRoles;
using UnityEngine;
using System.ComponentModel;
using CustomPlayerEffects;
using InventorySystem.Items.Firearms.Attachments;
using LightContainmentZoneDecontamination;
using PlayerStatsSystem;
using static TheRiptide.Translation;
using static RoundSummary;

namespace TheRiptide
{
    /// <summary>
    /// 主配置类
    /// </summary>
    public class MainConfig
    {
        /// <summary>
        /// 是否启用插件
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// 回合时间（分钟）
        /// </summary>
        [Description("回合时间（分钟）")]
        public float RoundTime { get; set; } = 20.0f;

        /// <summary>
        /// 回合结束时间（秒）
        /// </summary>
        [Description("回合结束时间（秒）")]
        public float RoundEndTime { get; set; } = 30.0f;

        /// <summary>
        /// 假人玩家名称
        /// </summary>
        public string DummyPlayerName { get; set; } = "[死亡竞赛]";
    }

    /// <summary>
    /// 全局参考配置类
    /// </summary>
    public class GlobalReferenceConfig
    {
        /// <summary>
        /// [自动生成文件] 可能包含不再有效的类型。用于其他配置的类型参考列表（请勿编辑）
        /// </summary>
        [Description("[自动生成文件] 可能包含不再有效的类型。用于其他配置的类型参考列表（请勿编辑）")]
        public List<ItemType> AllItems { get; set; } = new List<ItemType>();
        public List<string> AllEffects { get; set; } = new List<string>();
        public List<AttachmentName> AllAttachments { get; set; } = new List<AttachmentName>();
    }

    /// <summary>
    /// 死亡竞赛主插件类
    /// </summary>
    public class Deathmatch : Plugin
    {
        public static Deathmatch Singleton { get; private set; }

        public override string Name { get; } = "死亡竞赛";
        public override string Description { get; } = "SCP:SL 死亡竞赛游戏模式插件";
        public override string Author { get; } = "The Riptide";
        public override Version Version { get; } = new Version(2, 0, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);

        // 配置文件
        public MainConfig config = new MainConfig();
        public GlobalReferenceConfig global_reference_config = new GlobalReferenceConfig();
        public RoomsConfig rooms_config = new RoomsConfig();
        public KillstreakConfig killstreak_config = new KillstreakConfig();
        public LoadoutConfig loadout_config = new LoadoutConfig();
        public LobbyConfig lobby_config = new LobbyConfig();
        public ExperienceConfig experience_config = new ExperienceConfig();
        public RankConfig rank_config = new RankConfig();
        public TrackingConfig tracking_config = new TrackingConfig();
        public TranslationConfig translation_config = new TranslationConfig();
        public AttachmentBlacklistConfig attachment_blacklist_config = new AttachmentBlacklistConfig();
        public VoiceChatConfig voice_chat_config = new VoiceChatConfig();
        public CleanupConfig cleanup_config = new CleanupConfig();
        public LeaderBoardConfig leader_board_config = new LeaderBoardConfig();

        // 事件处理器
        private DeathmatchEventsHandler eventsHandler;

        public Deathmatch()
        {
            Singleton = this;
        }

        public override void Enable()
        {
            if (!config.IsEnabled)
                return;

            Logger.Info("正在加载死亡竞赛插件...");

            // 加载配置
            LoadConfigs();

            // 初始化数据库
            Database.Singleton.Load(ConfigPath);

            // 注册事件处理器
            eventsHandler = new DeathmatchEventsHandler();
            CustomHandlersManager.RegisterEventsHandler(eventsHandler);

            // 初始化所有功能模块
            BadgeOverride.Singleton.Init(2);

            DmRound.Singleton.Init(config);
            Statistics.Init();
            Rooms.Singleton.Init(rooms_config);
            Killstreaks.Singleton.Init(killstreak_config);
            Loadouts.Singleton.Init(loadout_config);
            Lobby.Singleton.Init(lobby_config);
            if (rank_config.IsEnabled)
                Ranks.Singleton.Init(rank_config);
            if (experience_config.IsEnabled)
                Experiences.Singleton.Init(experience_config);
            if (tracking_config.IsEnabled)
                Tracking.Singleton.Init(tracking_config);
            if (attachment_blacklist_config.IsEnabled)
                AttachmentBlacklist.Singleton.Init(attachment_blacklist_config);
            if (voice_chat_config.IsEnabled)
                VoiceChat.Singleton.Init(voice_chat_config);
            if (cleanup_config.IsEnabled)
                Cleanup.Singleton.Init(cleanup_config);
            if (leader_board_config.IsEnabled)
                LeaderBoard.Singleton.Init(leader_board_config);

            translation = translation_config;
            DeathmatchMenu.Singleton.SetupMenus();

            Logger.Info("死亡竞赛插件加载完成！");
        }

        public override void Disable()
        {
            Logger.Info("正在卸载死亡竞赛插件...");

            Database.Singleton.UnLoad();

            if (eventsHandler != null)
            {
                CustomHandlersManager.UnregisterEventsHandler(eventsHandler);
                eventsHandler = null;
            }

            DeathmatchMenu.Singleton.ClearMenus();

            Logger.Info("死亡竞赛插件已卸载！");
        }

        /// <summary>
        /// 加载所有配置文件
        /// </summary>
        private void LoadConfigs()
        {
            // 这里需要实现配置文件的加载逻辑
            // LabAPI 使用不同的配置系统，需要适配
        }

        /// <summary>
        /// 检查玩家是否有效
        /// </summary>
        public static bool IsPlayerValid(LabApi.Features.Wrappers.Player player)
        {
            return DmRound.players.Contains(player.ReferenceHub.PlayerId);
        }
    }
}
