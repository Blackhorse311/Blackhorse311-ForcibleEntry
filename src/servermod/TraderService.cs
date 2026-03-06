// ============================================================================
// Forcible Entry - Trader Service
// ============================================================================
// Adds the forcible entry tools to trader inventories.
// Items are added to Jaeger (fits thematically with outdoor/survival tools)
//
// AUTHOR: Blackhorse311
// LICENSE: MIT
// ============================================================================

using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services;

namespace Blackhorse311.ForcibleEntry.Server;

[Injectable(InjectionType.Singleton)]
public class TraderService(
    ISptLogger<TraderService> logger,
    DatabaseService databaseService,
    ItemService itemService)
{
    // Trader IDs
    private const string JaegerId = "5c0647fdd443bc2504c2d371";

    // Currency
    private const string RoublesId = "5449016a4bdc2d6f028b456f";

    // Assort IDs - must be 24-character hex strings
    // Using a pattern: fe + "assort" prefix + sequential number
    private const string HalliganAssortId = "fe00000000000000a5507001";
    private const string FlatHeadAxeAssortId = "fe00000000000000a5507002";

    /// <summary>
    /// Add forcible entry items to trader assorts
    /// </summary>
    public void AddItemsToTraders()
    {
        try
        {
            // Only add items that were successfully created
            if (itemService.HalliganCreated)
                AddItemToJaeger(ItemService.HalliganId, HalliganAssortId, 45000, 1);

            if (itemService.FlatHeadAxeCreated)
                AddItemToJaeger(ItemService.FlatHeadAxeId, FlatHeadAxeAssortId, 28000, 1);

            logger.Info("[ForcibleEntry-Server] Items added to Jaeger's inventory.");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to add items to traders: {ex.Message}");
        }
    }

    /// <summary>
    /// Add an item to Jaeger's assort
    /// </summary>
    /// <param name="itemTpl">The item template ID</param>
    /// <param name="assortId">The assort ID (must be 24-char hex)</param>
    /// <param name="priceRoubles">Price in roubles</param>
    /// <param name="loyaltyLevel">Required loyalty level (1-4)</param>
    private void AddItemToJaeger(string itemTpl, string assortId, int priceRoubles, int loyaltyLevel)
    {
        var trader = databaseService.GetTrader(JaegerId);
        if (trader?.Assort == null)
        {
            logger.Error("[ForcibleEntry-Server] Jaeger trader or assort not found!");
            return;
        }

        var itemId = assortId;

        // Idempotency guard - skip if already added (e.g. duplicate load)
        if (trader.Assort.Items.Exists(i => i.Id == itemId))
        {
            logger.Warning($"[ForcibleEntry-Server] Item {itemId} already in Jaeger's assort, skipping.");
            return;
        }

        // Create the item entry
        var item = new Item
        {
            Id = itemId,
            Template = itemTpl,
            ParentId = "hideout",
            SlotId = "hideout",
            Upd = new Upd
            {
                UnlimitedCount = false,
                StackObjectsCount = 5,
                BuyRestrictionMax = 2,
                BuyRestrictionCurrent = 0
            }
        };

        // Add item to assort
        trader.Assort.Items.Add(item);

        // Set loyalty level requirement
        trader.Assort.LoyalLevelItems[itemId] = loyaltyLevel;

        // Create barter scheme (price in roubles)
        var barterScheme = new List<List<BarterScheme>>
        {
            new List<BarterScheme>
            {
                new BarterScheme
                {
                    Template = RoublesId,
                    Count = priceRoubles
                }
            }
        };

        trader.Assort.BarterScheme[itemId] = barterScheme;

        logger.Debug($"[ForcibleEntry-Server] Added {itemTpl} to Jaeger at LL{loyaltyLevel} for {priceRoubles} roubles");
    }
}
