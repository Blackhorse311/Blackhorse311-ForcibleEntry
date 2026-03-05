# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
