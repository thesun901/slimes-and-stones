using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeons/Room Layout")]
public class RoomLayout : ScriptableObject
{
    public Vector2Int size;
    public List<PlacedObject> obstacles;
    public List<PlacedObject> monsters;
}

[System.Serializable]
public class PlacedObject
{
    public string objectType;
    public Vector2 position;
}