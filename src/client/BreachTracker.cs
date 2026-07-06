using System;
using System.Collections.Generic;

namespace Blackhorse311.ForcibleEntry
{
    /// <summary>
    /// Tracks consecutive breach attempts on doors.
    /// After reaching the configured threshold, the door will be unlocked.
    /// </summary>
    public static class BreachTracker
    {
        // All access occurs on Unity main thread via Harmony patches; no synchronization needed.
        private static readonly Dictionary<string, BreachData> _doorBreaches = new Dictionary<string, BreachData>();

        // Doors whose breach threshold has been reached and are therefore allowed to be unlocked
        // by KickOpenPatch. This makes RecordBreach the single authority on "may this door open";
        // KickOpen must not unlock a door that was never authorized here.
        private static readonly HashSet<string> _authorizedDoors = new HashSet<string>();

        // No seeding needed: each door's roll is made once and stored, so the sequence
        // doesn't have to be reproducible.
        private static readonly Random _random = new Random();

        private class BreachData
        {
            public int Count { get; set; }
            public DateTime LastBreachTime { get; set; }
            public int RolledThreshold { get; set; }
        }

        /// <summary>
        /// Record a breach attempt on a door and check if it should unlock
        /// </summary>
        /// <param name="doorId">The unique ID of the door</param>
        /// <param name="category">Breach-difficulty category resolved from the door's material</param>
        /// <param name="materialName">Raw material name, used only for the first-kick debug log</param>
        /// <returns>True if the door should be unlocked (threshold reached)</returns>
        public static bool RecordBreach(string doorId, DoorMaterialCategory category, string materialName)
        {
            if (string.IsNullOrEmpty(doorId))
                return false;

            var now = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(Plugin.BreachTimeout.Value);

            if (!_doorBreaches.TryGetValue(doorId, out var data))
            {
                // First kick on this door this raid: roll its threshold once and store it.
                // Every later kick reuses the stored roll — no re-rolling, so the door can't
                // get easier or harder mid-sequence.
                data = new BreachData { Count = 0, LastBreachTime = now, RolledThreshold = RollThreshold(category) };
                _doorBreaches[doorId] = data;
                if (Plugin.RandomizeBreaches.Value)
                {
                    Plugin.Log?.LogDebug(
                        $"[ForcibleEntry] Door {doorId} rolled threshold {data.RolledThreshold} (material {materialName}, category {category})");
                }
            }
            else if (now - data.LastBreachTime > timeout)
            {
                // Breach window expired: the kick sequence restarts, but the rolled threshold is
                // kept — the door didn't change material because the player took a break.
                data.Count = 0;
            }

            data.Count++;
            data.LastBreachTime = now;

            // When randomization is off, this is exactly the pre-1.1.0 behavior: the fixed
            // BreachesToUnlock value, read live so mid-raid config changes still apply.
            var threshold = Plugin.RandomizeBreaches.Value ? data.RolledThreshold : Plugin.BreachesToUnlock.Value;
            Plugin.Log?.LogDebug($"[ForcibleEntry] Breach {data.Count}/{threshold} on door {doorId}");

            if (data.Count >= threshold)
            {
                // Threshold reached: clear the counter and authorize KickOpen to unlock this door.
                _doorBreaches.Remove(doorId);
                _authorizedDoors.Add(doorId);
                Plugin.Log?.LogDebug($"[ForcibleEntry] Door {doorId} FORCED OPEN after {threshold} breaches!");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Rolls a breach threshold from the configured min/max range for the given category.
        /// Inverted user config (min > max) is swapped rather than thrown on, so a bad edit
        /// in the cfg file can't take the feature down mid-raid.
        /// </summary>
        private static int RollThreshold(DoorMaterialCategory category)
        {
            int min, max;
            switch (category)
            {
                case DoorMaterialCategory.Flimsy:
                    min = Plugin.FlimsyBreachesMin.Value;
                    max = Plugin.FlimsyBreachesMax.Value;
                    break;
                case DoorMaterialCategory.Reinforced:
                    min = Plugin.ReinforcedBreachesMin.Value;
                    max = Plugin.ReinforcedBreachesMax.Value;
                    break;
                default:
                    min = Plugin.SturdyBreachesMin.Value;
                    max = Plugin.SturdyBreachesMax.Value;
                    break;
            }

            if (max < min)
            {
                (min, max) = (max, min);
            }

            return _random.Next(min, max + 1);
        }

        /// <summary>
        /// Returns true exactly once if this door reached its breach threshold and is cleared to
        /// unlock, consuming the authorization so a single kick can only ever open one door once.
        /// KickOpenPatch gates its unlock on this rather than re-deriving intent from DoorState.
        /// </summary>
        public static bool ConsumeUnlockAuthorization(string doorId)
        {
            if (string.IsNullOrEmpty(doorId))
                return false;

            return _authorizedDoors.Remove(doorId);
        }

        /// <summary>
        /// Clear all breach tracking data
        /// </summary>
        public static void Clear()
        {
            _doorBreaches.Clear();
            _authorizedDoors.Clear();
        }
    }
}
