# Exile UI

A native Windows overlay toolkit for Path of Exile 1 and 2. No dependencies — single `.exe`, no AutoHotkey required.

## Requirements

- Windows 10/11 (x64)
- Path of Exile or Path of Exile 2

## Installation

1. Download `ExileUI.exe`
2. Place it anywhere (e.g. your PoE folder or desktop)
3. Launch the exe **before or after** starting the game

The app waits silently in the system tray until it detects a running PoE client.

## Usage

- **Start the game** — Exile UI detects it automatically and activates
- **System tray icon** — right-click for Settings or Exit
- **Hotkeys** — configurable per feature (see Settings)
- **Close the game** — Exile UI suspends itself and waits again
- **Exit** — right-click the tray icon → Exit

## Features

| Feature | Description |
|---|---|
| **Clone Frames** | Mirror and reposition screen regions (cooldowns, rage meter, charges) |
| **Item Checker** | Clipboard-based item tooltip with mod analysis |
| **Map Info** | Map mod display with custom rankings |
| **Map Tracker** | Session statistics, loot logging, and data export |
| **Act Tracker** | Campaign guide overlay with gem setup and skill tree |
| **Act Decoder** | Interactive zone layout overlay |
| **Stash Ninja** | poe.ninja price data displayed in your stash |
| **Sanctum Planner** | Floor scanner, path planner, and relic manager |
| **Chat Macros** | Chat wheel with customizable macros |
| **Exchange** | Vaal Street currency exchange tracker |
| **Anoints** | Blight oil anoint calculator |
| **Betrayal Info** | Betrayal board tracker |
| **Cheat Sheets** | Custom context-sensitive overlay images |
| **Search Strings** | In-game search manager for vendors, beasts, Gwennen |
| **Recombination** | Item recombination outcome simulator |
| **Statlas** | PoE 2 atlas overlay with boss and layout reference |
| **OCR Tools** | On-screen text recognition overlay |
| **QoL Tools** | In-game notepad, alarm timer, lab layout overlay |
| **Loot Filter** | In-game loot filter management |
| **Seed Explorer** | Legion timeless jewel seed explorer |

## Configuration

Settings are stored as JSON files in the `ini/` folder next to the exe. Each feature has its own file (e.g. `ini/MapInfo.json`). You can edit these directly or use the in-app Settings menu.

## Data Files

The `data/` and `img/` folders must remain next to the exe. These contain game data (maps, gems, items, zone layouts) and UI images used by the overlay features.

## Building from Source

Requires [.NET 9 SDK](https://dot.net).

```
git clone https://github.com/kobayashi-maru-1/ExileUI
cd ExileUI
dotnet publish src/ExileUI/ExileUI.csproj -c Release -r win-x64
```

Output: `src/ExileUI/bin/Release/net9.0-windows/win-x64/publish/ExileUI.exe`

The published exe is fully self-contained — no .NET runtime installation needed on the target machine.

## License

See [LICENSE.md](../../LICENSE.md).
