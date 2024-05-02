﻿using System.Collections.Generic;
using HarmonyLib;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Bundles;
using StardewArchipelago.GameModifications;
using StardewArchipelago.GameModifications.Modded;
using StardewArchipelago.Locations.CodeInjections.Initializers;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Serialization;
using StardewArchipelago.Stardew;
using StardewModdingAPI;

namespace StardewArchipelago.Locations.Patcher
{
    public class LocationPatcher
    {
        private List<ILocationPatcher> _patchers;

        public LocationPatcher(IMonitor monitor, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago, ArchipelagoStateDto state,
            LocationChecker locationChecker, StardewItemManager itemManager, WeaponsManager weaponsManager, BundlesManager bundlesManager,
            SeedShopStockModifier seedShopStockModifier, JunimoShopGenerator junimoShopGenerator, Friends friends)
        {
            CodeInjectionInitializer.Initialize(monitor, modHelper, archipelago, state, locationChecker, itemManager, weaponsManager, bundlesManager, seedShopStockModifier, junimoShopGenerator, friends);
            _patchers = new List<ILocationPatcher>();
            _patchers.Add(new VanillaLocationPatcher(monitor, modHelper, harmony, archipelago, locationChecker, itemManager));
            if (archipelago.SlotData.Mods.IsModded)
            {
                _patchers.Add(new ModLocationPatcher(harmony, modHelper, archipelago));
            }
        }

        public void ReplaceAllLocationsRewardsWithChecks()
        {
            foreach (var patcher in _patchers)
            {
                patcher.ReplaceAllLocationsRewardsWithChecks();
            }
        }

        public void CleanEvents()
        {
            foreach (var patcher in _patchers)
            {
                patcher.CleanEvents();
            }
        }
    }
}
