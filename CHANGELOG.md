# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.3] - 2026-03-06

### Changed
- Disabled server mod auto-deploy (server component not needed, mod is client-side only)
- Removed server mod DLL from SPT installation (was causing icon-not-found warnings on server start)

---

## [1.0.2] - 2026-03-05

### Fixed
- Locked doors no longer bypass threshold via vanilla RNG (first kick could open door on lucky roll)
- `CanBeBreached` field is now restored after interaction check (no permanent door state mutation)
- Breach tracker now clears at raid start (stale progress no longer carries between raids)
- Null-safe `OnDestroy` catch block prevents secondary exception on plugin unload
- Removed unnecessary `AllowUnsafeBlocks` from build config

### Fixed (Server Mod)
- Hardcoded version string replaced with assembly version
- Trader service skips items that failed to create (prevents referencing nonexistent templates)
- Trader service idempotency guard prevents duplicate entries on double-load
- Icon service validates assembly location before building file paths
- Updated SPT NuGet packages from 4.0.8 to 4.0.13
- Fixed nullable warning on `ModMetadata.License`

---

## [1.0.1] - 2026-03-05

### Fixed
- Updated for SPT 4.0.13 compatibility
- `BreachSuccessRoll` patch updated to match new `Vector3` parameter signature
- `CanBeBreached` patch reworked: field replaced property, now set via `GetInteractionParameters` prefix
- `KickOpen` patch updated for new overload `KickOpen(Vector3, bool)`
- Removed reflection hack for `_doorState` field, uses public `DoorState` setter

---

## [1.0.0] - 2024-12-15

### Added
- Initial release
- Ability to force open any locked door by breaching it multiple times
- Configurable breach count threshold (default: 5 breaches)
- Configurable timeout before breach counter resets (default: 10 seconds)
- Breach action now available on locked doors (not just unlocked/shut doors)

### Technical Details
- Patches `Door.BreachSuccessRoll` to track breach attempts per door
- Patches `Door.CanBeBreached` to enable breach action on locked doors
- Patches `Door.KickOpen` to handle unlocking locked doors after forced breach
