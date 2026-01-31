using PlayerStatsSystem;
using LabApi.Features.Wrappers;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Glicko2;
using CommandSystem;
using System.ComponentModel;
using static TheRiptide.Translation;
using RemoteAdmin;

namespace TheRiptide
{
    /// <summary>
    /// 排名系统配置类
    /// </summary>
    public class RankConfig
    {
        /// <summary>
        /// 是否启用排名系统
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        public string BadgeFormat { get; set; } = "段位: {name}\n";
        public string LeaderBoardFormat { get; set; } = "{tag}";

        /// <summary>
        /// 禁止追踪玩家显示名称
        /// </summary>
        public string DntName { get; set; } = "禁止追踪";
        public string DntTag { get; set; } = "DNT";
        public string DntColor { get; set; } = "nickel";

        /// <summary>
        /// 玩家初始为未排名状态，未排名玩家不会影响定级中/已排名玩家，达到最低经验要求后进入定级赛
        /// </summary>
        [Description("玩家初始为未排名状态，未排名玩家不会影响定级中/已排名玩家，达到最低经验要求后进入定级赛")]
        public string UnrankedName { get; set; } = "未排名";
        public string UnrankedTag { get; set; } = "----";
        public string UnrankedColor { get; set; } = "nickel";
        
        /// <summary>
        /// 进入定级赛所需的最低经验，数值从0开始，例如 level 0 = L:1, level 1 = L:2
        /// </summary>
        [Description("进入定级赛所需的最低经验，数值从0开始，例如 level 0 = L:1, level 1 = L:2")]
        public Experiences.XP MinXpForPlacement { get; set; } = new Experiences.XP { value = 0, level = 0, stage = 1, tier = 0 };

        /// <summary>
        /// 定级中玩家的排名会受到其他定级中玩家和已排名玩家的影响，但已排名玩家不会受到定级中玩家的影响
        /// </summary>
        [Description("定级中玩家的排名会受到其他定级中玩家和已排名玩家的影响，但已排名玩家不会受到定级中玩家的影响")]
        public string PlacementName { get; set; } = "定级中";
        public string PlacementTag { get; set; } = "?";
        public string PlacementColor { get; set; } = "magenta";
        
        /// <summary>
        /// 定级赛场次指与定级中和已排名玩家的击杀/死亡次数，达到此数量后完成定级
        /// </summary>
        [Description("定级赛场次指与定级中和已排名玩家的击杀/死亡次数，达到此数量后完成定级")]
        public int PlacementMatches { get; set; } = 150;

        /// <summary>
        /// 段位列表，必须按评分从低到高排列，颜色必须是有效的服务器组颜色
        /// 所有玩家进入定级赛时初始评分为1500，段位应围绕1500进行设置
        /// </summary>
        [Description("段位列表必须按评分从低到高排列，颜色必须是有效的服务器组颜色。所有玩家进入定级赛时初始评分为1500")]
        public List<RankInfo> Ranks { get; set; } = new List<RankInfo>
        {
            new RankInfo{ Name = "白银 I",                          Tag = "S1",     Rating = -150,     Color = "nickel" },
            new RankInfo{ Name = "白银 II",                         Tag = "S2",     Rating = 0,        Color = "nickel" },
            new RankInfo{ Name = "白银 III",                        Tag = "S3",     Rating = 150,      Color = "nickel" },
            new RankInfo{ Name = "白银 IV",                         Tag = "S4",     Rating = 300,      Color = "nickel" },
            new RankInfo{ Name = "白银精英",                         Tag = "SE",     Rating = 550,      Color = "silver" },
            new RankInfo{ Name = "白银至尊精英",                      Tag = "SEM",    Rating = 700,      Color = "silver" },
            new RankInfo{ Name = "黄金新星 I",                       Tag = "GN1",    Rating = 950,      Color = "cyan" },
            new RankInfo{ Name = "黄金新星 II",                      Tag = "GN2",    Rating = 1100,     Color = "cyan" },
            new RankInfo{ Name = "黄金新星 III",                     Tag = "GN3",    Rating = 1350,     Color = "cyan" },
            new RankInfo{ Name = "黄金新星大师",                      Tag = "GNM",    Rating = 1500,     Color = "aqua" },
            new RankInfo{ Name = "大师守卫 I",                       Tag = "MG1",    Rating = 1650,     Color = "blue_green" },
            new RankInfo{ Name = "大师守卫 II",                      Tag = "MG2",    Rating = 1800,     Color = "blue_green" },
            new RankInfo{ Name = "大师守卫精英",                      Tag = "MGE",    Rating = 1950,     Color = "emerald" },
            new RankInfo{ Name = "卓越大师守卫",                      Tag = "DMG",    Rating = 2100,     Color = "mint" },
            new RankInfo{ Name = "传奇之鹰",                         Tag = "LE",     Rating = 2250,     Color = "yellow" },
            new RankInfo{ Name = "传奇之鹰大师",                      Tag = "LEM",    Rating = 2400,     Color = "yellow" },
            new RankInfo{ Name = "至尊大师首席",                      Tag = "SMFC",   Rating = 2550,     Color = "orange" },
            new RankInfo{ Name = "全球精英",                         Tag = "GE",     Rating = 2700,     Color = "crimson" },
        };

