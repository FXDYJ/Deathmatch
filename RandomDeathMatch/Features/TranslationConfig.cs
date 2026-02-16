using CommandSystem;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TheRiptide
{
    /// <summary>
    /// 翻译配置类 - 包含所有游戏内文本的简体中文翻译
    /// </summary>
    public class TranslationConfig
    {
        #region 主要提示

        /// <summary>
        /// DNT（禁止追踪）警告消息
        /// </summary>
        [Description("DNT警告消息")]
        public string DntMsg { get; set; } = "<size=64><b><color=#FF0000>警告！您已启用DNT模式，因此本回合获得的任何经验和排名都将丢失！由于您在游戏设置中启用了DNT，您的配置、偏好设置、连杀奖励、统计数据、角色和装备都无法保存。为了公平起见，您与已排名玩家之间的击杀/死亡记录将被忽略。</color></b></size>";

        #endregion

        #region 命中部位

        [Description("命中部位名称")]
        public string Body { get; set; } = "躯干";
        public string Limb { get; set; } = "四肢";
        public string Head { get; set; } = "头部";

        #endregion

        #region 击杀信息

        [Description("击杀消息")]
        public string FirearmKill { get; set; } = "{killer} 用 {gun} 射中了 {victim} 的 {hitbox}";
        public string ExplosionKill { get; set; } = "{killer} 炸死了 {victim}";
        public string ExplosionSelfKill { get; set; } = "{victim} 被自己的 <b><color=#eb0d47>手榴弹</color></b> 炸死了";
        public string JailbirdHeadKill { get; set; } = "{killer} 用 <b><color=#eb0d47>囚鸟</color></b> 砸中了 {victim} 的 {hitbox}";
        public string JailbirdNormalKill { get; set; } = "{killer} 用 <b><color=#eb0d47>囚鸟</color></b> 拍中了 {victim} 的 {hitbox}";
        public string Scp018Kill { get; set; } = "{killer} 用 <b><color=#eb0d47>SCP-018</color></b> 砸死了 {victim}";
        public string Scp018SelfKill { get; set; } = "{victim} 因为接不住自己的弹力球而送了命";
        public string DistruptorKill { get; set; } = "{killer} 用 <b><color=#eb0d47>粒子干扰器</color></b> 击中了 {victim} 的 {hitbox} 并将其分解";
        public string DistruptorSelfKill { get; set; } = "{victim} 被自己的 <b><color=#eb0d47>粒子干扰器</color></b> 分解了";
        public string CustomReasonKill { get; set; } = "{victim} 被击杀: <b><color=#43BFF0>{reason}</color></b>";
        public string FailedFirstGrade { get; set; } = "{victim} <b><color=#eb0d47>因为不识字而离开了比赛</color></b>";
        public string SelfKill { get; set; } = "<b><color=#eb0d47>{victim}</color></b> 自杀了";

        #endregion

        #region 连杀系统

        [Description("连杀消息")]
        public string GlobalKillstreak { get; set; } = "<b>{killstreak} <color=#43BFF0>{name}</color></b> 达成了 <b><color=#FF0000>{count}</color></b> 连杀";
        public string PrivateKillstreak { get; set; } = "连杀数 <b><color=#FF0000>{count}</color></b>";
        public string GlobalKillstreakEnded { get; set; } = "<b>{killer_killstreak} <color=#43BFF0>{killer}</color></b> 终结了 <b>{victim_killstreak} <color=#43BFF0>{victim}</color></b> 的 <b><color=#FF0000>{count}</color></b> 连杀";

        #endregion

        #region 装备系统

        [Description("装备提示")]
        public string CustomisationHint { get; set; } = "<b>查看物品栏！<color=#FF0000>右键点击O5卡选择武器</color></b>";
        public List<string> CustomisationDenied { get; set; } = new List<string>() {
            "<color=#f8d107>射击/使用物品后无法自定义装备</color>",
            "<color=#43BFF0>请等待下次重生</color>" };
        public string LastWeapon { get; set; } = "<color=#f8d107>不能删除最后一把武器！</color>";
        public string WeaponBanned { get; set; } = "<color=#FF0000>{weapon} 目前已被禁用</color>";

        #endregion

        #region 大厅系统

        [Description("大厅提示")]
        public string Teleport { get; set; } = "<color=#43BFF0>选择武器后将被传送</color>";
        public List<string> WaitingForPlayers { get; set; } = new List<string>(){
            "<color=#43BFF0>等待1名玩家加入</color>",
            "<color=#43BFF0>您可以选择起始区域！</color>"};
        public string Respawn { get; set; } = "<b><color=#FFFF00>左键/右键点击重生</color></b>";
        public string Attachments { get; set; } = "<b><color=#FF0000>按Tab键编辑配件/预设</color></b>";
        public string Teleporting { get; set; } = "<color=#43BFF0>7秒后传送</color>";
        public string TeleportCancel { get; set; } = "<color=#43BFF0>打开[主菜单]取消传送</color> - <color=#FF0000>右键点击O5卡</color>";
        public string FastTeleport { get; set; } = "<color=#43BFF0>装备已设置，3秒后传送</color>";
        public string SpectatorMode { get; set; } = "观察SCP-939即可重生，如果出现问题可能需要退出后重新加入";

        #endregion

        #region 主菜单

        [Description("主菜单")]
        public string MainMenu { get; set; } = "<b><color=#43BFF0>[主菜单]</color></b> <b><color=#FF0000>右键点击选择</color></b>";
        public string BackToMainMenu { get; set; } = "<color=#5d318c>[O5卡]</color> = <b>返回<color=#43BFF0>[主菜单]</color></b>";
        public string SaveAndExit { get; set; } = "<color=#5d318c>[O5卡]</color> = <b><color=#5d318c>保存并退出</color></b>";
        public string CustomiseLoadout { get; set; } = "<color=#e7d77b>[科学家]</color> = <b><color=#FF0000>自定义装备 - </color><color=#43BFF0>[武器槽位]</color></b>";
        public string KillstreakRewardSystem { get; set; } = "<color=#e1ab21>[研究主管]</color> = <b><color=#43BFF0>[连杀奖励系统]</color></b>";
        public string Role { get; set; } = "<color=#bd8f86>[收容工程师]</color> = <b><color=#43BFF0>[角色]</color></b>";
        public string Preferences { get; set; } = "<color=#bd1a4a>[设施经理]</color> = <b><color=#43BFF0>[偏好设置]</color></b>";

        #endregion

        #region 武器槽位菜单

        [Description("武器槽位菜单")]
        public string GunSlotMenu { get; set; } = "<b><color=#43BFF0>[武器槽位]</color></b> <b><color=#FF0000>右键点击选择</color></b>";
        public string Primary { get; set; } = "<color=#bd1a4a>[设施经理]</color> = <b><color=#FF0000>主武器 - </color><color=#43BFF0>[武器类型]</color></b>";
        public string Secondary { get; set; } = "<color=#217b7b>[区域管理员]</color> = <b>副武器 - <color=#43BFF0>[武器类型]</color></b>";
        public string HeavyPrimary { get; set; } = "<color=#1b43cb>[队长]</color> = <b><color=#FF0000>主武器 - </color><color=#43BFF0>[武器类型]</color></b>";
        public string HeavySecondary { get; set; } = "<color=#177dde>[中士]</color> = <b>副武器 - <color=#43BFF0>[武器类型]</color></b>";
        public string HeavyTertiary { get; set; } = "<color=#accfe1>[列兵]</color> = <b>第三武器 - <color=#43BFF0>[武器类型]</color></b>";

        #endregion

        #region 武器类型菜单

        [Description("武器类型菜单")]
        public string GunClassMenu { get; set; } = "<b><color=#43BFF0>[武器类型]</color></b> <b><color=#FF0000>右键点击选择</color></b>";
        public string MtfGuns { get; set; } = "<color=#1b43cb>[队长]</color> = <b><color=#1b43cb>[MTF武器]</color></b>";
        public string ChaosGuns { get; set; } = "<color=#008f1c>[混沌]</color> = <b><color=#008f1c>[混沌武器]</color></b>";

        #endregion

        #region 武器菜单

        [Description("武器菜单")]
        public string MtfGunMenu { get; set; } = "<b><color=#1b43cb>[MTF武器]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";
        public string ChaosGunMenu { get; set; } = "<b><color=#008f1c>[混沌武器]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";
        public string GunSelected { get; set; } = "<color=#43BFF0>{gun}</color> 已添加到您的装备作为 <color=#FF0000>{slot}</color> 武器";

        #endregion

        #region 连杀奖励系统菜单

        [Description("连杀奖励系统菜单")]
        public string KillstreakRewardMenu { get; set; } = "<b><color=#43BFF0>[连杀奖励系统]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";
        public string KillstreakSelected { get; set; } = "{killstreak} 已选为您的连杀奖励系统";
        public string CurrentKillstreakSelected { get; set; } = "当前选择的连杀奖励系统: {killstreak}</color>";

        #endregion

        #region 角色菜单

        [Description("角色菜单")]
        public string RoleMenu { get; set; } = "<b><color=#43BFF0>[角色]</color></b> <b><color=#FF0000>右键点击选择</color></b>";
        public string ClassD { get; set; } = "<color=#bdafe4>[清洁工]</color> = <b><color=#FF8E00>D级人员</color></b>";
        public string Scientist { get; set; } = "<color=#e7d77b>[科学家]</color> = <b><color=#FFFF7C>科学家</color></b>";
        public string Guard { get; set; } = "<color=#5B6370>[警卫]</color> = <b><color=#5B6370>设施警卫</color></b>";
        public string Private { get; set; } = "<color=#accfe1>[列兵]</color> = <b><color=#accfe1>NTF列兵</color></b>";
        public string Sergeant { get; set; } = "<color=#177dde>[中士]</color> = <b><color=#177dde>NTF中士</color></b>";
        public string Captain { get; set; } = "<color=#1b43cb>[队长]</color> = <b><color=#1b43cb>NTF队长</color></b>";
        public string Chaos { get; set; } = "<color=#008f1c>[混沌]</color> = <b><color=#008f1c>混沌分裂者</color></b>";
        public string RoleSelected { get; set; } = "{role} 已选为角色";

        #endregion

        #region 偏好设置菜单

        [Description("偏好设置菜单")]
        public string PreferencesMenu { get; set; } = "<b><color=#43BFF0>[偏好设置]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";
        public string Stats { get; set; } = "<color=#eb0d47>[科学家]</color> = <b><color=#43BFF0>[统计数据]</color></b>";
        public string Spectator { get; set; } = "<color=#eb0d47>[手电筒]</color> = <b>启用观察者模式</b>";
        public string EnableRage { get; set; } = "<color=#eb0d47>[硬币]</color> = <b>启用[数据删除]模式</b>";
        public string DeleteData { get; set; } = "<color=#eb0d47>[清洁工]</color> = <b>删除数据 统计/配置/排名/经验/偏好设置（无法撤销）</b>";
        public string LeaderBoard { get; set; } = "<color=#eb0d47>[SCP-1576]</color> = <b><color=#43BFF0>[排行榜]</color></b>";

        #endregion

        #region 统计数据菜单

        [Description("统计数据菜单")]
        public string StatsMenu { get; set; } = "<b><color=#43BFF0>[统计数据]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";
        public string BackToPreferences { get; set; } = "<color=#5d318c>[O5卡]</color> = <b>返回<color=#43BFF0>[偏好设置]</color></b>";

        #endregion

        #region 确认删除数据菜单

        [Description("确认删除数据菜单")]
        public string DeleteDataMenu { get; set; } = "<b><color=#43BFF0>[数据删除确认]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";
        public string AreYouSure { get; set; } = "<b>您确定吗？您必须开启「禁止追踪」才能执行此操作。<color=#FF0000>删除后无法恢复。</color> <color=#43BFF0>如果确定，请右键点击清洁工钥匙卡删除您的数据</color></b>";
        public string FailedToDeleteData { get; set; } = "<b>删除数据失败，您必须在设置中开启「禁止追踪」</b>";
        public string DeletedData { get; set; } = "数据已删除";

        #endregion

        #region 排行榜菜单

        [Description("排行榜菜单")]
        public string LeaderBoardMenu { get; set; } = "<b><color=#43BFF0>[排行榜]</color></b> - <b><color=#FF0000>右键点击选择</color></b>";

        #endregion

        #region 房间系统

        [Description("房间系统")]
        public string SecondPlayerJoined { get; set; } = "玩家 <color=#43BFF0>{name}</color> 已加入，正在等待他选择装备";
        public string SecondPlayerHelp { get; set; } = "<color=#43BFF0>{name}</color> 正在设置装备中，请用对讲机帮助他";
        public string Decontaminating { get; set; } = "<color=#FF0000>正在清除！</color>";
        public string Caution { get; set; } = "<color=#FFFF00>注意！房间将在 {time} 后清除</color>";
        public string Warning { get; set; } = "<color=#FF8000>警告！房间将在 {time} 后清除</color>";
        public string Danger { get; set; } = "<color=#FF0000>危险！即将清除！{time}</color>";

        #endregion

        #region 统计数据

        [Description("统计数据显示")]
        public string DeathMsgKiller { get; set; } = "\n\n\n{killer} <color=#43BFF0>生命值: {health}</color>";
        public string DeathMsgAhp { get; set; } = " <color=#008f1c>护甲: {ahp}</color>";
        public string DeathMsgDamageReduction { get; set; } = " <color=#5900ff> 伤害减免: {reduction}%</color>";
        public string DeathMsgBodyshotReduction { get; set; } = " <color=#e7d77b> 躯干伤害减免: {reduction}%</color>";
        public string DeathMsgDamageDelt { get; set; } = "\n<color=#43BFF0>造成 - 伤害: {damage}</color> <color=#FF0000>爆头: {head_shots}</color> <color=#36a832>躯干: {body_shots}</color> <color=#43BFF0>四肢: {limb_shots}</color>";
        public string DeathMsgDamageTaken { get; set; } = "\n<color=#43BFF0>承受 - 伤害: {damage}</color> <color=#FF0000>爆头: {head_shots}</color> <color=#36a832>躯干: {body_shots}</color> <color=#43BFF0>四肢: {limb_shots}</color>";
        public string DeathMsgDamageOther { get; set; } = " 其他: {other_hits}";
        public string PlayerStatsLine1 { get; set; } = "<color=#76b8b5>击杀:</color> <color=#FF0000>{kills}</color>    <color=#76b8b5>死亡:</color> <color=#FF0000>{deaths}</color>    <color=#76b8b5>K/D:</color> <color=#FF0000>{kd}</color>    <color=#76b8b5>最高连杀:</color> <color=#FF0000>{top_ks}</color>    <color=#76b8b5>得分:</color> <color=#FF0000>{score}</color>";
        public string PlayerStatsLine2 { get; set; } = "<color=#76b8b5>爆头击杀:</color> <color=#FF0000>{hsk}%</color>    <color=#76b8b5>爆头率:</color> <color=#FF0000>{hs}%</color>    <color=#76b8b5>命中率:</color> <color=#FF0000>{accuracy}%</color>    <color=#76b8b5>造成伤害:</color> <color=#FF0000>{dmg_delt}</color>    <color=#76b8b5>承受伤害:</color> <color=#FF0000>{dmg_taken}</color>";
        public string HighestKillstreak { get; set; } = "<b><color=#43BFF0>{name}</color></b> <color=#d4af37>达成了最高连杀</color> <b><color=#FF0000>{streak}</color></b>";
        public string HighestKills { get; set; } = "<b><color=#43BFF0>{name}</color></b> <color=#c0c0c0>获得了最多击杀</color> <b><color=#FF0000>{kills}</color></b>";
        public string HighestScore { get; set; } = "<b><color=#43BFF0>{name}</color></b> <color=#a97142>是本局最佳玩家，得分</color> <b><color=#FF0000>{score}</color></b>";

        #endregion

        #region 经验系统

        [Description("经验奖励")]
        public string RewardXpKill { get; set; } = "击杀玩家获得 {xp} 经验";
        public string RewardXpMinute { get; set; } = "游玩 {time} 分钟获得 {xp} 经验";
        public string RewardXp100Damage { get; set; } = "造成100点伤害获得 {xp} 经验";
        public string RewardXp500Damage { get; set; } = "造成500点伤害获得 {xp} 经验";
        public string RewardXp2500Damage { get; set; } = "造成2500点伤害获得 {xp} 经验";
        public string RewardXp10000Damage { get; set; } = "造成10000点伤害获得 {xp} 经验";
        public string RewardXp5Killstreak { get; set; } = "达成5连杀获得 {xp} 经验";
        public string RewardXp10Killstreak { get; set; } = "达成10连杀获得 {xp} 经验";
        public string RewardXp15Killstreak { get; set; } = "达成15连杀获得 {xp} 经验";
        public string RewardXp20Killstreak { get; set; } = "达成20连杀获得 {xp} 经验";
        public string RewardXp25Killstreak { get; set; } = "达成25连杀获得 {xp} 经验";
        public string RewardXpRoundStart { get; set; } = "回合开始奖励获得 {xp} 经验";
        public string RewardXpItemUsed { get; set; } = "使用 {item} 获得 {xp} 经验";
        public string RewardXpItemThrown { get; set; } = "投掷 {item} 获得 {xp} 经验";
        public string XpMsg { get; set; } = "<align=center><voffset=2em><b><size=48>{xp}</size></b>\n";
        public string XpGainedMsg { get; set; } = "<align=center><voffset=2em><b><size=48>本回合您获得了 {xp} 经验！</size></b>\n";

        #endregion

        #region 排名系统

        [Description("排名显示")]
        public string RankMsg { get; set; } = "<align=center><voffset=1.5em><b><size=72><color={color}>{rank}</color></size></b>\n";

        #endregion

        #region 配件黑名单

        [Description("配件黑名单")]
        public string AttachmentBanned { get; set; } = "<color=#FF0000>配件 {attachment} 已被禁用</color>";

        #endregion

        #region 语音聊天

        [Description("语音聊天模式")]
        public string GlobalTalkGlobalReceive { get; set; } = "<color=#43BFF0>语音聊天: 全局发言 全局接收</color>";
        public string ProximityTalkGlobalReceive { get; set; } = "<color=#43BFF0>语音聊天:</color> <color=#FF0000>近距离发言</color> <color=#43BFF0>全局接收</color>";
        public string ProximityTalkProximityReceive { get; set; } = "<color=#43BFF0>语音聊天:</color> <color=#FF0000>近距离发言 近距离接收</color>";

        #endregion

        #region 排行榜

        [Description("排行榜")]
        public string LeaderBoardTitle { get; set; } = "<color=#d4af37><b><size=128>排行榜</size></b></color>";
        public string LeaderBoardControl { get; set; } = "\n<size=24><b><color=#FF0000>操作说明: 使用移动键控制页面/菜单滚动 左/右 = 类型, 前/后 = 翻页. 注意：靠墙时无法使用</color></b></size>";

        public string LedgendPos { get; set; } = "排名";
        public string LedgendName { get; set; } = "名称";
        public string LedgendRank { get; set; } = "段位";
        public string LedgendExperience { get; set; } = "经验";
        public string LedgendKillstreak { get; set; } = "连杀";
        public string LedgendKills { get; set; } = "击杀";
        public string LedgendTime { get; set; } = "时间";

        public string PageAndLine { get; set; } = "第 {page} 页 / 共 {page_max} 页 [{line_start} - {line_end}]/{line_max}";

        #endregion

        #region 回合系统

        [Description("回合提示")]
        public string RoundEnd5Minutes { get; set; } = "<color=#43BFF0>回合将在5分钟后结束</color>";
        public string RoundEnd1Minute { get; set; } = "<color=#43BFF0>回合将在1分钟后结束</color>";

        #endregion
    }

    /// <summary>
    /// 翻译静态类
    /// </summary>
    public static class Translation
    {
        public static TranslationConfig translation = null;
    }
}
