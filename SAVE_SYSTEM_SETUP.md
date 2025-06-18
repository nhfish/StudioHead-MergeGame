# Save System Setup Guide

## Overview
The save/load system for Studio Head includes:
- Binary save format for efficiency and tampering prevention
- Auto-save on scene transitions, app pause/quit
- 45-minute auto-backup system
- Save indicator UI
- Fame system integration
- Comprehensive error handling

## Required Prefabs

### 1. SaveSystem Prefab
Create a GameObject with the `SaveSystem` component:
```
SaveSystem (GameObject)
├── SaveSystem.cs
└── SaveIndicator (reference to SaveIndicator prefab)
```

### 2. FameManager Prefab
Create a GameObject with the `FameManager` component:
```
FameManager (GameObject)
└── FameManager.cs
```

### 3. SaveIndicator Prefab
Create a UI GameObject with the `SaveIndicator` component:
```
SaveIndicator (GameObject)
├── Canvas (child)
│   ├── Canvas.cs
│   └── CanvasScaler.cs
│   └── GraphicRaycaster.cs
│   └── SaveIndicatorImage (child)
│       ├── Image.cs
│       └── SaveIndicator.cs
```

**SaveIndicator Setup:**
- Position in top-right corner (e.g., Anchor: Top-Right, Offset: -20, -20)
- Small size (e.g., 32x32 pixels)
- Use a simple icon (gear, disk, or loading spinner)
- Set `indicatorImage` reference in the SaveIndicator component

### 4. GameInitializer Prefab
Create a GameObject with the `GameInitializer` component:
```
GameInitializer (GameObject)
└── GameInitializer.cs
```

## Scene Setup

### MainMenu Scene
1. Add `GameInitializer` prefab to the scene
2. Assign references in GameInitializer:
   - SaveSystem Prefab
   - FameManager Prefab  
   - SaveIndicator Prefab
3. Update MainMenuUI:
   - Add Continue and New Game buttons
   - Assign GameInitializer reference
   - Update button visibility logic

### GameMain Scene
1. Add `GameInitializer` prefab to the scene (if not already present)
2. Ensure all managers are properly initialized
3. Test save/load functionality

## Testing

### Manual Save/Load
```csharp
// Save game
SaveSystem.Instance.SaveGame();

// Load game  
SaveSystem.Instance.LoadGame();

// Check if save exists
bool hasSave = SaveSystem.Instance.HasSaveData();
```

### Auto-Save Events
- Scene transitions (automatic)
- Application pause (mobile)
- Application quit
- Application focus loss

### Backup System
- Creates backup every 45 minutes
- Backup file: `studiohead_backup.dat`
- Main save file: `studiohead_save.dat`

## Save Data Structure

The system saves:
- **Economy**: Money, Gems, Tickets
- **Grid State**: All items and their positions
- **Inventory Overflow**: Stored items and slot count
- **Fame**: Level and progress
- **Universal Crate Timer**: Spawn timing
- **Version Control**: Save version and timestamp

## Error Handling

The system includes:
- Graceful degradation on save failures
- Backup recovery if main save is corrupted
- Temporary file writing to prevent corruption
- Comprehensive logging for debugging

## File Locations

**Windows:**
```
%USERPROFILE%/AppData/LocalLow/[CompanyName]/[ProductName]/
```

**iOS:**
```
/var/mobile/Containers/Data/Application/[AppID]/Documents/
```

**Android:**
```
/storage/emulated/0/Android/data/[PackageName]/files/
```

## Integration Notes

### Adding New Save Data
1. Update `SaveData.cs` with new fields
2. Update `SaveSystem.CollectGameData()` to collect the data
3. Update `SaveSystem.ApplyGameData()` to restore the data
4. Update binary serialization in `WriteSaveData()` and `ReadSaveData()`
5. Increment `saveVersion` for save format changes

### Fame System Integration
The FameManager is included as a placeholder. To expand:
1. Add fame rewards to production/distribution systems
2. Create UI to display fame level and progress
3. Add fame-based unlocks or bonuses

## Performance Considerations

- Save operations are asynchronous to prevent frame drops
- Binary format minimizes file size
- Auto-save frequency is configurable
- Save indicator provides user feedback

## Security Notes

- Binary format makes tampering difficult
- No encryption (can be added if needed)
- Backup system provides corruption recovery
- File validation prevents loading corrupted saves 