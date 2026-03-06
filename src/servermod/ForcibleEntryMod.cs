// ============================================================================
// Forcible Entry - Server Mod Entry Point
// ============================================================================
// This is the main entry point for the server-side component of the
// Forcible Entry mod. It registers custom items (Halligan bar, Flat-Head Axe)
// and adds them to trader inventories.
//
// AUTHOR: Blackhorse311
// LICENSE: MIT
// ============================================================================

using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.DI;
using SPTarkov.Server.Core.Models.Utils;

namespace Blackhorse311.ForcibleEntry.Server;

/// <summary>
/// Main entry point for the Forcible Entry server-side mod.
/// Implements IOnLoad to run initialization code during server startup.
/// </summary>
[Injectable(TypePriority = OnLoadOrder.PostDBModLoader)]
public class ForcibleEntryMod(
    ISptLogger<ForcibleEntryMod> logger,
    ItemService itemService,
    TraderService traderService,
    IconService iconService) : IOnLoad
{
    public Task OnLoad()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "unknown";
        logger.Info("[ForcibleEntry-Server] ============================================");
        logger.Info($"[ForcibleEntry-Server] Forcible Entry v{version} - Server Component");
        logger.Info("[ForcibleEntry-Server] ============================================");

        // Create custom items - if this fails, skip trader/icon registration
        // since they depend on items existing
        try
        {
            itemService.CreateForcibleEntryItems();
            logger.Info("[ForcibleEntry-Server] Custom items created successfully.");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to create custom items: {ex.Message}");
            logger.Error("[ForcibleEntry-Server] Skipping trader and icon registration due to item creation failure.");
            return Task.CompletedTask;
        }

        // Add items to traders - only for items that were successfully created
        try
        {
            traderService.AddItemsToTraders();
            logger.Info("[ForcibleEntry-Server] Items added to traders successfully.");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to add items to traders: {ex.Message}");
        }

        // Register custom icons
        try
        {
            iconService.RegisterIcons();
            logger.Info("[ForcibleEntry-Server] Icons registered successfully.");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to register icons: {ex.Message}");
        }

        logger.Info("[ForcibleEntry-Server] Forcible entry tools ready for use.");

        return Task.CompletedTask;
    }
}
