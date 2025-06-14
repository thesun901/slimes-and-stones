using UnityEngine;

[CreateAssetMenu(menuName = "Dungeons/DungeonTypes")]
public class DungeonType : ScriptableObject
{
    public string dungeonName;
    public Color floorColor = Color.white;
    public Sprite[] decorSprites;
    public Sprite[] backgroundSprites;

    public int levelWidth;
    public int levelHeight;

    public GameObject doorPrefab;
}
