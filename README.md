# Blackhorse311-ForcibleEntry

A BepInEx mod for Single Player Tarkov that allows players to force open any locked door by breaching it multiple times.

## Features

- Force open **any locked door** by using the breach (kick) action repeatedly
- Configurable number of breach attempts required (default: 5)
- Configurable timeout before breach counter resets (default: 10 seconds)
- No special items required - uses the existing breach mechanic

## Requirements

- SPT 4.0.x
- BepInEx 5.4.x (included with SPT)

## Installation

1. Download the latest release
2. Extract the archive contents to your SPT installation folder
3. The folder structure should look like:
   ```
   SPT/
   └── BepInEx/
       └── plugins/
           └── Blackhorse311-ForcibleEntry/
               └── Blackhorse311.ForcibleEntry.dll
   ```
4. Start SPT and play!

## How to Use

1. Find a locked door in-raid
2. Use the breach action (kick) on the door
3. Repeat the breach action 5 times consecutively (within 10 seconds)
4. On the 5th successful breach, the door will be forced open

## Configuration

Configuration options are available in BepInEx config after first run:

| Setting | Default | Description |
|---------|---------|-------------|
| `BreachesToUnlock` | 5 | Number of consecutive breach attempts required to force open a locked door (1-20) |
| `BreachTimeout` | 10.0 | Time in seconds before breach counter resets if you stop breaching (5-60) |

Config file location: `BepInEx/config/com.blackhorse311.forcibleentry.cfg`

## Uninstallation

1. Delete the `BepInEx/plugins/Blackhorse311-ForcibleEntry` folder
2. Optionally delete `BepInEx/config/com.blackhorse311.forcibleentry.cfg`

## Compatibility

- This is a **client-side only** mod - no server mod required
- Should be compatible with most other mods
- Does not modify any game files permanently

## Source Code

Source code is available at: [GitHub Repository URL]

## License

This mod is released under the MIT License. See [LICENSE](LICENSE) for details.

## Credits

- **Author:** Blackhorse311
- **Special Thanks:** The SPT development team and modding community

## Changelog

See [CHANGELOG.md](CHANGELOG.md) for version history.
