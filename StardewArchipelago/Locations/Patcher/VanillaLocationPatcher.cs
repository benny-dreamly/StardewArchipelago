﻿using System;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewArchipelago.Archipelago;
using StardewArchipelago.Locations.CodeInjections.Vanilla;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Bundles;
using StardewArchipelago.Locations.CodeInjections.Vanilla.MonsterSlayer;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Quests;
using StardewArchipelago.Locations.CodeInjections.Vanilla.Relationship;
using StardewArchipelago.Locations.Festival;
using StardewArchipelago.Locations.GingerIsland;
using StardewArchipelago.Locations.ShopStockModifiers;
using StardewArchipelago.Stardew;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.GameData.Buildings;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Minigames;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.SpecialOrders;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace StardewArchipelago.Locations.Patcher
{
    public class VanillaLocationPatcher : ILocationPatcher
    {
        private readonly ArchipelagoClient _archipelago;
        private readonly Harmony _harmony;
        private readonly IModHelper _modHelper;
        private readonly GingerIslandPatcher _gingerIslandPatcher;
        private readonly ToolShopStockModifier _toolUpgradesShopStockModifier;
        private readonly FishingRodShopStockModifier _fishingRodShopStockModifier;
        private readonly CarpenterShopStockModifier _carpenterShopStockModifier;
        private readonly CarpenterBuildingsModifier _carpenterBuildingsModifier;
        private readonly AdventureGuildShopStockModifier _guildShopStockModifier;
        private readonly TravelingMerchantShopStockModifier _travelingMerchantShopStockModifier;
        private readonly FestivalShopStockModifier _festivalShopStockModifier;
        private readonly CookingRecipePurchaseStockModifier _cookingRecipePurchaseStockModifier;
        private readonly CraftingRecipePurchaseStockModifier _craftingRecipePurchaseStockModifier;
        private readonly KrobusStockModifier _krobusStockModifier;

        public VanillaLocationPatcher(IMonitor monitor, IModHelper modHelper, Harmony harmony, ArchipelagoClient archipelago, LocationChecker locationChecker, StardewItemManager stardewItemManager)
        {
            _archipelago = archipelago;
            _harmony = harmony;
            _modHelper = modHelper;
            _gingerIslandPatcher = new GingerIslandPatcher(monitor, _modHelper, _harmony, _archipelago, locationChecker);
            _toolUpgradesShopStockModifier = new ToolShopStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _fishingRodShopStockModifier = new FishingRodShopStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _carpenterShopStockModifier = new CarpenterShopStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _carpenterBuildingsModifier = new CarpenterBuildingsModifier(monitor, modHelper, archipelago);
            _guildShopStockModifier = new AdventureGuildShopStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _travelingMerchantShopStockModifier = new TravelingMerchantShopStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _festivalShopStockModifier = new FestivalShopStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _cookingRecipePurchaseStockModifier = new CookingRecipePurchaseStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _craftingRecipePurchaseStockModifier = new CraftingRecipePurchaseStockModifier(monitor, modHelper, archipelago, stardewItemManager);
            _krobusStockModifier = new KrobusStockModifier(monitor, modHelper, archipelago, stardewItemManager);
        }

        public void ReplaceAllLocationsRewardsWithChecks()
        {
            ReplaceCommunityCenterBundlesWithChecks();
            ReplaceCommunityCenterAreasWithChecks();
            ReplaceBackPackUpgradesWithChecks();
            ReplaceMineshaftChestsWithChecks();
            ReplaceElevatorsWithChecks();
            ReplaceToolUpgradesWithChecks();
            PatchFishingRods();
            ReplaceSkillsWithChecks();
            ReplaceQuestsWithChecks();
            PatchCarpenter();
            ReplaceIsolatedEventsWithChecks();
            PatchAdventurerGuildShop();
            ReplaceArcadeMachinesWithChecks();
            PatchTravelingMerchant();
            AddFishsanityLocations();
            AddMuseumsanityLocations();
            PatchFestivals();
            AddCropSanityLocations();
            ReplaceFriendshipsWithChecks();
            ReplaceSpecialOrdersWithChecks();
            ReplaceChildrenWithChecks();
            _gingerIslandPatcher.PatchGingerIslandLocations();
            PatchMonstersanity();
            AddCooksanityLocations();
            PatchChefAndCraftsanity();
            PatchKrobusShop();
            PatchFarmcave();
        }

        public void CleanEvents()
        {
            CleanToolEvents();
            CleanFishingRodEvents();
            CleanCarpenterEvents();
            CleanAdventureGuildEvents();
            CleanTravelingMerchantEvents();
            CleanFestivalEvents();
            CleanChefsanityEvents();
            CleanCraftsanityEvents();
            CleanKrobusEvents();
        }

        private void ReplaceCommunityCenterBundlesWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.checkForRewards)),
                postfix: new HarmonyMethod(typeof(JunimoNoteMenuInjections), nameof(JunimoNoteMenuInjections.CheckForRewards_SendBundleChecks_PostFix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.getRewardNameForArea)),
                prefix: new HarmonyMethod(typeof(JunimoNoteMenuInjections), nameof(JunimoNoteMenuInjections.GetRewardNameForArea_ScoutRoomRewards_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.setUpMenu)),
                postfix: new HarmonyMethod(typeof(JunimoNoteMenuInjections), nameof(JunimoNoteMenuInjections.SetupMenu_AddTextureOverrides_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.receiveLeftClick)),
                prefix: new HarmonyMethod(typeof(JunimoNoteMenuInjections), nameof(JunimoNoteMenuInjections.ReceiveLeftClick_PurchaseCurrencyBundle_Prefix))
            );
            var drawParameters = new[] { typeof(SpriteBatch) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(JunimoNoteMenu), nameof(JunimoNoteMenu.draw), drawParameters),
                postfix: new HarmonyMethod(typeof(JunimoNoteMenuInjections), nameof(JunimoNoteMenuInjections.Draw_AddCurrencyBoxes_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), nameof(CommunityCenter.shouldNoteAppearInArea)),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.ShouldNoteAppearInArea_AllowAccessEverything_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), nameof(CommunityCenter.checkAction)),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.CheckAction_BulletinBoardNoRequirements_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), "checkForMissedRewards"),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.CheckForMissedRewards_DontBother_Prefix))
            );
        }

        private void ReplaceCommunityCenterAreasWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(CommunityCenter), "doAreaCompleteReward"),
                prefix: new HarmonyMethod(typeof(CommunityCenterInjections), nameof(CommunityCenterInjections.DoAreaCompleteReward_AreaLocations_Prefix))
            );
        }

        private void ReplaceBackPackUpgradesWithChecks()
        {
            if (_archipelago.SlotData.BackpackProgression == BackpackProgression.Vanilla)
            {
                return;
            }

            var performActionArgumentTypes = new[] { typeof(string[]), typeof(Farmer), typeof(Location) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), performActionArgumentTypes),
                prefix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.PerformAction_BuyBackpack_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.AnswerDialogueAction_BackPackPurchase_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SeedShop), nameof(SeedShop.draw)),
                prefix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.Draw_SeedShopBackpack_Prefix)),
                postfix: new HarmonyMethod(typeof(BackpackInjections), nameof(BackpackInjections.Draw_SeedShopBackpack_Postfix))
            );
        }

        private void ReplaceMineshaftChestsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.CheckForAction_MineshaftChest_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), "addLevelChests"),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.AddLevelChests_Level120_Prefix))
            );
        }

        private void ReplaceToolUpgradesWithChecks()
        {
            var performActionArgumentTypes = new[] { typeof(string[]), typeof(Farmer), typeof(Location) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), performActionArgumentTypes),
                prefix: new HarmonyMethod(typeof(ScytheInjections), nameof(ScytheInjections.PerformAction_GoldenScythe_Prefix))
            );

            _modHelper.Events.Content.AssetRequested += _toolUpgradesShopStockModifier.OnShopStockRequested;
        }

        private void CleanToolEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _toolUpgradesShopStockModifier.OnShopStockRequested;
        }

        private void PatchFishingRods()
        {
            if (!_archipelago.SlotData.ToolProgression.HasFlag(ToolProgression.Progressive))
            {
                return;
            }

            _modHelper.Events.Content.AssetRequested += _fishingRodShopStockModifier.OnShopStockRequested;

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                prefix: new HarmonyMethod(typeof(FishingRodInjections), nameof(FishingRodInjections.SkipEvent_BambooPole_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(FishingRodInjections), nameof(FishingRodInjections.AwardFestivalPrize_BambooPole_Prefix))
            );
        }

        private void CleanFishingRodEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _fishingRodShopStockModifier.OnShopStockRequested;
        }

        private void ReplaceElevatorsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(MineElevatorMenu), nameof(MineElevatorMenu.draw), new[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.Draw_AddArchipelagoIndicators_Postfix))
            );

            if (_archipelago.SlotData.ElevatorProgression == ElevatorProgression.Vanilla)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.enterMine)),
                postfix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.EnterMine_SendElevatorCheck_PostFix))
            );

            var performActionArgumentTypes = new[] { typeof(string[]), typeof(Farmer), typeof(Location) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), performActionArgumentTypes),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.PerformAction_LoadElevatorMenu_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(MineShaft), nameof(MineShaft.checkAction)),
                prefix: new HarmonyMethod(typeof(MineshaftInjections), nameof(MineshaftInjections.CheckAction_LoadElevatorMenu_Prefix))
            );
        }

        private void ReplaceSkillsWithChecks()
        {
            if (_archipelago.SlotData.SkillProgression == SkillsProgression.Vanilla)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                    prefix: new HarmonyMethod(typeof(SkillInjections), nameof(SkillInjections.GainExperience_NormalExperience_Prefix))
                );
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.gainExperience)),
                prefix: new HarmonyMethod(typeof(SkillInjections), nameof(SkillInjections.GainExperience_ArchipelagoExperience_Prefix))
            );

            if (_archipelago.SlotData.SkillProgression != SkillsProgression.ProgressiveWithMasteries)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(MasteriesInjections), nameof(MasteriesInjections.PerformAction_MasteryCaveInteractions_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MasteryTrackerMenu), "claimReward"),
                prefix: new HarmonyMethod(typeof(MasteriesInjections), nameof(MasteriesInjections.ClaimReward_SendMasteryCheck_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(MasteryTrackerMenu), nameof(MasteryTrackerMenu.hasCompletedAllMasteryPlaques)),
                prefix: new HarmonyMethod(typeof(MasteriesInjections), nameof(MasteriesInjections.HasCompletedAllMasteryPlaques_RelyOnSentChecks_Prefix))
            );
        }

        private void ReplaceQuestsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Railroad), "resetLocalState"),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.ResetLocalState_PlayCutsceneIfConditionsAreMet_Postfix))
            );

            if (!_archipelago.SlotData.QuestLocations.StoryQuestsEnabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.QuestComplete_LocationInsteadOfReward_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.RemoveQuest)),
                postfix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.RemoveQuest_CheckLocation_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.getQuestOfTheDay)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.GetQuestOfTheDay_BalanceQuests_Prefix))
            );

            var performActionArgumentTypes = new[] { typeof(string[]), typeof(Farmer), typeof(Location) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), performActionArgumentTypes),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.PerformAction_MysteriousQiLumberPile_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Bush), "shake"),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.Shake_WinterMysteryBush_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Town), "mgThief_afterSpeech"),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.MgThief_AfterSpeech_WinterMysteryFinished_Prefix))
            );

            PatchSkillsPage();

            _harmony.Patch(
                original: AccessTools.Method(typeof(Object), "getPriceAfterMultipliers"),
                postfix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.GetPriceAfterMultipliers_BearKnowledge_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.AwardFestivalPrize_QiMilk_Prefix))
            );

            ReplaceDarkTalismanQuestsWithChecks();
        }

        private void ReplaceDarkTalismanQuestsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.CheckAction_ShowWizardMagicInk_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.CheckForAction_BuglandChest_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), "performRemoveHenchman"),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.PerformRemoveHenchman_CheckGoblinProblemLocation_Postfix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.setUpLocationSpecificFlair)),
                prefix: new HarmonyMethod(typeof(DarkTalismanInjections), nameof(DarkTalismanInjections.SetUpLocationSpecificFlair_CreateBuglandChest_Prefix))
            );
        }

        private void PatchSkillsPage()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(PowersTab), nameof(PowersTab.populateClickableComponentList)),
                postfix: new HarmonyMethod(typeof(QuestInjections), nameof(QuestInjections.PopulateClickableComponentList_BearKnowledge_Postfix))
            );
        }

        private void ReplaceSpecialOrdersWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.GetSpecialOrder)),
                postfix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.GetSpecialOrder_ArchipelagoReward_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.CheckCompletion)),
                postfix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.CheckCompletion_ArchipelagoReward_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.SetDuration)),
                prefix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.SetDuration_UseCorrectDateWithSeasonRandomizer_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.UpdateAvailableSpecialOrders)),
                prefix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.UpdateAvailableSpecialOrders_ChangeFrequencyToBeLessRng_Prefix))
            );

            if (_archipelago.SlotData.SpecialOrderLocations == SpecialOrderLocations.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(SpecialOrder), nameof(SpecialOrder.IsSpecialOrdersBoardUnlocked)),
                prefix: new HarmonyMethod(typeof(SpecialOrderInjections), nameof(SpecialOrderInjections.IsSpecialOrdersBoardUnlocked_UnlockBasedOnApItem_Prefix))
            );
        }

        private void ReplaceChildrenWithChecks()
        {
            if (_archipelago.SlotData.Friendsanity == Friendsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(SpouseInjections), nameof(SpouseInjections.CheckAction_SpouseStardrop_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.canGetPregnant)),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.CanGetPregnant_ShuffledPregnancies_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.Setup_PregnancyQuestionEvent_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(QuestionEvent), "answerPregnancyQuestion"),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.AnswerPregnancyQuestion_CorrectDate_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.tickUpdate)),
                prefix: new HarmonyMethod(typeof(PregnancyInjections), nameof(PregnancyInjections.TickUpdate_BirthingEvent_Prefix))
            );
        }

        private void PatchCarpenter()
        {
            _modHelper.Events.Content.AssetRequested += _carpenterShopStockModifier.OnShopStockRequested;
            _modHelper.Events.Content.AssetRequested += _carpenterBuildingsModifier.OnBuildingsRequested;
            _modHelper.GameContent.InvalidateCache("Data/Buildings");

            var performActionArgumentTypes = new[] { typeof(string[]), typeof(Farmer), typeof(Location) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction), performActionArgumentTypes),
                prefix: new HarmonyMethod(typeof(WizardBookInjections), nameof(WizardBookInjections.PerformAction_WizardBook_Prefix))
            );

            var blueprintEntryParameters = new[] { typeof(int), typeof(string), typeof(BuildingData), typeof(string) };
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(CarpenterMenu.BlueprintEntry), blueprintEntryParameters),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.BlueprintEntryConstructor_IfFreeMakeTheIdCorrect_Prefix))
            );

            if (!_archipelago.SlotData.BuildingProgression.HasFlag(BuildingProgression.Progressive))
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), "houseUpgradeOffer"),
                    prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeOffer_OfferCheaperUpgrade_Prefix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), "houseUpgradeAccept"),
                    prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeAccept_CheaperInAP_Prefix))
                );

                return;
            }

            var desiredOverloadParameters = new[] { typeof(string), typeof(Response[]), typeof(string) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.createQuestionDialogue), desiredOverloadParameters),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.CreateQuestionDialogue_CarpenterDialogOptions_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "houseUpgradeOffer"),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeOffer_OfferFreeUpgrade_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), "houseUpgradeAccept"),
                prefix: new HarmonyMethod(typeof(CarpenterInjections), nameof(CarpenterInjections.HouseUpgradeAccept_FreeFromAP_Prefix))
            );
        }

        private void CleanCarpenterEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _carpenterShopStockModifier.OnShopStockRequested;
            _modHelper.Events.Content.AssetRequested -= _carpenterBuildingsModifier.OnBuildingsRequested;
        }

        private void ReplaceIsolatedEventsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.SkipEvent_RustySword_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.AwardFestivalPrize_RustySword_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Woods), nameof(Woods.checkAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.CheckAction_OldMasterCanolli_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.AnswerDialogueAction_BeachBridge_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.checkAction)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.CheckAction_BeachBridge_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.fixBridge)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.FixBridge_DontFixDuringDraw_Prefix))
            );
            _harmony.Patch(
                original: AccessTools.Method(typeof(Beach), nameof(Beach.draw)),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.Draw_BeachBridgeQuestionMark_Prefix)),
                postfix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.Draw_BeachBridgeQuestionMark_Postfix))
            );

            var performTouchActionArgumentTypes = new[] { typeof(string[]), typeof(Vector2) };
            _harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performTouchAction), performTouchActionArgumentTypes),
                prefix: new HarmonyMethod(typeof(IsolatedEventInjections), nameof(IsolatedEventInjections.PerformTouchAction_GalaxySwordShrine_Prefix))
            );
        }

        private void PatchAdventurerGuildShop()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(DefaultPhoneHandler), nameof(DefaultPhoneHandler.CallAdventureGuild)),
                prefix: new HarmonyMethod(typeof(PhoneInjections), nameof(PhoneInjections.CallAdventureGuild_AllowRecovery_Prefix))
            );

            _modHelper.Events.Content.AssetRequested += _guildShopStockModifier.OnShopStockRequested;
        }

        private void CleanAdventureGuildEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _guildShopStockModifier.OnShopStockRequested;
        }

        private void ReplaceArcadeMachinesWithChecks()
        {
            if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.usePowerup)),
                prefix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.UsePowerup_PrairieKingBossBeaten_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MineCart), nameof(MineCart.EndCutscene)),
                prefix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.EndCutscene_JunimoKartLevelComplete_Prefix))
            );

            if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.Victories)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(AbigailGame.CowboyMonster), nameof(AbigailGame.CowboyMonster.getLootDrop)),
                prefix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.GetLootDrop_ExtraLoot_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MineCart), "restartLevel"),
                postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.RestartLevel_NewGame_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MineCart), nameof(MineCart.UpdateFruitsSummary)),
                postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.UpdateFruitsSummary_ExtraLives_Postfix))
            );

            var desiredAbigailGameCtorParameters = new[] { typeof(NPC) };
            _harmony.Patch(
                original: AccessTools.Constructor(typeof(AbigailGame), desiredAbigailGameCtorParameters),
                postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.AbigailGameCtor_Equipments_Postfix))
            );

            if (_archipelago.SlotData.ArcadeMachineLocations == ArcadeLocations.FullShuffling)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.startShoppingLevel)),
                    postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.StartShoppingLevel_ShopBasedOnSentChecks_PostFix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(AbigailGame), nameof(AbigailGame.tick)),
                    postfix: new HarmonyMethod(typeof(ArcadeMachineInjections), nameof(ArcadeMachineInjections.Tick_Shopping_PostFix))
                );
            }
        }

        private void PatchTravelingMerchant()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Forest), nameof(Forest.ShouldTravelingMerchantVisitToday)),
                prefix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.ShouldTravelingMerchantVisitToday_ArchipelagoDays_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.checkAction)),
                prefix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.NightMarketCheckAction_IsTravelingMerchantDay_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(ShopMenu), nameof(ShopMenu.SetUpShopOwner)),
                postfix: new HarmonyMethod(typeof(TravelingMerchantInjections), nameof(TravelingMerchantInjections.SetUpShopOwner_TravelingMerchantApFlair_Postfix))
            );

            _modHelper.Events.Content.AssetRequested += _travelingMerchantShopStockModifier.OnShopStockRequested;
        }

        private void CleanTravelingMerchantEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _travelingMerchantShopStockModifier.OnShopStockRequested;
        }

        private void AddFishsanityLocations()
        {
            if (_archipelago.SlotData.Goal == Goal.MasterAngler)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Farmer), nameof(Farmer.caughtFish)),
                    postfix: new HarmonyMethod(typeof(FishingInjections), nameof(FishingInjections.CaughtFish_CheckGoalCompletion_Postfix))
                );
            }

            if (_archipelago.SlotData.Fishsanity == Fishsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.caughtFish)),
                postfix: new HarmonyMethod(typeof(FishingInjections), nameof(FishingInjections.CaughtFish_Fishsanity_Postfix))
            );
        }

        private void AddMuseumsanityLocations()
        {
            if (_archipelago.SlotData.Goal == Goal.CompleteCollection)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(LibraryMuseum), nameof(LibraryMuseum.getRewardsForPlayer)),
                    postfix: new HarmonyMethod(typeof(MuseumInjections), nameof(MuseumInjections.GetRewardsForPlayer_CheckGoalCompletion_Postfix))
                );
            }

            if (_archipelago.SlotData.Museumsanity == Museumsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(LibraryMuseum), nameof(LibraryMuseum.getRewardsForPlayer)),
                prefix: new HarmonyMethod(typeof(MuseumInjections), nameof(MuseumInjections.GetRewardsForPlayer_Museumsanity_Prefix))
            );
        }

        private void ReplaceFriendshipsWithChecks()
        {
            _harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Friendship), nameof(Friendship.Points)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.GetPoints_ArchipelagoHearts_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Constructor(typeof(SocialPage), new[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.SocialPageCtor_CheckHints_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(SocialPage), "drawNPCSlot"),
                postfix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.DrawNPCSlot_DrawEarnedHearts_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Pet), nameof(Pet.dayUpdate)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.DayUpdate_ArchipelagoPoints_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.changeFriendship)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.ChangeFriendship_ArchipelagoPoints_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.resetFriendshipsForNewDay)),
                prefix: new HarmonyMethod(typeof(FriendshipInjections), nameof(FriendshipInjections.ResetFriendshipsForNewDay_AutopetHumans_Prefix))
            );
        }

        private void PatchFestivals()
        {
            _modHelper.Events.Content.AssetRequested += _festivalShopStockModifier.OnShopStockRequested;

            _harmony.Patch(
                original: AccessTools.Method(typeof(Utility), nameof(Utility.GetRandomWinterStarParticipant)),
                prefix: new HarmonyMethod(typeof(WinterStarInjections), nameof(WinterStarInjections.GetRandomWinterStarParticipant_ChooseBasedOnMonthNotYear_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.forceEndFestival)),
                postfix: new HarmonyMethod(typeof(FairInjections), nameof(FairInjections.ForceEndFestival_KeepStarTokens_Postfix))
            );

            if (_archipelago.SlotData.FestivalLocations == FestivalLocations.Vanilla)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(EggFestivalInjections), nameof(EggFestivalInjections.AwardFestivalPrize_Strawhat_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.setUpFestivalMainEvent)),
                postfix: new HarmonyMethod(typeof(FlowerDanceInjections), nameof(FlowerDanceInjections.SetUpFestivalMainEvent_FlowerDance_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.SwitchEvent)),
                postfix: new HarmonyMethod(typeof(LuauInjections), nameof(LuauInjections.SwitchEvent_GovernorReactionToSoup_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.setUpFestivalMainEvent)),
                postfix: new HarmonyMethod(typeof(MoonlightJelliesInjections), nameof(MoonlightJelliesInjections.SetUpFestivalMainEvent_MoonlightJellies_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(StrengthGame), nameof(StrengthGame.update)),
                prefix: new HarmonyMethod(typeof(FairInjections), nameof(FairInjections.StrengthGameUpdate_StrongEnough_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.interpretGrangeResults)),
                postfix: new HarmonyMethod(typeof(FairInjections), nameof(FairInjections.InterpretGrangeResults_Success_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Chest), nameof(Chest.checkForAction)),
                prefix: new HarmonyMethod(typeof(SpiritEveInjections), nameof(SpiritEveInjections.CheckForAction_SpiritEveChest_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AwardFestivalPrize)),
                prefix: new HarmonyMethod(typeof(IceFestivalInjections), nameof(IceFestivalInjections.AwardFestivalPrize_FishingCompetition_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(MermaidHouse), nameof(MermaidHouse.playClamTone), new Type[] { typeof(int), typeof(Farmer) }),
                prefix: new HarmonyMethod(typeof(MermaidHouseInjections), nameof(MermaidHouseInjections.PlayClamTone_SongFinished_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.draw)),
                prefix: new HarmonyMethod(typeof(BeachNightMarketInjections), nameof(BeachNightMarketInjections.Draw_DontDrawOriginalPainting_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.draw)),
                postfix: new HarmonyMethod(typeof(BeachNightMarketInjections), nameof(BeachNightMarketInjections.Draw_DrawCorrectPainting_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.checkAction)),
                prefix: new HarmonyMethod(typeof(BeachNightMarketInjections), nameof(BeachNightMarketInjections.CheckAction_LupiniPainting_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(BeachNightMarket), nameof(BeachNightMarket.answerDialogueAction)),
                prefix: new HarmonyMethod(typeof(BeachNightMarketInjections), nameof(BeachNightMarketInjections.AnswerDialogueAction_LupiniPainting_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Dialogue), nameof(Dialogue.chooseResponse)),
                postfix: new HarmonyMethod(typeof(WinterStarInjections), nameof(WinterStarInjections.ChooseResponse_LegendOfTheWinterStar_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.chooseSecretSantaGift)),
                prefix: new HarmonyMethod(typeof(WinterStarInjections), nameof(WinterStarInjections.ChooseSecretSantaGift_SuccessfulGift_Prefix))
            );
        }

        private void CleanFestivalEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _festivalShopStockModifier.OnShopStockRequested;
        }

        private void AddCropSanityLocations()
        {
            if (_archipelago.SlotData.Cropsanity == Cropsanity.Disabled)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Crop), nameof(Crop.harvest)),
                postfix: new HarmonyMethod(typeof(CropsanityInjections), nameof(CropsanityInjections.Harvest_CheckCropsanityLocation_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(FruitTree), nameof(FruitTree.shake)),
                prefix: new HarmonyMethod(typeof(CropsanityInjections), nameof(CropsanityInjections.Shake_CheckCropsanityFruitTreeLocation_Prefix))
            );
        }

        private void PatchMonstersanity()
        {
            if (_archipelago.SlotData.Goal == Goal.ProtectorOfTheValley)
            {
                _harmony.Patch(
                    original: AccessTools.Method(typeof(Stats), nameof(Stats.monsterKilled)),
                    postfix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.MonsterKilled_CheckGoalCompletion_Postfix))
                );

                _harmony.Patch(
                    original: AccessTools.Method(typeof(AdventureGuild), nameof(AdventureGuild.areAllMonsterSlayerQuestsComplete)),
                    prefix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.AreAllMonsterSlayerQuestsComplete_ExcludeGingerIsland_Prefix))
                );
            }

            _harmony.Patch(
                original: AccessTools.PropertyGetter(typeof(Character), nameof(Character.Name)),
                postfix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.GetName_SkeletonMage_Postfix))
            );

            if (_archipelago.SlotData.Monstersanity == Monstersanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(AdventureGuild), "gil"),
                prefix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.Gil_NoMonsterSlayerRewards_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(AdventureGuild), nameof(AdventureGuild.showMonsterKillList)),
                prefix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.ShowMonsterKillList_CustomListFromAP_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Stats), nameof(Stats.monsterKilled)),
                postfix: new HarmonyMethod(typeof(MonsterSlayerInjections), nameof(MonsterSlayerInjections.MonsterKilled_SendMonstersanityCheck_Postfix))
            );
        }

        private void AddCooksanityLocations()
        {
            if (_archipelago.SlotData.Cooksanity == Cooksanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.cookedRecipe)),
                postfix: new HarmonyMethod(typeof(CookingInjections), nameof(CookingInjections.CookedRecipe_CheckCooksanityLocation_Postfix))
            );
        }

        private void PatchChefAndCraftsanity()
        {
            PatchChefsanity();
            PatchCraftsanity();
        }

        private void PatchChefsanity()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(TV), "getWeeklyRecipe", Type.EmptyTypes),
                prefix: new HarmonyMethod(typeof(QueenOfSauceInjections), nameof(QueenOfSauceInjections.GetWeeklyRecipe_UseArchipelagoSchedule_Prefix))
            );

            _modHelper.Events.Content.AssetRequested += _cookingRecipePurchaseStockModifier.OnShopStockRequested;

            _harmony.Patch(
                original: AccessTools.Constructor(typeof(LevelUpMenu), new[] { typeof(int), typeof(int) }),
                prefix: new HarmonyMethod(typeof(RecipeLevelUpInjections), nameof(RecipeLevelUpInjections.LevelUpMenuConstructor_SendSkillRecipeChecks_Postfix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.grantConversationFriendship)),
                prefix: new HarmonyMethod(typeof(RecipeFriendshipInjections), nameof(RecipeFriendshipInjections.GrantConversationFriendship_SendFriendshipRecipeChecks_Postfix))
            );

            if (!_archipelago.SlotData.Chefsanity.HasFlag(Chefsanity.Friendship))
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AddCookingRecipe)),
                prefix: new HarmonyMethod(typeof(RecipeFriendshipInjections), nameof(RecipeFriendshipInjections.AddCookingRecipe_SkipLearningCookies_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                prefix: new HarmonyMethod(typeof(RecipeFriendshipInjections), nameof(RecipeFriendshipInjections.SkipEvent_CookiesRecipe_Prefix))
            );
        }

        private void CleanChefsanityEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _cookingRecipePurchaseStockModifier.OnShopStockRequested;
        }

        private void PatchCraftsanity()
        {
            _modHelper.Events.Content.AssetRequested += _craftingRecipePurchaseStockModifier.OnShopStockRequested;

            if (_archipelago.SlotData.Craftsanity == Craftsanity.None)
            {
                return;
            }

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.AddCraftingRecipe)),
                prefix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.AddCraftingRecipe_SkipLearningFurnace_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.skipEvent)),
                prefix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.SkipEvent_FurnaceRecipe_Prefix))
            );

            _harmony.Patch(
                original: AccessTools.Method(typeof(Stats), nameof(Stats.checkForCraftingAchievements)),
                postfix: new HarmonyMethod(typeof(CraftingInjections), nameof(CraftingInjections.CheckForCraftingAchievements_CheckCraftsanityLocation_Postfix))
            );
        }

        private void CleanCraftsanityEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _craftingRecipePurchaseStockModifier.OnShopStockRequested;
        }

        private void PatchKrobusShop()
        {
            _modHelper.Events.Content.AssetRequested += _krobusStockModifier.OnShopStockRequested;
        }

        private void CleanKrobusEvents()
        {
            _modHelper.Events.Content.AssetRequested -= _krobusStockModifier.OnShopStockRequested;
        }

        private void PatchFarmcave()
        {
            _harmony.Patch(
                original: AccessTools.Method(typeof(Event), nameof(Event.answerDialogue)),
                prefix: new HarmonyMethod(typeof(FarmCaveInjections), nameof(FarmCaveInjections.AnswerDialogue_SendFarmCaveCheck_Prefix))
            );
        }
    }
}
