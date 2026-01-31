using InventorySystem.Items;
using InventorySystem.Items.Firearms;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Radio;
using MapGeneration;
using Interactables.Interobjects;
using Mirror;
using MEC;
using PlayerRoles;
using PlayerStatsSystem;
using LabApi.Features.Wrappers;
using LabApi.Features.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Unity.Mathematics;
using static TheRiptide.Utility;
using static TheRiptide.Translation;
using Interactables.Interobjects.DoorUtils;

namespace TheRiptide
{
    /// <summary>
    /// 装备配置类
    /// </summary>
    public class LoadoutConfig
    {
        /// <summary>
        /// 是否启用武器黑名单
        /// </summary>
        public bool IsBlackListEnabled { get; set; } = true;

        /// <summary>
        /// 武器黑名单列表，仅影响菜单中的武器，不影响奖励武器
        /// </summary>
        [Description("在此添加禁用的武器，仅影响菜单中的武器，不影响奖励武器")]
        public List<ItemType> BlackList { get; set; } = new List<ItemType>();
        
        /// <summary>
        /// 所有武器类型列表（修改此项无效）
        /// </summary>
        [Description("所有武器类型列表（修改此项无效）")]
        public List<ItemType> AllWeapons { get; set; } = new List<ItemType>
        {
            ItemType.GunAK,
            ItemType.GunCOM15,
            ItemType.GunCOM18,
            ItemType.GunCom45,
            ItemType.GunCrossvec,
            ItemType.GunE11SR,
            ItemType.GunFSP9,
            ItemType.GunLogicer,
            ItemType.GunRevolver,
            ItemType.GunShotgun
        };
    }

    /// <summary>
    /// 装备管理类
    /// </summary>
    public class Loadouts
    {
        public static Loadouts Singleton { get; private set; }

        LoadoutConfig config;

        /// <summary>
        /// 武器槽位枚举
        /// </summary>
        public enum GunSlot { Primary, Secondary, Tertiary };

        /// <summary>
        /// 装备数据类
        /// </summary>
        public class Loadout
        {
            public ItemType primary = ItemType.None;
            public ItemType secondary = ItemType.None;
            public ItemType tertiary = ItemType.None;
            public GunSlot slot = GunSlot.Primary;

            public bool locked = false;
            public bool customising = false;
            public bool rage_mode_enabled = false;
        }

        public static Dictionary<int, Loadout> player_loadouts = new Dictionary<int, Loadout>();

        public Loadouts()
        {
            Singleton = this;
        }

        public void Init(LoadoutConfig config)
        {
            this.config = config;
        }

        void OnRoundStart()
        {
            Scp330Interobject scp330 = RoomIdentifier.AllRoomIdentifiers.Where((r => r.Name == RoomName.Lcz330)).First().GetComponentInChildren<Scp330Interobject>();
            NetworkServer.UnSpawn(scp330.gameObject);
        }

        void OnPlayerJoined(Player player)
        {
            if (!player_loadouts.ContainsKey(player.ReferenceHub.PlayerId))
                player_loadouts.Add(player.ReferenceHub.PlayerId, new Loadout());
        }

        void OnPlayerLeft(Player player)
        {
            if (player_loadouts.ContainsKey(player.ReferenceHub.PlayerId))
            {
                if (!player.ReferenceHub.authManager.DoNotTrack)
                    Database.Singleton.SaveConfigLoadout(player);
                player_loadouts.Remove(player.ReferenceHub.PlayerId);
            }
        }

