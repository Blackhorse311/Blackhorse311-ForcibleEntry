# Forcible Entry

**Kick it till it opens.** Force breach any locked door in Tarkov.

[![SPT 4.0.13+](https://img.shields.io/badge/SPT-4.0.13+-green.svg)](https://forge.sp-tarkov.com)
[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)

---

## What It Does

Forcible Entry lets you breach any locked door by kicking it repeatedly. No keycard, no key, no problem. Just boot the door enough times and it gives way.

### 30 Seconds to Understand

```
Find locked door → Kick it → Kick it again → ... → Kick #5 → Door forced open
                                                 ↑ timeout resets if you stop
```

---

## Key Features

| Feature | Description |
|---------|-------------|
| **Force Any Door** | Every locked door in the game can be breached open |
| **Configurable Threshold** | Set how many kicks it takes (1-20, default: 5) |
| **Timeout Window** | Breach counter resets if you stop kicking (5-60s, default: 10s) |
| **No Items Required** | Uses the existing breach/kick mechanic, no special tools needed |
| **Vanilla Feel** | Works through the normal door interaction menu |

---

## Quick Start

### Installation

**Automatic (Recommended)**

Install via The Forge mod manager.

**Manual Installation**

1. Download the latest release
2. Extract the archive directly into your SPT root folder (where `SPT.Server.exe` is located)
3. The folder structure should look like:

```
[SPT Root]/
└── BepInEx/
    └── plugins/
        └── Blackhorse311-ForcibleEntry/
            └── Blackhorse311.ForcibleEntry.dll
```

4. Start SPT and play!

---

## How to Use

1. Find a locked door in-raid
2. Use the **breach** (kick) action on the door
3. Keep kicking within the timeout window (default: 10 seconds)
4. On the 5th kick, the door is forced open

The breach counter resets if you stop kicking for longer than the timeout.

---

## Configuration Reference

Access mod settings via **F12** (BepInEx Configuration Manager) or edit the config file directly.

Config file location: `BepInEx/config/com.blackhorse311.forcibleentry.cfg`

| Setting | Default | Range | Description |
|---------|---------|-------|-------------|
| `BreachesToUnlock` | 5 | 1-20 | Number of consecutive breach attempts to force open a locked door |
| `BreachTimeout` | 10.0 | 5-60 | Seconds before breach counter resets if you stop kicking |

---

## Compatibility

| Mod/Version | Status | Notes |
|-------------|--------|-------|
| **SPT 4.0.13+** | Supported | Current target version |
| **SPT 4.0.0-4.0.12** | Use v1.0.0 | API changes in 4.0.13 broke older builds |
| **FIKA** | Untested | Should work, but not verified |
| **Other Door Mods** | Compatible | No conflicts expected with key/lockpick mods |

---

## Troubleshooting

| Problem | Solution |
|---------|----------|
| Breach option not showing on locked doors | Verify the DLL is in the correct plugins folder and check BepInEx log for load errors |
| Door not opening after kicks | Check your `BreachesToUnlock` setting, make sure you're kicking fast enough (within timeout) |
| Mod not loading at all | Confirm you're on SPT 4.0.13+, check `BepInEx/LogOutput.log` for errors |

---

## Building from Source

```bash
git clone https://github.com/Blackhorse311/Blackhorse311-ForcibleEntry.git
cd Blackhorse311-ForcibleEntry
dotnet build -c Release -p:SptPath="<your SPT folder>" -p:NoDeploy=true
```

References to SPT's `Assembly-CSharp.dll`, `BepInEx.dll`, and `0Harmony.dll` are resolved from `SptPath` (or the `SPT_PATH` environment variable). Omit `-p:NoDeploy=true` to copy the built DLL into that SPT install's plugins folder automatically.

---

## Uninstallation

1. Delete the `BepInEx/plugins/Blackhorse311-ForcibleEntry` folder
2. Optionally delete `BepInEx/config/com.blackhorse311.forcibleentry.cfg`

---

## Security and Compliance

This mod is open source and contains no obfuscated code, network calls, or telemetry.

**AI Collaboration Disclosure:** Development assisted by Claude (Anthropic). All code reviewed and approved by the author.

---

## Credits

**Developer:** Blackhorse311

**Community Contributors:** Want to contribute? PRs welcome!

**Special Thanks:** The SPT development team and modding community

---

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for full version history.

### Recent

- **v1.0.3** - Removed unused server mod (was causing icon warnings on server start)
- **v1.0.2** - Edge case fixes, hardened breach logic, raid cleanup
- **v1.0.1** - SPT 4.0.13 compatibility fix
- **v1.0.0** - Initial release

---

## Support

- **Bug Reports:** [Open an issue](https://github.com/Blackhorse311/Blackhorse311-ForcibleEntry/issues/new?template=bug_report.md)
- **Feature Requests:** [Open an issue](https://github.com/Blackhorse311/Blackhorse311-ForcibleEntry/issues/new?template=feature_request.md)

---

## License

This mod is released under the MIT License. See [LICENSE](LICENSE) for details.

## Source Code

Source code is available at: https://github.com/Blackhorse311/Blackhorse311-ForcibleEntry