        public List<PlayerPermissions> RankCmdPermissions { get; set; } = new List<PlayerPermissions>()
        {
            PlayerPermissions.ServerConsoleCommands
        };
    }

    /// <summary>
    /// 段位信息类
    /// </summary>
    public class RankInfo
    {
        public string Name { get; set; }
        public string Tag { get; set; }
        /// <summary>
        /// min rating for rank
        /// </summary>
        public float Rating { get; set; }
        public string Color { get; set; }
    }

    public class Ranks
    {
        private const float Rating = 1500.0f;
        private const float RatingDeviation = 350.0f;
        private const float RatingVolatility = 0.06f;

        public static Ranks Singleton { get; private set; }
        public RankConfig config;

        private Dictionary<int, Database.Rank> player_ranks = new Dictionary<int, Database.Rank>();
        private Dictionary<string, GlickoPlayer> player_glikco = new Dictionary<string, GlickoPlayer>();
        private Dictionary<string, List<GlickoOpponent>> player_matches = new Dictionary<string, List<GlickoOpponent>>();

        public Ranks()
        {
            Singleton = this;
        }

        public void Init(RankConfig config)
        {
            this.config = config;
        }

        public void MapGenerated()
        {
            player_ranks.Clear();
            player_glikco.Clear();
            player_matches.Clear();
        }

        void OnPlayerJoined(Player player)
        {
            if (!player.ReferenceHub.authManager.DoNotTrack)
            {
                int id = player.ReferenceHub.PlayerId;
                if (!player_ranks.ContainsKey(id))
                {
                    var duplicates = player_ranks.Where((r) => r.Value.UserId == player.UserId);
                    foreach (var d in duplicates.ToList())
                        player_ranks.Remove(d.Key);
                    player_ranks.Add(id, new Database.Rank { UserId = player.UserId });
                    Database.Singleton.LoadRank(player);
                }
            }
            else
            {
                BadgeOverride.Singleton.SetBadge(player, 0, config.BadgeFormat.Replace("{name}", config.DntName));
                BadgeOverride.Singleton.SetBadgeColor(player, config.DntColor);
            }
        }

        void OnPlayerDeath(Player victim, Player killer, DamageHandlerBase damage)
        {
            if (killer != null && victim != killer && player_glikco.ContainsKey(victim.UserId) && player_glikco.ContainsKey(killer.UserId))
            {
                Database.Rank victim_rank = player_ranks[victim.PlayerId];
                Database.Rank killer_rank = player_ranks[killer.PlayerId];

                //if (!(victim_rank.state == Database.RankState.Ranked && killer_rank.state == Database.RankState.Placement))
                //    player_matches[victim.UserId].Add(new GlickoOpponent(player_glikco[killer.UserId], 0));
                //if (!(killer_rank.state == Database.RankState.Ranked && victim_rank.state == Database.RankState.Placement))
                //    player_matches[killer.UserId].Add(new GlickoOpponent(player_glikco[victim.UserId], 1));

                if (victim_rank.state == Database.RankState.Placement || killer_rank.state == Database.RankState.Ranked)//(ranked/placement) kills placement || ranked kills ranked
                    player_matches[victim.UserId].Add(new GlickoOpponent(player_glikco[killer.UserId], 0));//-1 on victim
                if (killer_rank.state == Database.RankState.Placement || victim_rank.state == Database.RankState.Ranked)//(ranked/placement) kills ranked    || placement kills placement
                    player_matches[killer.UserId].Add(new GlickoOpponent(player_glikco[victim.UserId], 1));//+1 on killer

                if (victim_rank.state == Database.RankState.Placement)
                    victim_rank.placement_matches++;
                if (killer_rank.state == Database.RankState.Placement)
                    killer_rank.placement_matches++;
            }
        }