        bool OnPlayerDropitem(Player player, ItemBase item)
        {
            bool drop_allowed = false;
            Loadout loadout = player_loadouts[player.ReferenceHub.PlayerId];
            if (InventoryMenu.GetPlayerMenuID(player) == (int)DeathmatchMenu.MenuPage.None)
            {
                if (IsGun(item.ItemTypeId))
                {
                    if (!loadout.locked)
                    {
                        if (item.ItemTypeId == loadout.primary)
                        {
                            loadout.primary = ItemType.None;
                            RemoveItem(player, item.ItemTypeId);
                        }
                        else if (item.ItemTypeId == loadout.secondary)
                        {
                            loadout.secondary = ItemType.None;
                            RemoveItem(player, item.ItemTypeId);
                        }
                        else if (item.ItemTypeId == loadout.tertiary)
                        {
                            loadout.tertiary = ItemType.None;
                            RemoveItem(player, item.ItemTypeId);
                        }

                        if (IsLoadoutEmpty(player))
                        {
                            BroadcastOverride.ClearLines(player, BroadcastPriority.High);
                            BroadcastOverride.BroadcastLine(player, 1, 300, BroadcastPriority.High, translation.CustomisationHint);
                            if (Lobby.Singleton.InSpawn(player))
                            {
                                Lobby.Singleton.CancelTeleport(player);
                                BroadcastOverride.BroadcastLine(player, 2, 300, BroadcastPriority.High, translation.Teleport);
                            }
                        }
                    }
                    else if (!Killstreaks.Singleton.IsLoadoutLocked(player))
                    {
                        BroadcastOverride.ClearLines(player, BroadcastPriority.High);
                        BroadcastOverride.BroadcastLines(player, 1, 3, BroadcastPriority.High, translation.CustomisationDenied);
                    }
                    else
                    {
                        int gun_count = 0;
                        foreach (var i in player.ReferenceHub.inventory.UserInventory.Items.Values)
                            if (IsGun(i.ItemTypeId))
                                gun_count++;
                        if (gun_count >= 2)
                            RemoveItem(player, item.ItemTypeId);
                        else
                            BroadcastOverride.BroadcastLine(player, 1, 3, BroadcastPriority.High, translation.LastWeapon);
                    }
                }
                else if (item.Category != ItemCategory.Armor && loadout.locked)
                    drop_allowed = true;
            }
            BroadcastOverride.UpdateIfDirty(player);
            return drop_allowed;
        }

        bool OnPlayerDropAmmo(Player player, ItemType ammo, int amount)
        {
            return false;
        }

        void OnPlayerShotWeapon(Player player, Firearm firearm)
        {
            player_loadouts[player.ReferenceHub.PlayerId].locked = true;
            RemoveItem(player, ItemType.KeycardO5);
        }

        void OnPlayerUsedItem(Player player, ItemBase item)
        {
            player_loadouts[player.ReferenceHub.PlayerId].locked = true;
            RemoveItem(player, ItemType.KeycardO5);
        }

        bool OnPlayerInteractDoor(Player player, DoorVariant door, bool can_open)
        {
            if (door.ActiveLocks > 0 && !player.IsBypassEnabled)
                return true;

            if (door.AllowInteracting(player.ReferenceHub, 0))
            {
                door.NetworkTargetState = !door.TargetState;
                door._triggerPlayer = player.ReferenceHub;
                switch (door.NetworkTargetState)
                {
                    case false:
                        DoorEvents.TriggerAction(door, DoorAction.Closed, player.ReferenceHub);
                        break;
                    case true:
                        DoorEvents.TriggerAction(door, DoorAction.Opened, player.ReferenceHub);
                        break;
                }
            }
            return false;
        }

        void OnPlayerDying(Player target, Player killer, DamageHandlerBase damage)
        {
            target.ClearInventory();
        }

        void OnPlayerSpawn(Player player, RoleTypeId role)
        {
            if (player == null || !player_loadouts.ContainsKey(player.ReferenceHub.PlayerId))
                return;

            Loadout loadout = player_loadouts[player.ReferenceHub.PlayerId];
            if (config.IsBlackListEnabled)
            {
                if (config.BlackList.Contains(loadout.primary))
                    loadout.primary = ItemType.None;
                if (config.BlackList.Contains(loadout.secondary))
                    loadout.secondary = ItemType.None;
                if (config.BlackList.Contains(loadout.tertiary))
                    loadout.tertiary = ItemType.None;
            }

            if (Lobby.Singleton.GetSpawn(player).role == role)
            {
                loadout.locked = false;

                Timing.CallDelayed(0.0f, () =>
                {
                    if (player == null || !player_loadouts.ContainsKey(player.ReferenceHub.PlayerId))
                        return;
                    player.ClearInventory();
                    AddLoadoutStartItems(player);
                });
            }
        }

