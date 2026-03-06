// ============================================================================
// Forcible Entry - Icon Service
// ============================================================================
// Registers custom item icons with the SPT image router.
// Icons are loaded from the mod's assets folder.
//
// AUTHOR: Blackhorse311
// LICENSE: MIT
//
// CREDITS:
// - Halligan Bar 3D Model: CGTrader (https://www.cgtrader.com/3d-models/industrial/tool/halligan-bar)
// - Fire Axe 3D Model: CGTrader (https://www.cgtrader.com/3d-models/industrial/tool/fire-axe-233fc15b-d6d5-4e57-aa61-6eec4923cc08)
// ============================================================================

using System.Reflection;
using SPTarkov.DI.Annotations;
using SPTarkov.Server.Core.Models.Utils;
using SPTarkov.Server.Core.Routers;

namespace Blackhorse311.ForcibleEntry.Server;

[Injectable(InjectionType.Singleton)]
public class IconService(
    ISptLogger<IconService> logger,
    ImageRouter imageRouter)
{
    /// <summary>
    /// Register custom item icons with the image router
    /// </summary>
    public void RegisterIcons()
    {
        try
        {
            // Get the mod folder path
            var assemblyLocation = Assembly.GetExecutingAssembly().Location;
            if (string.IsNullOrEmpty(assemblyLocation))
            {
                logger.Error("[ForcibleEntry-Server] Could not determine assembly location for icon loading.");
                return;
            }

            string modFolder = Path.GetDirectoryName(assemblyLocation)!;

            // Icons should be in the assets/icons folder relative to the mod
            // But since server mod is in user/mods, we need to look for icons bundled with it
            string iconsFolder = Path.Combine(modFolder, "icons");

            // Register Halligan icon
            string halliganIconPath = Path.Combine(iconsFolder, $"{ItemService.HalliganId}.png");
            if (File.Exists(halliganIconPath))
            {
                imageRouter.AddRoute($"/files/handbook/{ItemService.HalliganId}.png", halliganIconPath);
                imageRouter.AddRoute($"/files/quest/icon/{ItemService.HalliganId}.png", halliganIconPath);
                logger.Debug($"[ForcibleEntry-Server] Registered Halligan icon: {halliganIconPath}");
            }
            else
            {
                logger.Warning($"[ForcibleEntry-Server] Halligan icon not found: {halliganIconPath}");
            }

            // Register Flat-Head Axe icon
            string axeIconPath = Path.Combine(iconsFolder, $"{ItemService.FlatHeadAxeId}.png");
            if (File.Exists(axeIconPath))
            {
                imageRouter.AddRoute($"/files/handbook/{ItemService.FlatHeadAxeId}.png", axeIconPath);
                imageRouter.AddRoute($"/files/quest/icon/{ItemService.FlatHeadAxeId}.png", axeIconPath);
                logger.Debug($"[ForcibleEntry-Server] Registered Flat-Head Axe icon: {axeIconPath}");
            }
            else
            {
                logger.Warning($"[ForcibleEntry-Server] Flat-Head Axe icon not found: {axeIconPath}");
            }

            logger.Info("[ForcibleEntry-Server] Custom icons registered.");
        }
        catch (Exception ex)
        {
            logger.Error($"[ForcibleEntry-Server] Failed to register icons: {ex.Message}");
        }
    }
}