        public void RankLoaded(Player player)
        {
            Database.Rank rank = player_ranks[player.ReferenceHub.PlayerId];
            if (rank.state != Database.RankState.Unranked && !player_glikco.ContainsKey(player.UserId))
            {
                player_glikco.Add(player.UserId, new GlickoPlayer(rank.rating, rank.rd, rank.rv));
                player_matches.Add(player.UserId, new List<GlickoOpponent>());
            }
            SetBadge(player, rank);
            ShowRankHint(player, rank, 10.0f);
            HintOverride.Refresh(player);
        }

        public void CalculateAndSaveRanks()
        {
            foreach (var id in player_ranks.Keys.ToList())
            {
                try
                {
                    Database.Rank rank = player_ranks[id];
                    Player player = null;
                    try { player = Player.Get(id); }
                    catch (Exception) { }
                    if (rank.state == Database.RankState.Unranked)
                    {
                        if (player != null)
                        {
                            Experiences.XP xp = Experiences.Singleton.GetXP(player);
                            Experiences.XP min = config.MinXpForPlacement;
                            if (xp.tier >= min.tier && xp.stage >= min.stage && xp.level >= min.level && xp.value >= min.value)
                            {
                                rank.state = Database.RankState.Placement;
                                rank.rating = Rating;
                                rank.rd = RatingDeviation;
                                rank.rv = RatingVolatility;
                            }
                        }
                    }
                    if (rank.state == Database.RankState.Placement)
                    {
                        if (rank.placement_matches >= config.PlacementMatches)
                            rank.state = Database.RankState.Ranked;
                    }
                    if (player_glikco.ContainsKey(rank.UserId))
                    {
                        try
                        {
                            GlickoPlayer new_rank = GlickoCalculator.CalculateRanking(new GlickoPlayer(rank.rating, rank.rd, rank.rv), player_matches[rank.UserId]);
                            rank.rating = (float)new_rank.Rating;
                            rank.rd = (float)new_rank.RatingDeviation;
                            rank.rv = (float)new_rank.Volatility;
                        }
                        catch (Exception ex)
                        {
                            Logger.Error("glicko error: " + ex.ToString());
                        }
                    }
                    Database.Singleton.SaveRank(rank);
                    if (player != null)
                    {
                        ShowRankHint(player, rank, 30.0f);
                        SetBadge(player, rank);
                    }
                }
                catch(System.Exception ex)
                {
                    Logger.Error("rank save error: " + ex.ToString());
                }
            }
        }

        public bool SetRank(Player player, float rating)
        {
            if (player_ranks.ContainsKey(player.ReferenceHub.PlayerId))
            {
                player_ranks[player.ReferenceHub.PlayerId].rating = rating;
                return true;
            }
            return false;
        }

        public bool ResetRank(Player player)
        {
            if (player_ranks.ContainsKey(player.ReferenceHub.PlayerId))
            {
                player_ranks[player.ReferenceHub.PlayerId].rating = Rating;
                player_ranks[player.ReferenceHub.PlayerId].rd = RatingDeviation;
                player_ranks[player.ReferenceHub.PlayerId].rv = RatingVolatility;
                return true;
            }
            return false;
        }

        public void DeleteAllRanks()
        {
            Database.Singleton.Async((db)=>
            {
                var ranks = db.GetCollection<Database.Rank>("ranks");
                ranks.DeleteAll();
            });
        }

