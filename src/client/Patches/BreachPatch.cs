using System;
using System.Reflection;
using HarmonyLib;
using EFT.Interactive;

namespace Blackhorse311.ForcibleEntry.Patches
{
    /// <summary>
    /// Patch Door.BreachSuccessRoll to track breach attempts and force success after threshold.
    /// This allows players to force open any locked door by breaching it multiple times.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.BreachSuccessRoll))]
    public static class BreachSuccessPatch
    {
        /// <summary>
        /// Prefix patch to intercept breach success roll.
        /// Tracks breach attempts and forces success after configured number of attempts.
        /// </summary>
        [HarmonyPrefix]
        public static bool Prefix(Door __instance, ref bool __result)
        {
            try
            {
                string doorId = __instance.Id;

                // Record this breach attempt and check if threshold reached
                bool shouldUnlock = BreachTracker.RecordBreach(doorId);

                if (shouldUnlock)
                {
                    // Force the breach to succeed
                    __result = true;
                    return false; // Skip original method
                }

                // Let original method run (normal breach chance)
                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in BreachSuccessPatch: {ex}");
                return true; // Let original method run on error
            }
        }
    }

    /// <summary>
    /// Patch to make breach action available on locked doors.
    /// By default, breach only works on unlocked (shut) doors.
    /// This patch allows breach attempts on locked doors.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.CanBeBreached), MethodType.Getter)]
    public static class CanBeBreachedPatch
    {
        /// <summary>
        /// Postfix to allow breach on locked doors.
        /// </summary>
        [HarmonyPostfix]
        public static void Postfix(Door __instance, ref bool __result)
        {
            try
            {
                // If door is locked, allow breach (so player can attempt to force it open)
                if (__instance.DoorState == EDoorState.Locked)
                {
                    __result = true;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in CanBeBreachedPatch: {ex}");
            }
        }
    }

    /// <summary>
    /// Patch to handle the actual door unlock after successful breach on locked door.
    /// When a locked door is breached successfully, it should unlock and open.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.KickOpen))]
    public static class KickOpenPatch
    {
        /// <summary>
        /// Prefix to handle kicking open a locked door.
        /// If the door is locked, we need to unlock it first.
        /// </summary>
        [HarmonyPrefix]
        public static void Prefix(Door __instance)
        {
            try
            {
                // If the door is locked, unlock it first so it can be kicked open
                if (__instance.DoorState == EDoorState.Locked)
                {
                    Plugin.Log?.LogDebug($"[ForcibleEntry] Unlocking door {__instance.Id} after forced breach!");

                    // Use reflection to set door state to Shut (unlocked but closed)
                    // This allows KickOpen to work properly
                    var doorStateField = typeof(Door).GetField("_doorState", BindingFlags.NonPublic | BindingFlags.Instance);
                    if (doorStateField != null)
                    {
                        doorStateField.SetValue(__instance, EDoorState.Shut);
                    }
                    else
                    {
                        // Try the property setter approach
                        __instance.DoorState = EDoorState.Shut;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in KickOpenPatch: {ex}");
            }
        }
    }
}
