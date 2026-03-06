// ============================================================================
// Forcible Entry - Item Service
// ============================================================================
// Creates and registers the custom forcible entry tools with the SPT database.
// Items are cloned from existing game items and modified with custom properties.
//
// ITEMS:
// - Halligan Bar: A firefighter's prying tool used for forcible entry
// - Flat-Head Axe: A firefighter's axe used to strike the Halligan
//
// AUTHOR: Blackhorse311
// LICENSE: MIT
// ============================================================================

using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Eft.Common.Tables;
using SPTarkov.Server.Core.Models.Spt.Mod;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Services.Mod;

namespace Blackhorse311.ForcibleEntry.Server;

[Injectable(InjectionType.Singleton)]
public class ItemService(
    ISptLogger<ItemService> logger,
    CustomItemService customItemService)
{
    // Custom item IDs - must be 24-character hex strings (MongoDB ObjectId format)
    // These must match what the client mod expects
    public const string HalliganId = "fe0000000000000000000001";      // Halligan Bar
    public const string FlatHeadAxeId = "fe0000000000000000000002";   // Flat-Head Axe

    // Base items to clone from
    // Freeman Crowbar (melee weapon): 5c07df7f0db834001b73588a
    // Leatherman Multitool: 544fb5454bdc2df8738b456a
    private const string CrowbarTemplateId = "5c07df7f0db834001b73588a";    // Freeman Crowbar (melee)
    private const string MultitoolTemplateId = "544fb5454bdc2df8738b456a";  // Leatherman Multitool

    // Parent IDs for item categories
    // Tool category: 57864bb7245977548b3b66c2 (Barter items > Tools)
    private const string ToolParentId = "57864bb7245977548b3b66c2";

    // Handbook category for tools
    private const string ToolHandbookParentId = "5b47574386f77428ca22b342";

    // Track which items were successfully created for downstream consumers
    public bool HalliganCreated { get; private set; }
    public bool FlatHeadAxeCreated { get; private set; }

    public void CreateForcibleEntryItems()
    {
        CreateHalliganBar();
        CreateFlatHeadAxe();
    }

    private void CreateHalliganBar()
    {
        logger.Info("[ForcibleEntry-Server] Creating Halligan Bar item...");

        var locales = new Dictionary<string, LocaleDetails>
        {
            ["en"] = new LocaleDetails
            {
                Name = "Halligan Bar",
                ShortName = "Halligan",
                Description = "A multipurpose firefighter tool consisting of a claw, blade, and tapered pick. Essential for forcible entry operations. Works best when paired with a striking tool like a flat-head axe or sledgehammer."
            }
        };

        var itemDetails = new NewItemFromCloneDetails
        {
            ItemTplToClone = MultitoolTemplateId,
            ParentId = ToolParentId,
            NewId = HalliganId,
            FleaPriceRoubles = 45000,
            HandbookPriceRoubles = 38000,
            HandbookParentId = ToolHandbookParentId,
            Locales = locales,
            OverrideProperties = new TemplateItemProperties
            {
                Weight = 4.5f,          // Halligan bars are heavy (~4.5 kg)
                Width = 3,              // 3 slots wide (firefighter tool)
                Height = 1,
                ExamineTime = 1,
                BackgroundColor = "orange"
            }
        };

        try
        {
            customItemService.CreateItemFromClone(itemDetails);
            HalliganCreated = true;
            logger.Info($"[ForcibleEntry-Server] Halligan Bar created with ID: {HalliganId}");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to create Halligan Bar: {ex.Message}");
            throw;
        }
    }

    private void CreateFlatHeadAxe()
    {
        logger.Info("[ForcibleEntry-Server] Creating Flat-Head Axe item...");

        var locales = new Dictionary<string, LocaleDetails>
        {
            ["en"] = new LocaleDetails
            {
                Name = "Flat-Head Axe",
                ShortName = "FH Axe",
                Description = "A firefighter's flat-head axe with a 6-pound head. The flat striking surface is designed to be used with a Halligan bar for forcible entry. The 'irons' combination of Halligan and flat-head axe is standard equipment for fire departments worldwide."
            }
        };

        var itemDetails = new NewItemFromCloneDetails
        {
            ItemTplToClone = CrowbarTemplateId,
            ParentId = ToolParentId,
            NewId = FlatHeadAxeId,
            FleaPriceRoubles = 25000,
            HandbookPriceRoubles = 20000,
            HandbookParentId = ToolHandbookParentId,
            Locales = locales,
            OverrideProperties = new TemplateItemProperties
            {
                Weight = 3.2f,          // ~7 lbs / 3.2 kg
                Width = 3,              // 3 slots wide (firefighter axe)
                Height = 1,
                ExamineTime = 1,
                BackgroundColor = "orange"
            }
        };

        try
        {
            customItemService.CreateItemFromClone(itemDetails);
            FlatHeadAxeCreated = true;
            logger.Info($"[ForcibleEntry-Server] Flat-Head Axe created with ID: {FlatHeadAxeId}");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to create Flat-Head Axe: {ex.Message}");
            throw;
        }
    }
}
