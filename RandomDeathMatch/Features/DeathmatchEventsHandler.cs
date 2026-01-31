using System;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features.Console;
using LabApi.Features.Wrappers;
using PlayerRoles;
using PlayerStatsSystem;
using UnityEngine;
using static TheRiptide.Translation;
using static RoundSummary;
using LightContainmentZoneDecontamination;
using Scp914;

namespace TheRiptide
{
    /// <summary>
    /// 死亡竞赛事件处理器
    /// </summary>
    public class DeathmatchEventsHandler : CustomEventsHandler
    {
        #region 服务器事件

        /// <summary>
        /// 地图生成完成事件
        /// </summary>
        public override void OnMapGenerated()
        {
            try
            {
                FacilityManager.MapGenerated();
                Lobby.Singleton.MapGenerated();
                if (Deathmatch.Singleton.rank_config.IsEnabled)
                    Ranks.Singleton.MapGenerated();
                if (Deathmatch.Singleton.leader_board_config.IsEnabled)
                    LeaderBoard.Singleton.MapGenerated();
            }
            catch (Exception ex)
            {
                Logger.Error($"地图生成事件处理错误: {ex}");
            }
        }

        /// <summary>
        /// 等待玩家事件
        /// </summary>
        public override void OnWaitingForPlayers()
        {
            try
            {
                GenerateGlobalReferenceConfig();
                DmRound.Singleton.WaitingForPlayers();
                if (Deathmatch.Singleton.tracking_config.IsEnabled)
                    Tracking.Singleton.WaitingForPlayers();
                if (Deathmatch.Singleton.voice_chat_config.IsEnabled)
                    VoiceChat.Singleton.WaitingForPlayers();
            }
            catch (Exception ex)
            {
                Logger.Error($"等待玩家事件处理错误: {ex}");
            }
        }

        /// <summary>
        /// 回合结束事件
        /// </summary>
        public override void OnRoundEnded(RoundEndedEventArgs ev)
        {
            try
            {
                DmRound.Singleton.RoundEnd();
            }
            catch (Exception ex)
            {
                Logger.Error($"回合结束事件处理错误: {ex}");
            }
        }

        /// <summary>
        /// 回合重启事件
        /// </summary>
        public override void OnRoundRestarting()
        {
            try
            {
                DmRound.Singleton.RoundRestart();
                FacilityManager.RoundRestart();
                Rooms.Singleton.RoundRestart();
                if (Deathmatch.Singleton.cleanup_config.IsEnabled)
                    Cleanup.Singleton.RoundRestart();
            }
            catch (Exception ex)
            {
                Logger.Error($"回合重启事件处理错误: {ex}");
            }
        }

        #endregion

        #region 玩家事件

        /// <summary>
        /// 玩家加入服务器事件
        /// </summary>
        public override void OnPlayerJoined(PlayerJoinedEventArgs ev)
        {
            try
            {
                DmRound.players.Add(ev.Player.ReferenceHub.PlayerId);
                if (!ev.Player.ReferenceHub.authManager.DoNotTrack)
                    Database.Singleton.LoadConfig(ev.Player.ReferenceHub.PlayerId, ev.Player.UserId);
                else
                    MEC.Timing.CallDelayed(1.0f, () => 
                    { 
                        HintOverride.Add(ev.Player.ReferenceHub, 0, translation.DntMsg, 30.0f); 
                        HintOverride.Refresh(ev.Player.ReferenceHub); 
                    });
            }
            catch (Exception ex)
            {
                Logger.Error($"玩家加入事件处理错误: {ex}");
            }
        }

        /// <summary>
        /// 玩家离开服务器事件
        /// </summary>
        public override void OnPlayerLeft(PlayerLeftEventArgs ev)
        {
            try
            {
                DmRound.players.Remove(ev.Player.ReferenceHub.PlayerId);
            }
            catch (Exception ex)
            {
                Logger.Error($"玩家离开事件处理错误: {ex}");
            }
        }

        #endregion

        #region 辅助方法

        /// <summary>
        /// 生成全局参考配置
        /// </summary>
        private void GenerateGlobalReferenceConfig()
        {
            try
            {
                Deathmatch.Singleton.global_reference_config.AllItems.Clear();
                foreach (ItemType item in Enum.GetValues(typeof(ItemType)))
                    Deathmatch.Singleton.global_reference_config.AllItems.Add(item);

                Deathmatch.Singleton.global_reference_config.AllEffects.Clear();
                foreach (CustomPlayerEffects.StatusEffectBase effect in Server.Host.GameObject.GetComponentsInChildren<CustomPlayerEffects.StatusEffectBase>(true))
                    Deathmatch.Singleton.global_reference_config.AllEffects.Add(effect.name);

                Deathmatch.Singleton.global_reference_config.AllAttachments.Clear();
                foreach (InventorySystem.Items.Firearms.Attachments.AttachmentName name in Enum.GetValues(typeof(InventorySystem.Items.Firearms.Attachments.AttachmentName)))
                    Deathmatch.Singleton.global_reference_config.AllAttachments.Add(name);
            }
            catch (Exception ex)
            {
                Logger.Error($"生成全局参考配置时出错，如果此错误频繁出现请删除配置文件\n {ex}");
            }
        }

        #endregion
    }
}
