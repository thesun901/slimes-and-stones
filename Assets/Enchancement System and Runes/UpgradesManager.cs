using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class UpgradesManager : MonoBehaviour
{
    private List<string> activeUpgradeIDs = new List<string>();

    private string savePath;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, "runes.json");
        LoadActiveRunes();
        ApplyUpgrades();
    }


    void LoadActiveRunes()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"Upgrade save file not found at {savePath}");
            return;
        }

        string json = File.ReadAllText(savePath);
        var data = JsonUtility.FromJson<RuneSaveData>(json);

        if (data?.activeRuneIDs != null)
            activeUpgradeIDs = data.activeRuneIDs;
    }


    void ApplyUpgrades()
    {
        foreach (var id in activeUpgradeIDs)
        {
            ApplyUpgrade(id);
        }
    }


    void ApplyUpgrade(string id)
    {
        switch (id)
        {
            case "a":
                Debug.Log("Applying upgrade A");
                // TODO: increase player damage, e.g. player.Damage += X;
                break;

            case "b":
                Debug.Log("Applying upgrade B");
                // TODO: increase player speed
                break;

            case "c":
                Debug.Log("Applying upgrade C");
                // TODO: add health regen
                break;

            case "d":
                Debug.Log("Applying upgrade D");
                // TODO: enable double jump
                break;

            default:
                Debug.LogWarning($"Unknown upgrade ID: {id}");
                break;
        }
    }
}
