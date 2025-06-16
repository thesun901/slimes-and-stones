using UnityEngine;
using System.Collections.Generic;
using System.IO;


public class UpgradesManager : MonoBehaviour
{
    private List<string> activeUpgradeIDs = new List<string>();
    private PlayerController controller;
    private BoomerangBehavior boomerangBehavior;
    private string savePath;

    void Start()
    {
        savePath = Path.Combine(Application.persistentDataPath, "runes.json");
        LoadActiveRunes();


    }

    private void LateUpdate()
    {
        if (controller == null)
        {
            controller = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
            boomerangBehavior = GameObject.Find("BoomerangBullet(Clone)").GetComponent<BoomerangBehavior>();
            ApplyUpgrades();
        }
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
                controller.hp += 100;
                break;

            case "b":
                boomerangBehavior.forwardSpeed += 10;
                boomerangBehavior.forwardDuration /= 2;
                boomerangBehavior.backSpeed = 7;
                break;

            case "c":
                boomerangBehavior.damageBack += 15;
                boomerangBehavior.damageForward -= 5;
                break;

            case "d":
                controller.moveSpeed += 5;
                break;

            default:
                Debug.LogWarning($"Unknown upgrade ID: {id}");
                break;
        }
    }
}
