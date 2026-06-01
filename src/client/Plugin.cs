using System;
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace Blackhorse311.ForcibleEntry
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    [BepInProcess("EscapeFromTarkov.exe")]
    public class Plugin : BaseUnityPlugin
    {
        internal static ManualLogSource Log;
        internal static ConfigEntry<int> BreachesToUnlock;
        internal static ConfigEntry<float> BreachTimeout;

        private Harmony _harmony;

        private void Awake()
        {
            Log = Logger;

            // Configuration
            BreachesToUnlock = Config.Bind(
                "General",
                "BreachesToUnlock",
                5,
                new ConfigDescription(
                    "Number of consecutive breach attempts required to force open a locked door",
                    new AcceptableValueRange<int>(1, 20)
                )
            );

            BreachTimeout = Config.Bind(
                "General",
                "BreachTimeout",
                10.0f,
                new ConfigDescription(
                    "Time in seconds before breach counter resets (if you stop breaching)",
                    new AcceptableValueRange<float>(5f, 60f)
                )
            );

            // Apply each patch class individually rather than PatchAll(), which is all-or-nothing:
            // if one [HarmonyPatch] can't bind its obfuscated EFT target after a game/SPT update,
            // PatchAll throws and none of the patches apply, yet the plugin still reports loaded.
            // Per-class application lets us log exactly which patch broke and fail loudly.
            _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            int applied = ApplyPatches(
                typeof(Patches.BreachSuccessPatch),
                typeof(Patches.CanBeBreachedPatch),
                typeof(Patches.KickOpenPatch),
                typeof(Patches.RaidCleanupPatch));

            if (applied == 0)
            {
                Log.LogError(
                    $"{PluginInfo.PLUGIN_NAME} DISABLED — no patches could be applied. This usually means " +
                    $"an SPT/EFT version change renamed the Door/GameWorld members this mod hooks. " +
                    $"Check for a {PluginInfo.PLUGIN_NAME} update for your SPT version.");
            }
            else
            {
                Log.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} loaded ({applied} patches applied).");
                Log.LogInfo($"Breach {BreachesToUnlock.Value} times to force open any locked door!");
            }
        }

        /// <summary>
        /// Applies each patch class on its own so one failed bind (e.g. an obfuscated member
        /// renamed by a game update) is logged by name instead of silently aborting the rest.
        /// </summary>
        /// <returns>The number of patch classes that applied successfully.</returns>
        private int ApplyPatches(params Type[] patchClasses)
        {
            int applied = 0;
            foreach (var patchClass in patchClasses)
            {
                try
                {
                    _harmony.CreateClassProcessor(patchClass).Patch();
                    applied++;
                }
                catch (Exception ex)
                {
                    Log.LogError(
                        $"Failed to apply patch {patchClass.Name} (likely an SPT/EFT version change): {ex.Message}");
                }
            }
            return applied;
        }

        private void OnDestroy()
        {
            try
            {
                _harmony?.UnpatchSelf();
                BreachTracker.Clear();
            }
            catch (Exception ex)
            {
                Log?.LogWarning($"Failed to unpatch: {ex}");
            }
        }
    }

    public static class PluginInfo
    {
        public const string PLUGIN_GUID = "com.blackhorse311.forcibleentry";
        public const string PLUGIN_NAME = "Blackhorse311-ForcibleEntry";
        // Keep in sync with <Version> in Blackhorse311.ForcibleEntry.csproj
        public const string PLUGIN_VERSION = "1.0.3";
    }
}
