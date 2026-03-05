using System;
using HarmonyLib;
using UnityEngine;
using EFT.Interactive;

namespace Blackhorse311.ForcibleEntry.Patches
{
    /// <summary>
    /// Patch Door.BreachSuccessRoll to track breach attempts and force success after threshold.
    /// This allows players to force open any locked door by breaching it multiple times.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.BreachSuccessRoll), typeof(Vector3))]
    public static class BreachSuccessPatch
    {
        [HarmonyPrefix]
        public static bool Prefix(Door __instance, ref bool __result)
        {
            try
            {
                string doorId = __instance.Id;

                bool shouldUnlock = BreachTracker.RecordBreach(doorId);

                if (shouldUnlock)
                {
                    __result = true;
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogError($"Error in BreachSuccessPatch: {ex}");
                return true;
            }
        }
    }

    /// <summary>
    /// Patch GetInteractionParameters to enable breach on locked doors.
    /// CanBeBreached is now a public field (not a property), so we set it
    /// before interaction parameters are calculated.
    /// </summary>
    [HarmonyPatch(typeof(Door), nameof(Door.GetInteractionParameters), typeof(Vector3))]
    public static class CanBeBreachedPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Door __instance)
        {
            try
            {
                if (__instance.DoorState == EDoorState.Locked)
                {
                    __instance.CanBeBreached = true;
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
    [HarmonyPatch(typeof(Door), nameof(Door.KickOpen), typeof(Vector3), typeof(bool))]
    public static class KickOpenPatch
    {
        [HarmonyPrefix]
        public static void Prefix(Door __instance)
        {
            try
            {
                if (__instance.DoorState == EDoorState.Locked)
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
}
