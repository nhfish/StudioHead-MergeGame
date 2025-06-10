# 🎬 Studio Head

A 2D top-down **merge + idle** game prototype set inside a chaotic film production studio. Players drag-and-merge props, costumes, gear, and talent across a stylized workspace to create short films and grow their studio empire.

Built with **Unity 6.2 (2023 LTS)** and supports mobile-first platforms (iOS/Android).

---

## 🚀 Features

✅ Drag-and-drop merge mechanics  
✅ Tiered item trees across departments  
✅ Grid snapping + occupancy detection  
✅ ScriptableObject-based item data  
✅ Merge rejection handling (snap-back)  
✅ Modular, scalable project layout  
✅ Touch-friendly and mobile responsive
✅ Multi-currency economy and shop interface
✅ Production and distribution pipeline ([ProductionManager](./Assets/_Game/Scripts/Managers/ProductionManager.cs) / [DistributionQueueManager](./Assets/_Game/Scripts/Managers/DistributionQueueManager.cs))
✅ Improved crate spawner that finds a free cell
✅ Talent inventory / card system
✅ Main Menu with Settings overlay
✅ Inventory overflow slots for storing extra items off the grid, expandable via currency ([InventoryOverflowManager](./Assets/_Game/Scripts/Managers/InventoryOverflowManager.cs))
✅ Universal crate system that periodically spawns department crates across the grid ([UniversalCrateSystem](./Assets/_Game/Scripts/Managers/UniversalCrateSystem.cs))
✅ Synergy bonuses when writer, director, and actor share the same genre ([MovieRecipe](./Assets/_Game/Scripts/Data/MovieRecipe.cs), [SynergyBonusConfig](./Assets/_Game/Scripts/Data/SynergyBonusConfig.cs))

---

## 🧠 Current Systems Implemented

- `GridManager` manages a 7x9 tile-based workspace with occupancy rules.
- `MergeTile` handles input, snapping, merging, and revert logic.
- `MergeItemData` ScriptableObject defines each item’s tier, type, and evolution path.
- Tier-based color system for quick visual recognition.
- Support for Unity Visual Scripting **or** full C# logic.

---

## 📁 Project Structure

StudioHead-MergeGame/
├── Assets/
│   └── _Game/
│       ├── Scripts/
│       ├── Prefabs/
│       ├── Scenes/
│       └── ScriptableObjects/
├── GeneratedAssets/
├── Packages/
├── ProjectSettings/
├── README.md                ← 👋 Welcome doc for new contributors
├── DESIGNLOG.md             ← 📜 Running design decisions
├── .gitignore               ← 🧹 Prevent junk from polluting repo
└── ignore.conf

---

## 📲 Mobile-Ready Setup

- Canvas Scaler set to `Scale with Screen Size`
- Touch + mouse input both supported
- `Input.GetTouch()` and `Input.mousePosition` logic inside `MergeTile.cs`

---

## 🛠️ Requirements

- Unity 6.2 LTS or 2023.3+
- TextMeshPro (included in Unity Package Manager)
- Unity Visual Scripting (optional but supported)
- GitHub repo tracking enabled
- Mobile build module (iOS/Android)

---

## 🗓️ Roadmap

| Milestone | Status |
|-----------|--------|
| Grid logic + merging | ✅ Complete |
| Merge rejection logic | ✅ Complete |
| Timed tile spawner | ⏳ In progress |
| Movie Recipe System | ⏳ Design stage |
| Talent/crew management | 🔜 Next |
| Save/load system | 🔜 Planned |
| Full merge trees x4 departments | 🔜 Planned |

---

## 🎨 Art Style

- Top-down 2D  
- Flat-color stylization (AI-generated placeholders currently in use)  
- Merge tiers use color-coded backgrounds:
  - Tier 1: Gray → Tier 10: Gold
- All items have layered depth via `SpriteRenderer.sortingOrder`

---

## 📦 Setup Instructions

1. Clone this repo
2. Open in Unity 6.2 or later
3. Open `Scenes/MainMenu.unity` as your starting scene
4. Hit Play to test drag + merge functionality. The CurrencyPanel now appears across the top of the screen.
5. To add new items:
   - Create new `MergeItemData` ScriptableObject
   - Assign sprite, tier, and evolution
   - Drop prefab into scene or spawner logic

---

## 🧪 Dev Tools

| Tool        | Usage                |
|-------------|----------------------|
| GitHub Copilot | Auto-refactor, C# autocomplete |
| Notion / Docs | Track design updates |
| Figma / XD   | UI mockups (future) |
| Midjourney / DALL·E | Placeholder art |

---

## 💬 Contact & Credits

Built by [@nhfish](https://github.com/nhfish)  
Unity assistant powered by [Unity Copilot (ChatGPT)]()  
Game concept: satirical merge sim set in a film studio 🎬

---

## 🧠 See Also

- [`DESIGNLOG.md`](./DESIGNLOG.md) – for changelog, next steps, and roadmap tracking
- [`MergeTile.cs`](./Assets/_Game/Scripts/Merge/MergeTile.cs) – core gameplay logic
- [`GridManager.cs`](./Assets/_Game/Scripts/Grid/GridManager.cs) – snap + occupancy control

---

## 📜 License

MIT (or custom license if needed)  
You’re free to use, remix, or extend this system for prototyping.

UPDATES:

## Economy & Shop UI

06.03.2025 via CODEX
The `EconomyManager` now supports multiple currencies (`Money`, `Gems`, and `Tickets`).
Attach the new `CurrencyPanel` prefab or script to a UI canvas and assign three
`TextMeshProUGUI` fields to display these values. The panel listens for economy
changes and updates automatically.

## Crate Spawner Fix

Latest merge: fixed bug where crate spawner placed crates on occupied cells. It now searches for a free grid cell before spawning.

## Department Crate Shop
Added `CrateShopPanel` with purchase buttons for each department. Buying a crate spends money and spawns a matching crate if space allows.
