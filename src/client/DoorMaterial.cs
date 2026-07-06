using System;
using EFT.Ballistics;
using EFT.Interactive;

namespace Blackhorse311.ForcibleEntry
{
    /// <summary>
    /// Breach-difficulty category a door falls into based on its physical material.
    /// Kept as a plain enum so BreachTracker can consume it without referencing
    /// UnityEngine or EFT types (keeps the tracker plainly testable).
    /// </summary>
    public enum DoorMaterialCategory
    {
        Flimsy,
        Sturdy,
        Reinforced
    }

    /// <summary>
    /// Resolves a door's ballistic surface material into a breach-difficulty category.
    /// All UnityEngine/EFT access lives here so BreachTracker stays engine-free.
    /// </summary>
    public static class DoorMaterialResolver
    {
        /// <summary>
        /// Reads the door's material from its BallisticCollider and maps it to a category.
        /// Any failure (no collider, destroyed object, game update moved the member) falls
        /// back to Sturdy so a broken lookup can never make every door trivially easy.
        /// </summary>
        /// <param name="door">The door being breached.</param>
        /// <param name="material">The raw material found, or None if unreadable (for logging).</param>
        public static DoorMaterialCategory Resolve(Door door, out MaterialType material)
        {
            material = MaterialType.None;
            try
            {
                // Unity fake-null: destroyed Components compare == null but are not
                // reference-null, so this must be the UnityEngine.Object overload.
                if (door == null)
                    return DoorMaterialCategory.Sturdy;

                // RE note (Assembly-CSharp, SPT 4.0.13 / EFT 0.16.9, verified via Cecil 2026-07-05):
                // EFT.Ballistics.BallisticCollider is a MonoBehaviour living on the door's collider
                // GameObjects (children of the Door), exposing public MaterialType TypeOfMaterial.
                // Take the first collider with a real material; a door's panel and frame share one.
                var ballistics = door.GetComponentsInChildren<BallisticCollider>(true);
                foreach (var ballistic in ballistics)
                {
                    if (ballistic != null && ballistic.TypeOfMaterial != MaterialType.None)
                    {
                        material = ballistic.TypeOfMaterial;
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                Plugin.Log?.LogWarning(
                    $"[ForcibleEntry] Could not read door material (probably a game update — check for a mod update). Treating as Sturdy: {ex.Message}");
            }

            return Categorize(material);
        }

        /// <summary>
        /// Groups the 39 MaterialType values into three door-difficulty buckets. Only materials
        /// a door collider plausibly carries are mapped; everything else (terrain, body armor,
        /// None/unreadable) lands in Sturdy so odd data behaves like a normal door.
        /// </summary>
        public static DoorMaterialCategory Categorize(MaterialType material)
        {
            switch (material)
            {
                case MaterialType.Cardboard:
                case MaterialType.Fabric:
                case MaterialType.GenericSoft:
                case MaterialType.Glass:
                case MaterialType.GlassShattered:
                case MaterialType.Plastic:
                case MaterialType.WoodThin:
                    return DoorMaterialCategory.Flimsy;

                case MaterialType.Concrete:
                case MaterialType.GenericHard:
                case MaterialType.MetalNoDecal:
                case MaterialType.MetalThick:
                case MaterialType.MetalThin:
                case MaterialType.Stone:
                    return DoorMaterialCategory.Reinforced;

                default:
                    return DoorMaterialCategory.Sturdy;
            }
        }
    }
}
