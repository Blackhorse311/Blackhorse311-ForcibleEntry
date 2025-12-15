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
        private static readonly Dictionary<string, BreachData> _doorBreaches = new Dictionary<string, BreachData>();

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

            var now = DateTime.Now;
            var timeout = TimeSpan.FromSeconds(Plugin.BreachTimeout.Value);
            var threshold = Plugin.BreachesToUnlock.Value;

            if (_doorBreaches.TryGetValue(doorId, out var data))
            {
                // Check if too much time has passed - reset counter
                if (now - data.LastBreachTime > timeout)
                {
                    data.Count = 1;
                    data.LastBreachTime = now;
                    Plugin.Log?.LogDebug($"[ForcibleEntry] Breach timeout - resetting count for door {doorId}. Breach 1/{threshold}");
                    return false;
                }

                // Increment counter
                data.Count++;
                data.LastBreachTime = now;

                Plugin.Log?.LogDebug($"[ForcibleEntry] Breach {data.Count}/{threshold} on door {doorId}");

                // Check if threshold reached
                if (data.Count >= threshold)
                {
                    // Reset for next time
                    _doorBreaches.Remove(doorId);
                    Plugin.Log?.LogDebug($"[ForcibleEntry] Door {doorId} FORCED OPEN after {threshold} breaches!");
                    return true;
                }
            }
            else
            {
                // First breach on this door
                _doorBreaches[doorId] = new BreachData
                {
                    Count = 1,
                    LastBreachTime = now
                };
                Plugin.Log?.LogDebug($"[ForcibleEntry] Breach 1/{threshold} on door {doorId}");
            }

            return false;
        }

        /// <summary>
        /// Get the current breach count for a door
        /// </summary>
        public static int GetBreachCount(string doorId)
        {
            if (_doorBreaches.TryGetValue(doorId, out var data))
            {
                var timeout = TimeSpan.FromSeconds(Plugin.BreachTimeout.Value);
                if (DateTime.Now - data.LastBreachTime <= timeout)
                {
                    return data.Count;
                }
            }
            return 0;
        }

        /// <summary>
        /// Clear all breach tracking data
        /// </summary>
        public static void Clear()
        {
            _doorBreaches.Clear();
        }

        /// <summary>
        /// Reset breach count for a specific door
        /// </summary>
        public static void ResetDoor(string doorId)
        {
            _doorBreaches.Remove(doorId);
        }
    }
}
