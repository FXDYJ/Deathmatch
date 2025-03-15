using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using PluginAPI.Core;
using PluginAPI.Core.Attributes;
using PluginAPI.Enums;
using MEC;
using PlayerRoles;
using UnityEngine;

namespace TheRiptide
{
    public class Database
    {
        //configs collection
        class Config
        {
            public string UserId { get; set; }
            public ItemType primary { get; set; } = ItemType.None;
            public ItemType secondary { get; set; } = ItemType.None;
            public ItemType tertiary { get; set; } = ItemType.None;
            public bool rage_enabled { get; set; } = false;
            public RoleTypeId role { get; set; } = RoleTypeId.ClassD;
            public string killstreak_mode { get; set; } = "";
        }

        //users collection
        public class Hit
        {
            public long HitId { get; set; }
            public byte health { get; set; } = 0;
            public byte damage { get; set; } = 0;
            public byte hitbox { get; set; } = 0;
            public byte weapon { get; set; } = 0;
        }

        public class Loadout : System.IEquatable<Loadout>
        {
            public long LoadoutId { get; set; }
            public string killstreak_mode { get; set; } = "";
            public ItemType primary { get; set; } = ItemType.None;
            public uint primary_attachment_code { get; set; } = 0;
            public ItemType secondary { get; set; } = ItemType.None;
            public uint secondary_attachment_code { get; set; } = 0;
            public ItemType tertiary { get; set; } = ItemType.None;
            public uint tertiary_attachment_code { get; set; } = 0;

            public bool Equals(Loadout other)
            {
                return killstreak_mode == other.killstreak_mode &&
                    primary == other.primary &&
                    primary_attachment_code == other.primary_attachment_code &&
                    secondary == other.secondary &&
                    secondary_attachment_code == other.secondary_attachment_code &&
                    tertiary == other.tertiary &&
                    tertiary_attachment_code == other.tertiary_attachment_code;
            }
        }

        public class Kill
        {
            public long KillId { get; set; }
            public float time { get; set; } = UnityEngine.Time.time;
            public HitboxType hitbox { get; set; } = HitboxType.Body;
            public ItemType weapon { get; set; } = ItemType.None;
            public uint attachment_code { get; set; } = 0;
        }

        public class Life
        {
            public long LifeId { get; set; }
            public RoleTypeId role { get; set; } = RoleTypeId.ClassD;
            public int shots { get; set; } = 0;
            public float time { get; set; } = UnityEngine.Time.time;
            public Loadout loadout { get; set; } = null;
            public List<Kill> kills { get; set; } = new List<Kill>();
            public List<Hit> delt { get; set; } = new List<Hit>();
            public List<Hit> received { get; set; } = new List<Hit>();
            public Kill death { get; set; } = null;
        }

        public class Round
        {
            public long RoundId { get; set; }
            public System.DateTime start { get; set; } = System.DateTime.Now;
            public System.DateTime end { get; set; } = System.DateTime.Now;
            public int max_players { get; set; } = 0;
        }

        public class Session
        {
            public long SessionId { get; set; }
            public string nickname { get; set; } = "*unconnected";
            public System.DateTime connect { get; set; } = System.DateTime.Now;
            public System.DateTime disconnect { get; set; } = System.DateTime.Now;
            public Round round { get; set; } = null;
            public List<Life> lives { get; set; } = new List<Life>();
        }

        public class Tracking
        {
            public long TrackingId { get; set; }
            public List<Session> sessions { get; set; } = new List<Session>();
        }

        public class User
        {
            public string UserId { get; set; }
            public Tracking tracking { get; set; } = new Tracking();
        }

        //ranks collection
        public enum RankState { Unranked, Placement, Ranked };
        public class Rank
        {
            public string UserId { get; set; }
            public RankState state { get; set; } = RankState.Unranked;
            public int placement_matches { get; set; } = 0;
            public float rating { get; set; } = 0;
            public float rd { get; set; } = 0;
            public float rv { get; set; } = 0;
        }

