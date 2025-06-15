using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

// Represents the logical layout of your dungeon
public class DungeonMap
{
    public List<Vector2Int> RoomPositions = new List<Vector2Int>();
    public Dictionary<Vector2Int, HashSet<Vector2Int>> RoomDoors = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

    public DungeonMap(List<Vector2Int> roomPositions, Dictionary<Vector2Int, HashSet<Vector2Int>> roomDoors = null)
    {
        RoomPositions = roomPositions;
        RoomDoors = roomDoors ?? new Dictionary<Vector2Int, HashSet<Vector2Int>>();
    }
}

public class MinimapController : MonoBehaviour
{
    public RectTransform minimapPanel;
    public GameObject doorIconPrefab;
    public GameObject roomIconPrefab;
    public Color defaultRoomColor = Color.gray;
    public Color currentRoomColor = Color.yellow;
    public Color doorColor = Color.white;

    private Dictionary<Vector2Int, GameObject> minimapRooms = new Dictionary<Vector2Int, GameObject>();
    private List<GameObject> minimapDoors = new List<GameObject>();
    private DungeonMap dungeonMap;
    private Vector2Int currentRoom;

    public float roomScale = 0.6f;

    public static MinimapController Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }
    
    public void InitializeMinimap(DungeonMap map, Vector2Int startingRoom)
    {
        dungeonMap = map;
        currentRoom = startingRoom;
        DrawMinimap();
        HighlightCurrentRoom();
    }

    void OnRectTransformDimensionsChange()
    {
        if (dungeonMap != null)
        {
            DrawMinimap();
            HighlightCurrentRoom();
        }
    }

    void DrawMinimap()
    {
        // Clear old icons
        foreach (Transform child in minimapPanel)
            Destroy(child.gameObject);

        minimapRooms.Clear();
        minimapDoors.Clear();

        if (dungeonMap.RoomPositions == null || dungeonMap.RoomPositions.Count == 0)
            return;

        // Find dungeon bounds
        int minX = int.MaxValue, minY = int.MaxValue, maxX = int.MinValue, maxY = int.MinValue;
        foreach (var pos in dungeonMap.RoomPositions)
        {
            if (pos.x < minX) minX = pos.x;
            if (pos.y < minY) minY = pos.y;
            if (pos.x > maxX) maxX = pos.x;
            if (pos.y > maxY) maxY = pos.y;
        }
        int width = maxX - minX + 1;
        int height = maxY - minY + 1;

        Vector2 panelSize = minimapPanel.rect.size;
        float margin = 10f;
        float availableWidth = panelSize.x - margin * 2;
        float availableHeight = panelSize.y - margin * 2;
        float cellWidth = availableWidth / width;
        float cellHeight = availableHeight / height;
        float cellSize = Mathf.Min(cellWidth, cellHeight);

        // Centering offset so the grid is centered in the minimap panel
        float totalGridWidth = cellSize * width;
        float totalGridHeight = cellSize * height;
        float offsetX = (panelSize.x - totalGridWidth) / 2f;
        float offsetY = (panelSize.y - totalGridHeight) / 2f;

        // Draw rooms (make them smaller by roomScale)
        foreach (var roomPos in dungeonMap.RoomPositions)
        {
            var icon = Instantiate(roomIconPrefab, minimapPanel);
            icon.GetComponent<Image>().color = defaultRoomColor;

            var iconRect = icon.GetComponent<RectTransform>();
            iconRect.sizeDelta = new Vector2(cellSize * roomScale, cellSize * roomScale);

            // Set anchor and pivot to center
            iconRect.anchorMin = new Vector2(0, 0);
            iconRect.anchorMax = new Vector2(0, 0);
            iconRect.pivot = new Vector2(0.5f, 0.5f);

            float x = (roomPos.x - minX) * cellSize + cellSize / 2f + offsetX;
            float y = (roomPos.y - minY) * cellSize + cellSize / 2f + offsetY;
            iconRect.anchoredPosition = new Vector2(x, y);

            minimapRooms[roomPos] = icon;
        }

        // Draw doors between rooms
        if (dungeonMap.RoomDoors != null)
        {
            HashSet<(Vector2Int, Vector2Int)> drawnDoors = new HashSet<(Vector2Int, Vector2Int)>();

            foreach (var kvp in dungeonMap.RoomDoors)
            {
                Vector2Int room = kvp.Key;
                foreach (var neighbor in kvp.Value)
                {
                    // Draw each door only once
                    var ordered = room.x < neighbor.x || (room.x == neighbor.x && room.y < neighbor.y)
                        ? (room, neighbor) : (neighbor, room);
                    if (drawnDoors.Contains(ordered)) continue;
                    drawnDoors.Add(ordered);

                    // Position between rooms
                    float x1 = (room.x - minX) * cellSize + cellSize / 2f + offsetX;
                    float y1 = (room.y - minY) * cellSize + cellSize / 2f + offsetY;
                    float x2 = (neighbor.x - minX) * cellSize + cellSize / 2f + offsetX;
                    float y2 = (neighbor.y - minY) * cellSize + cellSize / 2f + offsetY;

                    Vector2 doorPos = (new Vector2(x1, y1) + new Vector2(x2, y2)) / 2f;

                    var doorIcon = Instantiate(doorIconPrefab, minimapPanel);
                    var doorRect = doorIcon.GetComponent<RectTransform>();

                    // Set anchor and pivot to center
                    doorRect.anchorMin = new Vector2(0, 0);
                    doorRect.anchorMax = new Vector2(0, 0);
                    doorRect.pivot = new Vector2(0.5f, 0.5f);

                    // Make door thinner and shorter than the cell
                    if (room.x != neighbor.x) // Horizontal door
                        doorRect.sizeDelta = new Vector2(cellSize * (1f - roomScale), cellSize * 0.18f);
                    else // Vertical door
                        doorRect.sizeDelta = new Vector2(cellSize * 0.18f, cellSize * (1f - roomScale));

                    doorRect.anchoredPosition = doorPos;

                    var doorImage = doorIcon.GetComponent<Image>();
                    if (doorImage != null)
                        doorImage.color = doorColor;

                    minimapDoors.Add(doorIcon);
                }
            }
        }
    }

    public void UpdateCurrentRoom(Vector2Int newRoom)
    {
        currentRoom = newRoom;
        HighlightCurrentRoom();
    }

    void HighlightCurrentRoom()
    {
        foreach (var kvp in minimapRooms)
        {
            kvp.Value.GetComponent<Image>().color = (kvp.Key == currentRoom) ? currentRoomColor : defaultRoomColor;
        }
    }
}