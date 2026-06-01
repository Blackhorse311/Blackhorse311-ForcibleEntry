using System;
using HarmonyLib;
using UnityEngine;
using EFT;
using EFT.Interactive;

namespace Blackhorse311.ForcibleEntry.Patches
{
    /// <summary>
    /// Patch Door.BreachSuccessRoll to track breach attempts and force success after threshold.
    /// For locked doors, we always control the result (threshold is the gate, not RNG).
    /// For normal doors, vanilla breach logic runs unmodified.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.BreachSuccessRoll), typeof(Vector3))]
    public static class BreachSuccessPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Door __instance, ref bool __result)
        {
            try
            {
                // Only intercept locked doors - vanilla handles normal breaches
                if (__instance.DoorState != EDoorState.Locked)
                    return true;

                string doorId = __instance.Id;
                bool shouldUnlock = BreachTracker.RecordBreach(doorId);

                // Always control the result for locked doors - threshold is the gate
                __result = shouldUnlock;
                return false;
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in BreachSuccessPatch: {ex}");
                return true;
            }
        }
    }

    /// <summary>
    /// Temporarily enable breach on locked doors for the duration of GetInteractionParameters.
    /// CanBeBreached is a public field - we set it in the prefix and restore in the postfix
    /// so we don't permanently mutate door state.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.GetInteractionParameters), typeof(Vector3))]
    public static class CanBeBreachedPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Door __instance, ref bool __state)
        {
            __state = __instance.CanBeBreached;
            try
            {
                if (__instance.DoorState == EDoorState.Locked)
                {
                    __instance.CanBeBreached = true;
                }
            }
            catch (Exception ex)
            {
                __instance.CanBeBreached = __state;
                Plugin.Log?.LogError($"Error in CanBeBreachedPatch prefix: {ex}");
            }
        }

        [HarmonyPostfix]
        public static void Postfix(Door __instance, bool __state)
        {
            try
            {
                __instance.CanBeBreached = __state;
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in CanBeBreachedPatch postfix: {ex}");
            }
        }
    }

    /// <summary>
    /// Unlock locked doors before KickOpen runs so the kick animation completes.
    /// Only unlocks doors that BreachTracker has authorized (threshold reached); a locked door
    /// that was never breached to threshold is left locked, so KickOpen can never "free unlock"
    /// a door reached through any path other than a successful gated breach roll.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.KickOpen), typeof(Vector3), typeof(bool))]
    public static class KickOpenPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Door __instance)
        {
            try
            {
                if (__instance.DoorState == EDoorState.Locked
                    && BreachTracker.ConsumeUnlockAuthorization(__instance.Id))
                {
                    Plugin.Log?.LogDebug($"[ForcibleEntry] Unlocking door {__instance.Id} after forced breach!");
                    __instance.DoorState = EDoorState.Shut;
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in KickOpenPatch: {ex}");
            }
        }
    }

    /// <summary>
    /// Clear breach tracking data at the start of each raid to prevent
    /// stale state from carrying over between raids.
    /// </summary>
    [HarmonyPatch(typeof(GameWorld), nameof(GameWorld.OnGameStarted))]
    public static class RaidCleanupPatch
    {
        [HarmonyPostfix]
        public static void Postfix()
        {
            BreachTracker.Clear();
            Plugin.Log?.LogDebug("[ForcibleEntry] Breach tracker cleared for new raid.");
        }
    }
}