        public void NormalizeRanks()
        {
            double total = 0.0f;
            int count = 0;
            Database.Singleton.Async((db) =>
            {
                var ranks = db.GetCollection<Database.Rank>("ranks");
                ranks.EnsureIndex(x => x.UserId);
                foreach (var rank in ranks.FindAll())
                {
                    if(rank.state != Database.RankState.Unranked)
                    {
                        total += rank.rating;
                        count++;
                    }
                }
                float delta = (float)(Rating - (total / count));
                foreach (var rank in ranks.FindAll())
                {
                    if(rank.state  != Database.RankState.Unranked)
                    {
                        rank.rating += delta;
                        ranks.Upsert(rank);
                    }
                }
            });
        }

        public Database.Rank GetRank(Player player)
        {
            return player_ranks[player.ReferenceHub.PlayerId];
        }

        public RankInfo GetInfo(Database.Rank rank)
        {
            RankInfo info = new RankInfo { Name = config.UnrankedName, Tag = config.UnrankedTag, Color = config.UnrankedColor };
            if (rank.state == Database.RankState.Placement)
            {
                info.Name = config.PlacementName;
                info.Tag = config.PlacementTag;
                info.Color = config.PlacementColor;
            }
            else if (rank.state == Database.RankState.Ranked)
            {
                int index = config.Ranks.Count - 1;
                while (index > 0 && rank.rating < config.Ranks[index].Rating)
                    index--;

                info.Name = config.Ranks[index].Name;
                info.Tag = config.Ranks[index].Tag;
                info.Color = config.Ranks[index].Color;
            }
            return info;
        }

        private void SetBadge(Player player, Database.Rank rank)
        {
            RankInfo info = GetInfo(rank);
            BadgeOverride.Singleton.SetBadge(player, 0, config.BadgeFormat.Replace("{name}", info.Name));
            BadgeOverride.Singleton.SetBadgeColor(player, info.Color);
        }

        private void ShowRankHint(Player player, Database.Rank rank, float duration)
        {
            RankInfo info = GetInfo(rank);
            if (!BadgeOverride.ColorNameToHex.ContainsKey(info.Color))
            {
                Logger.Error("invalid rank color: " + info.Color);
                return;
            }
            string rank_hint = translation.RankMsg.Replace("{color}", BadgeOverride.ColorNameToHex[info.Color]).Replace("{rank}", config.BadgeFormat.Replace("{name}", info.Name));
            HintOverride.Add(player, 0, rank_hint, duration);
        }

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class DmNormalizeRanks : ICommand
        {
            public bool SanitizeResponse => false;

            public string Command { get; } = "dm_normalize_ranks";

            public string[] Aliases { get; } = new string[] { "dmnr" };

            public string Description { get; } = "normalizes all ranks to be around 1500. usage: dm_normalize_ranks then soft restart";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is PlayerCommandSender sender1 && !sender1.CheckPermission(Singleton.config.RankCmdPermissions.ToArray(), out response))
                    return false;

