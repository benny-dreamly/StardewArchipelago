﻿using KaitoKid.ArchipelagoUtilities.Net.Client;
using StardewArchipelago.Items.Mail;
using StardewValley;
using StardewValley.Tools;

namespace StardewArchipelago.Stardew
{
    public class StardewSlingshot : StardewWeapon
    {
        private readonly int _defaultSalePrice;

        public StardewSlingshot(string id, string name, string description, int minDamage, int maxDamage, double knockBack, double speed, double addedPrecision, double addedDefence, int type, int baseMineLevel, int minMineLevel, double addedAoe, double criticalChance, double criticalDamage, string displayName, int defaultSalePrice)
            : base(id, name, description, minDamage, maxDamage, knockBack, speed, addedPrecision, addedDefence, type, baseMineLevel, minMineLevel, addedAoe, criticalChance, criticalDamage, displayName)
        {
            _defaultSalePrice = defaultSalePrice;
        }

        public override Item PrepareForGivingToFarmer(int amount = 1)
        {
            return new Slingshot(Id);
        }

        public override LetterAttachment GetAsLetter(ReceivedItem receivedItem, int amount = 1)
        {
            return new LetterActionAttachment(receivedItem, LetterActionsKeys.GiveSlingshot, Id.ToString());
        }

        public int GetDefaultSalePrice()
        {
            return _defaultSalePrice;
        }
    }
}
