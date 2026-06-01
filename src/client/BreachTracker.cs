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

        private class BreachData
        {
            public int Count { get; set; }
            public DateTime LastBreachTime { get; set; }
        }

        /// <summary>
        /// Record a breach attempt on a door and check if it should unlock
        /// </summary>
        /// <param name="doorId">The unique ID of the door</param>
        /// <returns>True if the door should be unlocked (threshold reached)</returns>
        public static bool RecordBreach(string doorId)
        {
            if (string.IsNullOrEmpty(doorId))
                return false;

            var now = DateTime.UtcNow;
            var timeout = TimeSpan.FromSeconds(Plugin.BreachTimeout.Value);
            var threshold = Plugin.BreachesToUnlock.Value;

            // Treat an unseen door, or one whose breach window has expired, as a fresh sequence.
            // Both cases then run the same increment-and-test below, so the threshold is checked
            // on every breach including the first (otherwise threshold == 1 would need two kicks).
            if (!_doorBreaches.TryGetValue(doorId, out var data) || now - data.LastBreachTime > timeout)
            {
                data = new BreachData { Count = 0, LastBreachTime = now };
                _doorBreaches[doorId] = data;
            }

            data.Count++;
            data.LastBreachTime = now;
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
        /// Get the current breach count for a door
        /// </summary>
        public static int GetBreachCount(string doorId)
        {
            if (string.IsNullOrEmpty(doorId))
                return 0;

            if (_doorBreaches.TryGetValue(doorId, out var data))
            {
                var timeout = TimeSpan.FromSeconds(Plugin.BreachTimeout.Value);
                if (DateTime.UtcNow - data.LastBreachTime <= timeout)
                {
                    return data.Count;
                }
                _doorBreaches.Remove(doorId);
            }
            return 0;
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

        /// <summary>
        /// Reset breach count for a specific door
        /// </summary>
        public static void ResetDoor(string doorId)
        {
            if (!string.IsNullOrEmpty(doorId))
            {
                _doorBreaches.Remove(doorId);
                _authorizedDoors.Remove(doorId);
            }
        }
    }
}