                Singleton.NormalizeRanks();
                response = "success";
                return true;
            }
        }

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class DmDeleteAllRanks : ICommand
        {
            public bool SanitizeResponse => false;

            public string Command { get; } = "dm_delete_all_ranks";

            public string[] Aliases { get; } = new string[] { "dmdar" };

            public string Description { get; } = "deletes all player ranks. usage: dm_delete_all_ranks";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is PlayerCommandSender sender1 && !sender1.CheckPermission(Singleton.config.RankCmdPermissions.ToArray(), out response))
                    return false;

                Singleton.DeleteAllRanks();
                response = "success";
                return true;
            }
        }

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class DmResetRank : ICommand
        {
            public bool SanitizeResponse => false;

            public string Command { get; } = "dm_reset_rank";

            public string[] Aliases { get; } = new string[] { "dmrr" };

            public string Description { get; } = "reset players rating. usage: dm_reset_rank [playerid]";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is PlayerCommandSender sender1 && !sender1.CheckPermission(Singleton.config.RankCmdPermissions.ToArray(), out response))
                    return false;

                int id;
                Player target = null;
                if (!int.TryParse(arguments.ElementAt(0), out id) || !Player.TryGet(id, out target))
                {
                    response = "failed - invalid id: " + arguments.ElementAt(0);
                    return false;
                }

                if (Singleton.ResetRank(target))
                    response = "success";
                else
                    response = "failed - player does not have a rank yet";
                return true;
            }
        }

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class DmSetRank : ICommand
        {
            public bool SanitizeResponse => false;

            public string Command { get; } = "dm_set_rank";

            public string[] Aliases { get; } = new string[] { "dsr"};

            public string Description { get; } = "set players rating. usage: dm_set_rank [playerid] [rating]";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is PlayerCommandSender sender1 && !sender1.CheckPermission(Singleton.config.RankCmdPermissions.ToArray(), out response))
                    return false;

                if (arguments.Count != 2)
                {
                    response = "usage: dm_set_rank [playerid] [rating]";
                    return false;
                }

                int id;
                Player target = null;
                if (!int.TryParse(arguments.ElementAt(0), out id) || !Player.TryGet(id, out target))
                {
                    response = "failed - invalid id: " + arguments.ElementAt(0);
                    return false;
                }

                int rating;
                if (!int.TryParse(arguments.ElementAt(1), out rating))
                {
                    response = "failed - invalid rating: " + arguments.ElementAt(1);
                    return false;
                }

                if (Singleton.SetRank(target, rating))
                    response = "success";
                else
                    response = "failed - player does not have a rank yet";
                return true;
            }
        }

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        [CommandHandler(typeof(GameConsoleCommandHandler))]
        public class DmGetRank : ICommand
        {
            public bool SanitizeResponse => false;

            public string Command { get; } = "dm_get_rank";

            public string[] Aliases { get; } = new string[] { "dmgr" };

            public string Description { get; } = "get players rating. usage: dm_get_rank [player_id]";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is PlayerCommandSender sender1 && !sender1.CheckPermission(Singleton.config.RankCmdPermissions.ToArray(), out response))
                    return false;

                if (arguments.Count != 1)
                {
                    response = "usage: dm_get_rank [playerid]";
                    return false;
                }

                int id;
                Player target = null;
                if (!int.TryParse(arguments.ElementAt(0), out id) || !Player.TryGet(id, out target))
                {
                    response = "failed - invalid id: " + arguments.ElementAt(0);
                    return false;
                }

                if (Singleton.player_ranks.ContainsKey(target.PlayerId))
                    response = target.Nickname + " rating is " + Singleton.GetRank(target).rating.ToString("0");
                else
                    response = "failed - player does not have a rank yet";
                return true;
            }
        }

        [CommandHandler(typeof(RemoteAdminCommandHandler))]
        public class DmSetRankState : ICommand
        {
            public bool SanitizeResponse => false;

            public string Command { get; } = "dm_set_rank_state";

            public string[] Aliases { get; } = new string[] { "dmrs" };

            public string Description { get; } = "set players rank state. usage: dm_set_rank_state [player_id] [state], states: 0 = Unranked 1 = Placement 2 = Ranked";

            public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
            {
                if (sender is PlayerCommandSender sender1 && !sender1.CheckPermission(Singleton.config.RankCmdPermissions.ToArray(), out response))
                    return false;

                if(arguments.Count != 2)
                {
                    response = "usage: dm_set_rank_state [playerid] [state], states: 0 = Unranked 1 = Placement 2 = Ranked";
                    return false;
                }

                int id;
                Player target = null;
                if (!int.TryParse(arguments.ElementAt(0), out id) || !Player.TryGet(id, out target))
                {
                    response = "failed - invalid id: " + arguments.ElementAt(0);
                    return false;
                }

                int state;
                if (!int.TryParse(arguments.ElementAt(1), out state))
                {
                    response = "failed - invalid state: " + arguments.ElementAt(1);
                    return false;
                }

                if (!Enum.IsDefined(typeof(Database.RankState), state))
                {
                    response = "failed - invalid state: valid states are 0 = Unranked 1 = Placement 2 = Ranked";
                    return false;
                }

                Singleton.GetRank(target).state = (Database.RankState)state;
                response = "success";
                return true;
            }
        }
    }
}