        //experience collection
        public class Experience
        {
            public string UserId { get; set; }
            public int value { get; set; } = 0;
            public int level { get; set; } = 0;
            public int stage { get; set; } = 0;
            public int tier { get; set; } = 0;
        }

        //leader board collection
        public class LeaderBoard
        {
            public string UserId { get; set; }
            public int total_kills { get; set; } = 0;
            public int highest_killstreak { get; set; } = 0;
            public string killstreak_tag { get; set; } = "";
            public int total_play_time { get; set; } = 0;
        }

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

        private MySqlConnection db;
        public MySqlConnection DB { get { return db; } }

        private Database() { }

        public void Load(string config_path)
        {
            string connectionString = "server=localhost;user=root;database=deathmatch;port=3306;password=your_password";
            db = new MySqlConnection(connectionString);
            db.Open();
        }

        public void UnLoad()
        {
            db.Close();
        }

        public void Checkpoint()
        {
            // No equivalent method in MySQL, so this is a no-op
        }

        public void LoadConfig(Player player)
        {
            DbDelayedAsync(() =>
            {
                Loadouts.Loadout loadout = Loadouts.GetLoadout(player);
                Lobby.Spawn spawn = Lobby.Singleton.GetSpawn(player);
                Killstreaks.Killstreak killstreak = Killstreaks.GetKillstreak(player);

                string query = $"SELECT * FROM configs WHERE UserId = '{player.UserId}'";
                MySqlCommand cmd = new MySqlCommand(query, db);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Timing.CallDelayed(0.0f, () =>
                    {
                        loadout.primary = (ItemType)reader.GetInt32("primary");
                        loadout.secondary = (ItemType)reader.GetInt32("secondary");
                        loadout.tertiary = (ItemType)reader.GetInt32("tertiary");
                        loadout.rage_mode_enabled = reader.GetBoolean("rage_enabled");
                        spawn.role = (RoleTypeId)reader.GetInt32("role");
                        killstreak.name = reader.GetString("killstreak_mode");
                        Killstreaks.Singleton.KillstreakLoaded(player);
                    });
                }
                reader.Close();
            });
        }

        class ConfigRef
        {
            public Loadouts.Loadout loadout = null;
            public Lobby.Spawn spawn = null;
            public Killstreaks.Killstreak killstreak = null;

            public bool IsReady { get { return loadout != null && spawn != null && killstreak != null; } }
        }

        Dictionary<int, ConfigRef> config_cache = new Dictionary<int, ConfigRef>();

        public void SaveConfigLoadout(Player player)
        {
            Loadouts.Loadout loadout = Loadouts.GetLoadout(player);
            if (!config_cache.ContainsKey(player.PlayerId))
                config_cache.Add(player.PlayerId, new ConfigRef { loadout = loadout });
            else
                config_cache[player.PlayerId].loadout = loadout;
            if (config_cache[player.PlayerId].IsReady)
                SaveConfig(player);
        }

        public void SaveConfigSpawn(Player player)
        {
            Lobby.Spawn spawn = Lobby.Singleton.GetSpawn(player);
            if (!config_cache.ContainsKey(player.PlayerId))
                config_cache.Add(player.PlayerId, new ConfigRef { spawn = spawn });
            else
                config_cache[player.PlayerId].spawn = spawn;
            if (config_cache[player.PlayerId].IsReady)
                SaveConfig(player);
        }

        public void SaveConfigKillstreak(Player player)
        {
            Killstreaks.Killstreak killstreak = Killstreaks.GetKillstreak(player);
            if (!config_cache.ContainsKey(player.PlayerId))
                config_cache.Add(player.PlayerId, new ConfigRef { killstreak = killstreak });
            else
                config_cache[player.PlayerId].killstreak = killstreak;
            if (config_cache[player.PlayerId].IsReady)
                SaveConfig(player);
        }

