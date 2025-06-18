using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance { get; private set; }

    [Header("Save Settings")]
    public string saveFileName = "studiohead_save.dat";
    public string backupFileName = "studiohead_backup.dat";
    public float autoBackupInterval = 2700f; // 45 minutes in seconds

    [Header("Save Indicator")]
    public GameObject saveIndicatorPrefab;
    private GameObject saveIndicator;

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);
    private string BackupPath => Path.Combine(Application.persistentDataPath, backupFileName);
    
    private float lastBackupTime;
    private bool isSaving = false;
    private bool isLoading = false;

    public System.Action SaveCompleted;
    public System.Action LoadCompleted;
    public System.Action<string> SaveError;
    public System.Action<string> LoadError;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Create save indicator
        if (saveIndicatorPrefab != null)
        {
            saveIndicator = Instantiate(saveIndicatorPrefab);
            saveIndicator.SetActive(false);
            DontDestroyOnLoad(saveIndicator);
        }

        // Subscribe to scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
        
        // Subscribe to application pause/quit
        Application.quitting += OnApplicationQuit;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Application.quitting -= OnApplicationQuit;
    }

    void Update()
    {
        // Check for auto-backup
        if (Time.time - lastBackupTime >= autoBackupInterval)
        {
            CreateBackup();
            lastBackupTime = Time.time;
        }
    }

    public void SaveGame()
    {
        if (isSaving) return;

        StartCoroutine(SaveGameCoroutine());
    }

    private System.Collections.IEnumerator SaveGameCoroutine()
    {
        isSaving = true;
        ShowSaveIndicator(true);

        try
        {
            SaveData saveData = CollectGameData();
            
            // Save to temporary file first
            string tempPath = SavePath + ".tmp";
            bool saveSuccess = WriteSaveData(saveData, tempPath);
            
            if (saveSuccess)
            {
                // If successful, replace the main save file
                if (File.Exists(SavePath))
                    File.Delete(SavePath);
                File.Move(tempPath, SavePath);
                
                Debug.Log("Game saved successfully");
                SaveCompleted?.Invoke();
            }
            else
            {
                throw new Exception("Failed to write save data");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Save failed: {e.Message}");
            SaveError?.Invoke($"Save failed: {e.Message}");
        }
        finally
        {
            isSaving = false;
            ShowSaveIndicator(false);
        }

        yield return null;
    }

    public void LoadGame()
    {
        if (isLoading) return;

        StartCoroutine(LoadGameCoroutine());
    }

    private System.Collections.IEnumerator LoadGameCoroutine()
    {
        isLoading = true;
        ShowSaveIndicator(true);

        try
        {
            SaveData saveData = null;
            
            // Try to load main save file
            if (File.Exists(SavePath))
            {
                saveData = ReadSaveData(SavePath);
            }
            
            // If main save failed or doesn't exist, try backup
            if (saveData == null && File.Exists(BackupPath))
            {
                Debug.LogWarning("Main save file corrupted or missing, loading from backup");
                saveData = ReadSaveData(BackupPath);
            }
            
            // If still no data, start fresh
            if (saveData == null)
            {
                Debug.Log("No save data found, starting fresh game");
                LoadCompleted?.Invoke();
                yield break;
            }

            ApplyGameData(saveData);
            Debug.Log("Game loaded successfully");
            LoadCompleted?.Invoke();
        }
        catch (Exception e)
        {
            Debug.LogError($"Load failed: {e.Message}");
            LoadError?.Invoke($"Load failed: {e.Message}");
        }
        finally
        {
            isLoading = false;
            ShowSaveIndicator(false);
        }

        yield return null;
    }

    private void CreateBackup()
    {
        try
        {
            if (File.Exists(SavePath))
            {
                File.Copy(SavePath, BackupPath, true);
                Debug.Log("Backup created successfully");
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"Backup creation failed: {e.Message}");
        }
    }

    private SaveData CollectGameData()
    {
        SaveData data = new SaveData();

        // Economy
        if (EconomyManager.Instance != null)
        {
            data.currency[CurrencyType.Money] = EconomyManager.Instance.GetAmount(CurrencyType.Money);
            data.currency[CurrencyType.Gems] = EconomyManager.Instance.GetAmount(CurrencyType.Gems);
            data.currency[CurrencyType.Tickets] = EconomyManager.Instance.GetAmount(CurrencyType.Tickets);
        }

        // Grid State
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            // Collect all items on the grid
            for (int x = 0; x < gridManager.columns; x++)
            {
                for (int y = 0; y < gridManager.rows; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    IGridOccupant occupant = gridManager.GetTileAt(gridPos);
                    
                    if (occupant is MergeTile mergeTile && mergeTile.data != null)
                    {
                        GridItemData gridItem = new GridItemData
                        {
                            gridPosition = mergeTile.CurrentGridPosition,
                            itemId = mergeTile.data.name,
                            department = mergeTile.data.department,
                            tier = mergeTile.data.tier
                        };
                        data.gridItems.Add(gridItem);
                    }
                }
            }
        }

        // Inventory Overflow
        if (InventoryOverflowManager.Instance != null)
        {
            data.overflowSlots = InventoryOverflowManager.Instance.MaxSlots;
            foreach (var item in InventoryOverflowManager.Instance.StoredItems)
            {
                data.overflowItems.Add(item);
            }
        }

        // Fame
        if (FameManager.Instance != null)
        {
            data.fameLevel = FameManager.Instance.currentFameLevel;
            data.fameProgress = FameManager.Instance.fameProgress;
        }

        // Universal Crate Timer
        UniversalCrateSystem crateSystem = FindFirstObjectByType<UniversalCrateSystem>();
        if (crateSystem != null)
        {
            data.universalCrateTimer = crateSystem.timeSinceLastSpawn;
        }

        return data;
    }

    private void ApplyGameData(SaveData data)
    {
        // Economy
        if (EconomyManager.Instance != null)
        {
            // Reset currency first
            EconomyManager.Instance.Spend(CurrencyType.Money, EconomyManager.Instance.GetAmount(CurrencyType.Money));
            EconomyManager.Instance.Spend(CurrencyType.Gems, EconomyManager.Instance.GetAmount(CurrencyType.Gems));
            EconomyManager.Instance.Spend(CurrencyType.Tickets, EconomyManager.Instance.GetAmount(CurrencyType.Tickets));
            
            // Apply saved currency
            foreach (var currency in data.currency)
            {
                EconomyManager.Instance.Add(currency.Key, currency.Value);
            }
        }

        // Grid State - Clear existing grid first
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            // Clear existing grid items
            for (int x = 0; x < gridManager.columns; x++)
            {
                for (int y = 0; y < gridManager.rows; y++)
                {
                    Vector2Int gridPos = new Vector2Int(x, y);
                    IGridOccupant occupant = gridManager.GetTileAt(gridPos);
                    
                    if (occupant is MergeTile mergeTile)
                    {
                        gridManager.UnregisterTile(gridPos);
                        Destroy(mergeTile.gameObject);
                    }
                }
            }
            
            // Restore saved grid items
            foreach (var gridItem in data.gridItems)
            {
                // Load the item data from Resources
                DepartmentItemData itemData = Resources.Load<DepartmentItemData>($"DepartmentItems/{gridItem.itemId}");
                if (itemData != null)
                {
                    // Create the merge tile at the saved position
                    Vector3 worldPos = gridManager.GetWorldPosition(gridItem.gridPosition);
                    GameObject tilePrefab = Resources.Load<GameObject>("Prefabs/Tiles/MergeTile");
                    if (tilePrefab != null)
                    {
                        GameObject newTile = Instantiate(tilePrefab, worldPos, Quaternion.identity, gridManager.tileParent);
                        MergeTile mergeTile = newTile.GetComponent<MergeTile>();
                        if (mergeTile != null)
                        {
                            mergeTile.data = itemData;
                            mergeTile.SetCurrentGridPosition(gridItem.gridPosition);
                            mergeTile.ApplyVisuals();
                            gridManager.RegisterTile(gridItem.gridPosition, mergeTile);
                        }
                    }
                }
            }
        }

        // Inventory Overflow
        if (InventoryOverflowManager.Instance != null)
        {
            // Clear existing overflow
            var existingItems = new List<Item>(InventoryOverflowManager.Instance.StoredItems);
            foreach (var item in existingItems)
            {
                InventoryOverflowManager.Instance.DiscardItem(item);
            }
            
            // Restore saved overflow items
            foreach (var item in data.overflowItems)
            {
                InventoryOverflowManager.Instance.AddItem(item);
            }
        }

        // Fame
        if (FameManager.Instance != null)
        {
            FameManager.Instance.SetFameLevel(data.fameLevel, data.fameProgress);
        }

        // Universal Crate Timer
        UniversalCrateSystem crateSystem = FindFirstObjectByType<UniversalCrateSystem>();
        if (crateSystem != null)
        {
            crateSystem.timeSinceLastSpawn = data.universalCrateTimer;
        }
    }

    private bool WriteSaveData(SaveData data, string path)
    {
        try
        {
            using (FileStream stream = new FileStream(path, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(stream))
                {
                    // Write save version
                    writer.Write(data.saveVersion);
                    
                    // Write timestamp
                    writer.Write(data.lastSaveTime.ToBinary());
                    
                    // Write currency
                    writer.Write(data.currency.Count);
                    foreach (var currency in data.currency)
                    {
                        writer.Write((int)currency.Key);
                        writer.Write(currency.Value);
                    }
                    
                    // Write grid items
                    writer.Write(data.gridItems.Count);
                    foreach (var gridItem in data.gridItems)
                    {
                        writer.Write(gridItem.gridPosition.x);
                        writer.Write(gridItem.gridPosition.y);
                        writer.Write(gridItem.itemId ?? "");
                        writer.Write((int)gridItem.department);
                        writer.Write((int)gridItem.tier);
                    }
                    
                    // Write overflow items
                    writer.Write(data.overflowItems.Count);
                    foreach (var item in data.overflowItems)
                    {
                        writer.Write(item.departmentName ?? "");
                        writer.Write(item.tier);
                        writer.Write(item.baseValue);
                    }
                    
                    // Write overflow slots
                    writer.Write(data.overflowSlots);
                    
                    // Write fame data
                    writer.Write(data.fameLevel);
                    writer.Write(data.fameProgress);
                    
                    // Write universal crate timer
                    writer.Write(data.universalCrateTimer);
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to write save data: {e.Message}");
            return false;
        }
    }

    private SaveData ReadSaveData(string path)
    {
        try
        {
            SaveData data = new SaveData();
            
            using (FileStream stream = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    // Read save version
                    data.saveVersion = reader.ReadInt32();
                    
                    // Read timestamp
                    data.lastSaveTime = DateTime.FromBinary(reader.ReadInt64());
                    
                    // Read currency
                    int currencyCount = reader.ReadInt32();
                    for (int i = 0; i < currencyCount; i++)
                    {
                        CurrencyType type = (CurrencyType)reader.ReadInt32();
                        int amount = reader.ReadInt32();
                        data.currency[type] = amount;
                    }
                    
                    // Read grid items
                    int gridItemCount = reader.ReadInt32();
                    for (int i = 0; i < gridItemCount; i++)
                    {
                        GridItemData gridItem = new GridItemData
                        {
                            gridPosition = new Vector2Int(reader.ReadInt32(), reader.ReadInt32()),
                            itemId = reader.ReadString(),
                            department = (DepartmentType)reader.ReadInt32(),
                            tier = (DepartmentItemTier)reader.ReadInt32()
                        };
                        data.gridItems.Add(gridItem);
                    }
                    
                    // Read overflow items
                    int overflowItemCount = reader.ReadInt32();
                    for (int i = 0; i < overflowItemCount; i++)
                    {
                        Item item = new Item
                        {
                            departmentName = reader.ReadString(),
                            tier = reader.ReadInt32(),
                            baseValue = reader.ReadInt32()
                        };
                        data.overflowItems.Add(item);
                    }
                    
                    // Read overflow slots
                    data.overflowSlots = reader.ReadInt32();
                    
                    // Read fame data
                    data.fameLevel = reader.ReadInt32();
                    data.fameProgress = reader.ReadSingle();
                    
                    // Read universal crate timer
                    data.universalCrateTimer = reader.ReadSingle();
                }
            }
            return data;
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to read save data: {e.Message}");
            return null;
        }
    }

    private void ShowSaveIndicator(bool show)
    {
        if (saveIndicator != null)
        {
            saveIndicator.SetActive(show);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Auto-save on scene transitions
        SaveGame();
    }

    private void OnApplicationQuit()
    {
        // Save on application quit
        SaveGame();
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // Save when app is paused (mobile)
            SaveGame();
        }
    }

    void OnApplicationFocus(bool hasFocus)
    {
        if (!hasFocus)
        {
            // Save when app loses focus
            SaveGame();
        }
    }

    public bool HasSaveData()
    {
        return File.Exists(SavePath) || File.Exists(BackupPath);
    }

    public DateTime GetLastSaveTime()
    {
        if (File.Exists(SavePath))
        {
            try
            {
                using (FileStream stream = new FileStream(SavePath, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(stream))
                {
                    reader.ReadInt32(); // Skip version
                    return DateTime.FromBinary(reader.ReadInt64());
                }
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        return DateTime.MinValue;
    }
} 