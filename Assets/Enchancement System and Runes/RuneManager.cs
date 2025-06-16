using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;
using TMPro;

/// <summary>
/// Manages collected and active runes, and handles JSON save/load.
/// </summary>
public class RuneManager : MonoBehaviour
{
    public static RuneManager Instance { get; private set; }

    [Header("All Runes Database")]
    public List<RuneSO> allRunes;

    [Header("UI References")]
    public Transform activeRunesPanel;   // should have exactly 3 children with RuneSlotUI(slotIndex 0,1,2)
    public Transform inventoryPanel;     // should have a GridLayoutGroup + RuneSlotUI(slotIndex = -1)

    [Header("Prefabs")]
    public GameObject runeIconPrefab;    // prefab with Image + RuneIconUI

    [Header("Combo Description UI")]
    public TextMeshProUGUI comboDescriptionText;

    private List<RuneSO> collectedRunes = new List<RuneSO>();
    private List<RuneSO> activeRunes = new List<RuneSO>();

    private string savePath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        if(Time.timeScale < 1) Time.timeScale = 1;

        savePath = Path.Combine(Application.persistentDataPath, "runes.json");
        LoadRunes();
        RefreshUI();
    }

    #region Public API

    /// <summary>
    /// Call this when player picks up a new rune.
    /// </summary>
    public void CollectRune(string runeID)
    {
        if (collectedRunes.Exists(r => r.id == runeID)) return;
        var s = allRunes.Find(r => r.id == runeID);
        if (s != null)
        {
            collectedRunes.Add(s);
            SaveRunes();
            RefreshUI();
        }
    }

    /// <summary>
    /// Activate a rune in given slot (0..2).
    /// </summary>
    public void SetActiveRune(RuneSO rune, int slotIndex)
    {
        if (!collectedRunes.Contains(rune)) return;
        if (slotIndex < 0 || slotIndex > 2) return;

        // ensure no duplicates
        activeRunes.Remove(rune);

        // fill or override
        if (activeRunes.Count > slotIndex)
            activeRunes[slotIndex] = rune;
        else
        {
            // pad with nulls if needed
            while (activeRunes.Count < slotIndex)
                activeRunes.Add(null);
            activeRunes.Add(rune);
        }

        SaveRunes();
        RefreshUI();
    }

    /// <summary>
    /// Deactivate rune (remove from active list).
    /// </summary>
    public void DeactivateRune(RuneSO rune)
    {
        if (activeRunes.Remove(rune))
        {
            SaveRunes();
            RefreshUI();
        }
    }

    #endregion

    #region Save / Load

    private void SaveRunes()
    {
        var data = new RuneSaveData();
        foreach (var r in collectedRunes) data.collectedRuneIDs.Add(r.id);
        foreach (var r in activeRunes) data.activeRuneIDs.Add(r.id);

        string json = JsonUtility.ToJson(data, prettyPrint: true);
        File.WriteAllText(savePath, json);
    }

    private void LoadRunes()
    {
        if (!File.Exists(savePath)) return;

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<RuneSaveData>(json);

        // rebuild collectedRunes
        collectedRunes.Clear();
        foreach (var id in data.collectedRuneIDs)
        {
            var s = allRunes.Find(r => r.id == id);
            if (s != null) collectedRunes.Add(s);
        }

        // rebuild activeRunes
        activeRunes.Clear();
        foreach (var id in data.activeRuneIDs)
        {
            var s = allRunes.Find(r => r.id == id);
            if (s != null && collectedRunes.Contains(s)) activeRunes.Add(s);
        }
    }

    #endregion

    #region UI

    /// <summary>
    /// Rebuilds all UI slots & inventory.
    /// </summary>
    public void RefreshUI()
    {
        if(activeRunesPanel == null || inventoryPanel == null) return;

        // 1) Clear all panels

        foreach (Transform slot in activeRunesPanel)
        {
            // iterate backwards to safely destroy
            for (int i = slot.childCount - 1; i >= 0; i--)
            {
                Destroy(slot.GetChild(i).gameObject);
            }
        }


        foreach (Transform icon in inventoryPanel)
            Destroy(icon.gameObject);


        // 2) Populate active slots
        for (int i = 0; i < activeRunesPanel.childCount; i++)
        {
            var slot = activeRunesPanel.GetChild(i);
            if (i < activeRunes.Count && activeRunes[i] != null)
                CreateIcon(activeRunes[i], slot);
        }

        // 3) Populate inventory
        foreach (var rune in collectedRunes)
        {
            if(!activeRunes.Contains(rune))
                CreateIcon(rune, inventoryPanel);
        }

        UpdateComboDescription();
    }

    private void UpdateComboDescription()
    {
        if (comboDescriptionText == null) return;

        // collect non‐null descriptions
        var lines = new List<string>();
        foreach (var rune in activeRunes)
        {
            if (rune != null && !string.IsNullOrEmpty(rune.description))
                lines.Add(rune.description);
        }

        // join with newline
        comboDescriptionText.text = lines.Count > 0
            ? string.Join("\n", lines)
            : "";
    }

    /// <summary>
    /// Instantiates a RuneIcon under the given parent slot.
    /// </summary>
    private void CreateIcon(RuneSO rune, Transform parent)
    {
        var go = Instantiate(runeIconPrefab, parent);
        var icon = go.GetComponent<RuneIconUI>();
        icon.Setup(rune);
    }

    #endregion
}