        private void SaveConfig(Player player)
        {
            ConfigRef config_ref = config_cache[player.PlayerId];
            config_cache.Remove(player.PlayerId);
            Loadouts.Loadout loadout = config_ref.loadout;
            Lobby.Spawn spawn = config_ref.spawn;
            Killstreaks.Killstreak killstreak = config_ref.killstreak;

            DbAsync(() =>
            {
                string query = $"REPLACE INTO configs (UserId, primary, secondary, tertiary, rage_enabled, role, killstreak_mode) VALUES ('{player.UserId}', {(int)loadout.primary}, {(int)loadout.secondary}, {(int)loadout.tertiary}, {loadout.rage_mode_enabled}, {(int)spawn.role}, '{killstreak.name}')";
                MySqlCommand cmd = new MySqlCommand(query, db);
                cmd.ExecuteNonQuery();
            });
        }

        public void LoadRank(Player player)
        {
            DbDelayedAsync(() =>
            {
                Rank player_rank = Ranks.Singleton.GetRank(player);
                string query = $"SELECT * FROM ranks WHERE UserId = '{player.UserId}'";
                MySqlCommand cmd = new MySqlCommand(query, db);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Timing.CallDelayed(0.0f, () =>
                    {
                        try
                        {
                            player_rank.UserId = reader.GetString("UserId");
                            player_rank.state = (RankState)reader.GetInt32("state");
                            player_rank.placement_matches = reader.GetInt32("placement_matches");
                            player_rank.rating = reader.GetFloat("rating");
                            player_rank.rd = reader.GetFloat("rd");
                            player_rank.rv = reader.GetFloat("rv");
                            Ranks.Singleton.RankLoaded(player);
                        }
                        catch (System.Exception ex)
                        {
                            Log.Error("database rank error: " + ex.ToString());
                        }
                    });
                }
                reader.Close();
            });
        }

        public void SaveRank(Rank rank)
        {
            DbAsync(() =>
            {
                string query = $"REPLACE INTO ranks (UserId, state, placement_matches, rating, rd, rv) VALUES ('{rank.UserId}', {(int)rank.state}, {rank.placement_matches}, {rank.rating}, {rank.rd}, {rank.rv})";
                MySqlCommand cmd = new MySqlCommand(query, db);
                cmd.ExecuteNonQuery();
            });
        }

        public void LoadExperience(Player player)
        {
            DbDelayedAsync(() =>
            {
                Experiences.XP player_xp = Experiences.Singleton.GetXP(player);
                string query = $"SELECT * FROM experiences WHERE UserId = '{player.UserId}'";
                MySqlCommand cmd = new MySqlCommand(query, db);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    Timing.CallDelayed(0.0f, () =>
                    {
                        try
                        {
                            player_xp.value = reader.GetInt32("value");
                            player_xp.level = reader.GetInt32("level");
                            player_xp.stage = reader.GetInt32("stage");
                            player_xp.tier = reader.GetInt32("tier");
                            Experiences.Singleton.XpLoaded(player);
                        }
                        catch(System.Exception ex)
                        {
                            Log.Error("database experience error: " + ex.ToString());
                        }
                    });
                }
                reader.Close();
            });
        }

        public void SaveExperience(Player player)
        {
            Experiences.XP player_xp = Experiences.Singleton.GetXP(player);
            DbAsync(() =>
            {
                string query = $"REPLACE INTO experiences (UserId, value, level, stage, tier) VALUES ('{player.UserId}', {player_xp.value}, {player_xp.level}, {player_xp.stage}, {player_xp.tier})";
                MySqlCommand cmd = new MySqlCommand(query, db);
                cmd.ExecuteNonQuery();
            });
        }

        public void SaveTrackingSession(Player player)
        {
            Session session = TheRiptide.Tracking.Singleton.GetSession(player);
            DbAsync(() =>
            {
                foreach(Life life in session.lives)
                {
                    foreach (Hit hit in life.delt)
                    {
                        string query = $"REPLACE INTO hits (HitId, health, damage, hitbox, weapon) VALUES ({hit.HitId}, {hit.health}, {hit.damage}, {hit.hitbox}, {hit.weapon})";
                        MySqlCommand cmd = new MySqlCommand(query, db);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (Hit hit in life.received)
                    {
                        string query = $"REPLACE INTO hits (HitId, health, damage, hitbox, weapon) VALUES ({hit.HitId}, {hit.health}, {hit.damage}, {hit.hitbox}, {hit.weapon})";
                        MySqlCommand cmd = new MySqlCommand(query, db);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (Kill kill in life.kills)
                    {
                        string query = $"REPLACE INTO kills (KillId, time, hitbox, weapon, attachment_code) VALUES ({kill.KillId}, {kill.time}, {(int)kill.hitbox}, {(int)kill.weapon}, {kill.attachment_code})";
                        MySqlCommand cmd = new MySqlCommand(query, db);
                        cmd.ExecuteNonQuery();
                    }

                    if (life.death != null)
                    {
                        string query = $"REPLACE INTO kills (KillId, time, hitbox, weapon, attachment_code) VALUES ({life.death.KillId}, {life.death.time}, {(int)life.death.hitbox}, {(int)life.death.weapon}, {life.death.attachment_code})";
                        MySqlCommand cmd = new MySqlCommand(query, db);
                        cmd.ExecuteNonQuery();
                    }

                    if (life.loadout != null)
                    {
                        string query = $"REPLACE INTO loadouts (LoadoutId, killstreak_mode, primary, primary_attachment_code, secondary, secondary_attachment_code, tertiary, tertiary_attachment_code) VALUES ({life.loadout.LoadoutId}, '{life.loadout.killstreak_mode}', {(int)life.loadout.primary}, {life.loadout.primary_attachment_code}, {(int)life.loadout.secondary}, {life.loadout.secondary_attachment_code}, {(int)life.loadout.tertiary}, {life.loadout.tertiary_attachment_code})";
                        MySqlCommand cmd = new MySqlCommand(query, db);
                        cmd.ExecuteNonQuery();
                    }

                    string lifeQuery = $"REPLACE INTO lives (LifeId, role, shots, time, loadout) VALUES ({life.LifeId}, {(int)life.role}, {life.shots}, {life.time}, {life.loadout?.LoadoutId})";
                    MySqlCommand lifeCmd = new MySqlCommand(lifeQuery, db);
                    lifeCmd.ExecuteNonQuery();
                }

                if (session.round != null)
                {
                    string roundQuery = $"REPLACE INTO rounds (RoundId, start, end, max_players) VALUES ({session.round.RoundId}, '{session.round.start.ToString("yyyy-MM-dd HH:mm:ss")}', '{session.round.end.ToString("yyyy-MM-dd HH:mm:ss")}', {session.round.max_players})";
                    MySqlCommand roundCmd = new MySqlCommand(roundQuery, db);
                    roundCmd.ExecuteNonQuery();
                }

                string sessionQuery = $"REPLACE INTO sessions (SessionId, nickname, connect, disconnect, round) VALUES ({session.SessionId}, '{session.nickname}', '{session.connect.ToString("yyyy-MM-dd HH:mm:ss")}', '{session.disconnect.ToString("yyyy-MM-dd HH:mm:ss")}', {session.round?.RoundId})";
                MySqlCommand sessionCmd = new MySqlCommand(sessionQuery, db);
                sessionCmd.ExecuteNonQuery();

                if (!player.DoNotTrack)
                {
                    string userQuery = $"SELECT * FROM users WHERE UserId = '{player.UserId}'";
                    MySqlCommand userCmd = new MySqlCommand(userQuery, db);
                    MySqlDataReader reader = userCmd.ExecuteReader();
                    User user = null;
                    if (reader.Read())
                    {
                        user = new User { UserId = reader.GetString("UserId") };
                        user.tracking.TrackingId = reader.GetInt64("TrackingId");
                    }
                    reader.Close();

                    if (user == null)
                        user = new User { UserId = player.UserId };

                    user.tracking.sessions.Add(session);
                    string trackingQuery = $"REPLACE INTO tracking (TrackingId) VALUES ({user.tracking.TrackingId})";
                    MySqlCommand trackingCmd = new MySqlCommand(trackingQuery, db);
                    trackingCmd.ExecuteNonQuery();

                    string userInsertQuery = $"REPLACE INTO users (UserId, TrackingId) VALUES ('{user.UserId}', {user.tracking.TrackingId})";
                    MySqlCommand userInsertCmd = new MySqlCommand(userInsertQuery, db);
                    userInsertCmd.ExecuteNonQuery();
                }
                else
                {
                    Tracking player_tracking = new Tracking();
                    player_tracking.sessions.Add(session);
                    string trackingQuery = $"REPLACE INTO tracking (TrackingId) VALUES ({player_tracking.TrackingId})";
                    MySqlCommand trackingCmd = new MySqlCommand(trackingQuery, db);
                    trackingCmd.ExecuteNonQuery();
                }
            });
        }

        public void UpdateLeaderBoard(Player player)
        {
            Session session = TheRiptide.Tracking.Singleton.GetSession(player);
            DbAsync(() =>
            {
                if (TheRiptide.LeaderBoard.Singleton.config.BeginEpoch < session.connect && 
                    session.connect < TheRiptide.LeaderBoard.Singleton.config.EndEpoch)
                {
                    string query = $"SELECT * FROM leader_board WHERE UserId = '{player.UserId}'";
                    MySqlCommand cmd = new MySqlCommand(query, db);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    LeaderBoard lb = null;
                    if (reader.Read())
                    {
                        lb = new LeaderBoard
                        {
                            UserId = reader.GetString("UserId"),
                            total_play_time = reader.GetInt32("total_play_time"),
                            total_kills = reader.GetInt32("total_kills"),
                            highest_killstreak = reader.GetInt32("highest_killstreak"),
                            killstreak_tag = reader.GetString("killstreak_tag")
                        };
                    }
                    reader.Close();

                    if (lb == null)
                        lb = new LeaderBoard { UserId = player.UserId };

                    lb.total_play_time += Mathf.CeilToInt((float)(session.disconnect - session.connect).TotalSeconds);
                    foreach(var life in session.lives)
                    {
                        int ks = 0;
                        foreach(var kill in life.kills)
                        {
                            if(life.death == null || kill != life.death)
                            {
                                lb.total_kills++;
                                ks++;
                            }
                        }
                        if(ks > lb.highest_killstreak)
                        {
                            lb.highest_killstreak = ks;
                            lb.killstreak_tag = life.loadout.killstreak_mode;
                        }
                    }

                    string updateQuery = $"REPLACE INTO leader_board (UserId, total_play_time, total_kills, highest_killstreak, killstreak_tag) VALUES ('{lb.UserId}', {lb.total_play_time}, {lb.total_kills}, {lb.highest_killstreak}, '{lb.killstreak_tag}')";
                    MySqlCommand updateCmd = new MySqlCommand(updateQuery, db);
                    updateCmd.ExecuteNonQuery();
                }
            });
        }

        public void DeleteData(string user_id)
        {
            DbAsync(() =>
            {
                string[] tables = { "users", "experiences", "ranks", "configs", "leader_board" };
                foreach (string table in tables)
                {
                    string query = $"DELETE FROM {table} WHERE UserId = '{user_id}'";
                    MySqlCommand cmd = new MySqlCommand(query, db);
                    cmd.ExecuteNonQuery();
                }
            });
        }

        public void DeleteData(Player player)
        {
            DeleteData(player.UserId);
        }

        public void Async(System.Action<MySqlConnection> action)
        {
            new Task(() =>
            {
                try
                {
                    lock (db)
                    {
                        action.Invoke(db);
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("Database error: " + ex.ToString());
                }
            }).Start();
        }

        private void DbAsync(System.Action action)
        {
            new Task(() =>
            {
                try
                {
                    lock (db)
                    {
                        action.Invoke();
                    }
                }
                catch (System.Exception ex)
                {
                    Log.Error("Database error: " + ex.ToString());
                }
            }).Start();
        }

        private void DbDelayedAsync(System.Action action)
        {
            Timing.CallDelayed(0.0f, () =>
            {
                new Task(() =>
                {
                    try
                    {
                        lock (db)
                        {
                            action.Invoke();
                        }
                    }
                    catch (System.Exception ex)
                    {
                        Log.Error("Database error: " + ex.ToString());
                    }
                }).Start();
            });
        }
    }
}
