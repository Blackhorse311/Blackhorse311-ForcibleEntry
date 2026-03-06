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

            try
            {
                _harmony = new Harmony(PluginInfo.PLUGIN_GUID);
                _harmony.PatchAll(Assembly.GetExecutingAssembly());

                Log.LogInfo($"{PluginInfo.PLUGIN_NAME} v{PluginInfo.PLUGIN_VERSION} loaded.");
                Log.LogInfo($"Breach {BreachesToUnlock.Value} times to force open any locked door!");
            }
            catch (Exception ex)
            {
                Log.LogError($"Failed to initialize patches: {ex}");
            }
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
        public const string PLUGIN_VERSION = "1.0.2";
    }
}