        public static bool ValidateLoadout(Player player)
        {
            if (IsLoadoutEmpty(player))
            {
                BroadcastOverride.BroadcastLine(player, 1, 300, BroadcastPriority.High, translation.CustomisationHint);
                return false;
            }
            else
                return true;
        }

        public static Loadout GetLoadout(Player player)
        {
            return player_loadouts[player.ReferenceHub.PlayerId];
        }

        public static bool CustomiseLoadout(Player player)
        {
            Loadout loadout = GetLoadout(player);
            if (!loadout.locked)
            {
                loadout.customising = true;
                Lobby.Singleton.CancelTeleport(player);
                return true;
            }
            else
            {
                BroadcastOverride.ClearLines(player, BroadcastPriority.High);
                BroadcastOverride.BroadcastLines(player, 1, 3, BroadcastPriority.High, translation.CustomisationDenied);
                return false;
            }
        }

        public static bool IsLoadoutEmpty(Player player)
        {
            Loadout loadout = player_loadouts[player.ReferenceHub.PlayerId];
            if (Killstreaks.Singleton.IsLoadoutLocked(player))
                return false;
            ItemType armor = Killstreaks.Singleton.ArmorType(player);
            if (armor == ItemType.None)
                return loadout.primary == ItemType.None;
            else if (armor == ItemType.ArmorLight || armor == ItemType.ArmorCombat)
                return loadout.primary == ItemType.None && loadout.secondary == ItemType.None;
            else
                return loadout.primary == ItemType.None && loadout.secondary == ItemType.None && loadout.tertiary == ItemType.None;
        }

        public static void AddLoadoutStartItems(Player player)
        {
            Loadout loadout = player_loadouts[player.ReferenceHub.PlayerId];
            Killstreaks.Killstreak killstreak = Killstreaks.GetKillstreak(player);

            if (!IsLoadoutEmpty(player))
            {
                ItemType armor = Killstreaks.Singleton.ArmorType(player);
                if (armor != ItemType.None)
                    AddArmor(player, armor, true);

                Killstreaks.Singleton.AddKillstreakStartAmmo(player);
                if (!Killstreaks.Singleton.IsLoadoutLocked(player))
                {
                    AddFirearm(player, loadout.primary, false);
                    if (armor != ItemType.None)
                        AddFirearm(player, loadout.secondary, false);
                    if(armor == ItemType.ArmorHeavy)
                        AddFirearm(player, loadout.tertiary, false);
                }
                Killstreaks.Singleton.AddKillstreakStartItems(player);
                if (!Lobby.Singleton.InSpawn(player) && DmRound.GameStarted)
                    Killstreaks.Singleton.AddKillstreakStartEffects(player);
            }
            player.AddItem(ItemType.KeycardO5);
        }

        public bool SetGun(Player player, ItemType gun)
        {
            Loadout loadout = player_loadouts[player.ReferenceHub.PlayerId];

            if(config.IsBlackListEnabled && config.BlackList.Contains(gun))
            {
                BroadcastOverride.BroadcastLine(player, 2, 5.0f, BroadcastPriority.High, translation.WeaponBanned.Replace("{weapon}", gun.ToString().Substring(3)));
                return false;
            }
            if (loadout.slot == GunSlot.Primary)
                loadout.primary = gun;
            else if (loadout.slot == GunSlot.Secondary)
                loadout.secondary = gun;
            else if (loadout.slot == GunSlot.Tertiary)
                loadout.tertiary = gun;
            return true;
        }
    }
}
