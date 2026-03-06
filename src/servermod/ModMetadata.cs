// ============================================================================
// Forcible Entry - Mod Metadata
// ============================================================================
// This class provides metadata required by the SPT server to load the mod.
// SPT server mods must include a class that inherits from AbstractModMetadata.
//
// AUTHOR: Blackhorse311
// LICENSE: MIT
// ============================================================================

using System.Reflection;
using SPTarkov.Server.Core.Models.Spt.Mod;

namespace Blackhorse311.ForcibleEntry.Server;

/// <summary>
/// Mod metadata required by SPT server to load and identify the mod.
/// </summary>
public record ModMetadata : AbstractModMetadata
{
    /// <summary>
    /// Unique identifier for this mod.
    /// </summary>
    public override string ModGuid { get; init; } = "blackhorse311.forcibleentry";

    /// <summary>
    /// Human-readable name of the mod.
    /// </summary>
    public override string Name { get; init; } = "Forcible Entry (Server)";

    /// <summary>
    /// Mod author.
    /// </summary>
    public override string Author { get; init; } = "Blackhorse311";

    /// <summary>
    /// Current mod version using semantic versioning.
    /// </summary>
    public override SemanticVersioning.Version Version { get; init; } =
        new(Assembly.GetExecutingAssembly().GetName().Version?.ToString(3) ?? "0.1.0");

    /// <summary>
    /// Compatible SPT version range using semantic versioning.
    /// ~4.0.0 means compatible with 4.0.x versions.
    /// </summary>
    public override SemanticVersioning.Range SptVersion { get; init; } = new("~4.0.0");

    /// <summary>
    /// URL to the mod's homepage or repository.
    /// </summary>
    public override string? Url { get; init; } = null;

    /// <summary>
    /// License under which the mod is distributed.
    /// </summary>
    public override string License { get; init; } = "MIT";

    /// <summary>
    /// List of contributors to the mod.
    /// </summary>
    public override List<string>? Contributors { get; init; } = null;

    /// <summary>
    /// List of incompatible mods by their GUIDs.
    /// </summary>
    public override List<string>? Incompatibilities { get; init; } = null;

    /// <summary>
    /// Dictionary of required mod dependencies and their version ranges.
    /// </summary>
    public override Dictionary<string, SemanticVersioning.Range>? ModDependencies { get; init; } = null;

    /// <summary>
    /// Whether this mod includes Unity asset bundles.
    /// </summary>
    public override bool? IsBundleMod { get; init; } = false;
}
