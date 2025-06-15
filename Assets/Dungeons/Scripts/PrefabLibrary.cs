using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Dungeons/Prefab Library")]
public class PrefabLibrary : ScriptableObject
{
    public List<PrefabEntry> prefabs;

    private Dictionary<string, GameObject> prefabMap;

    public void Init()
    {
        prefabMap = new Dictionary<string, GameObject>();
        foreach (var entry in prefabs)
        {
            prefabMap[entry.name] = entry.prefab;
        }
    }

    public GameObject GetPrefab(string name)
    {
        if (prefabMap == null) Init();
        return prefabMap.TryGetValue(name, out var prefab) ? prefab : null;
    }
}

[System.Serializable]
public class PrefabEntry
{
    public string name;
    public GameObject prefab;
}