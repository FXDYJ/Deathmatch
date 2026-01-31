using LightContainmentZoneDecontamination;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using LabApi.Features.Wrappers;
using LabApi.Features.Console;
using Scp914;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static TheRiptide.Translation;

namespace TheRiptide
{
    /// <summary>
    /// 死亡竞赛回合管理类
    /// </summary>
    public class DmRound
    {
        public static DmRound Singleton { get; private set; }

        private MainConfig config;

        private static bool game_started = false;
        public static bool game_ended = false;
        public static SortedSet<int> players = new SortedSet<int>();
        public Action OnConfigReloaded;
        private CoroutineHandle restart_handle;
        private CoroutineHandle round_timer_handle;
        private CoroutineHandle round_5_minute_warning;
        private CoroutineHandle round_1_minute_warning;

        /// <summary>
        /// 游戏是否已开始
        /// </summary>
        public static bool GameStarted
        {
            get => game_started;
            set
            {
                if (value == true)
                {
                    foreach (var player in Player.List)
                        if (player.IsAlive)
                            Killstreaks.Singleton.AddKillstreakStartEffects(player.ReferenceHub);
                }
                else
                {
                    foreach (var player in Player.List)
                        if (player.IsAlive)
                            Lobby.ApplyGameNotStartedEffects(player.ReferenceHub);
                }
                game_started = value;
            }
        }

        public DmRound()
        {
            Singleton = this;
            Killfeeds.Init(2, 5, 20);

            OnConfigReloaded = new Action(() =>
            {
                try
                {
                    ServerConsole.FriendlyFire = true;
                    FriendlyFireConfig.PauseDetector = true;
                }
                catch (Exception ex)
                {
                    Logger.Error("配置覆盖错误: " + ex.ToString());
                }
            });
        }

        /// <summary>
        /// 初始化回合管理器
        /// </summary>
        public void Init(MainConfig config)
        {
            this.config = config;
        }

        /// <summary>
        /// 等待玩家时调用
        /// </summary>
        public void WaitingForPlayers()
        {
            game_ended = false;
            Database.Singleton.Checkpoint();
        }

        /// <summary>
        /// 回合开始时调用
        /// </summary>
        public void OnRoundStart()
        {
            if (config.RoundTime > 5.0f)
                round_5_minute_warning = Timing.CallDelayed(60.0f * (config.RoundTime - 5.0f), () => { BroadcastOverride.BroadcastLine(1, 30, BroadcastPriority.Medium, translation.RoundEnd5Minutes); });
            if (config.RoundTime > 1.0f)
                round_1_minute_warning = Timing.CallDelayed(60.0f * (config.RoundTime - 1.0f), () => { BroadcastOverride.BroadcastLine(1, 30, BroadcastPriority.Medium, translation.RoundEnd1Minute); });
            round_timer_handle = Timing.CallDelayed(60.0f * config.RoundTime, () =>
            {
                try
                {
                    restart_handle = Timing.CallDelayed(config.RoundEndTime, () => LabApi.Features.Wrappers.Round.Restart(false));
                    Timing.CallPeriodically(config.RoundEndTime, 0.2f, () =>
                    {
                        foreach (var p in Player.List)
                            p.IsGodModeEnabled = true;
                    });
                    try { Statistics.DisplayRoundStats(); }
                    catch (Exception ex) { Logger.Error(ex.ToString()); }
                    try { Experiences.Singleton.SaveExperiences(); }
                    catch (Exception ex) { Logger.Error(ex.ToString()); }
                    try { Ranks.Singleton.CalculateAndSaveRanks(); }
                    catch (Exception ex) { Logger.Error(ex.ToString()); }
                    HintOverride.Refresh();
                    VoiceChat.Singleton.ForceGlobalTalkGlobalReceive();
                    Server.Host.SetRole(RoleTypeId.Spectator);
                    game_ended = true;
                    Tracking.Singleton.UpdateLeaderBoard();
                    LeaderBoard.Singleton.ReloadLeaderBoard();
                    if (Deathmatch.Singleton.leader_board_config.DisplayEndRoundDelay < config.RoundEndTime)
                    {
                        LeaderBoard.Singleton.EnableTitle = false;
                        Timing.CallDelayed(Deathmatch.Singleton.leader_board_config.DisplayEndRoundDelay, () =>
                        {
                            foreach (var p in Player.List)
                                if (p.IsReady)
                                    LeaderBoard.Singleton.EnableLeaderBoardMode(p.ReferenceHub, Enum.IsDefined(typeof(LeaderBoardType), Deathmatch.Singleton.leader_board_config.LeaderBoardType) ? (LeaderBoardType)Deathmatch.Singleton.leader_board_config.LeaderBoardType : (LeaderBoardType)UnityEngine.Random.Range(0, Enum.GetValues(typeof(LeaderBoardType)).Length));
                        });
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error("回合结束错误: " + ex.ToString());
                }
            });

            Logger.Info($"Server.Host.IpAddress: {Server.Host.IpAddress == null}");
            Server.Host.SetRole(RoleTypeId.Scp939);
            Server.Host.ReferenceHub.nicknameSync.SetNick(config.DummyPlayerName);
            Server.Host.Position = new Vector3(128.8f, 994.0f, 18.0f);
            LabApi.Features.Wrappers.Round.IsLocked = true;
            LabApi.Features.Wrappers.Warhead.IsLocked = true;
            DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;
            AttackerDamageHandler._ffMultiplier = 1.0f;
        }

        /// <summary>
        /// 回合重启时调用
        /// </summary>
        public void RoundRestart()
        {
            Timing.KillCoroutines(round_timer_handle, restart_handle, round_5_minute_warning, round_1_minute_warning);
            LabApi.Features.Wrappers.Round.IsLocked = false;
        }

        /// <summary>
        /// 回合结束时调用
        /// </summary>
        public void RoundEnd()
        {
            Timing.KillCoroutines(round_timer_handle, restart_handle, round_5_minute_warning, round_1_minute_warning);
            LabApi.Features.Wrappers.Round.IsLocked = false;
        }
    }
}
