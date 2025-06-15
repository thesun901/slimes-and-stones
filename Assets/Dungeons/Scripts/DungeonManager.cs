using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public GameObject tilePrefab;
    public GameObject wallPrefab;
    public Transform roomParent;  // Optional: assign an empty GameObject in the scene as parent
    public float tileSize = 1f;   // Adjust if your prefab size is different
    public DungeonType dungeonType;

    public GameObject playerPrefab;
    private GameObject playerInstance;

    private Dictionary<Vector2Int, Room> dungeonRooms;
    private Room currentRoom;

    public static DungeonManager Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        playerInstance = Instantiate(playerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        playerInstance.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        DungeonGenerator generator = new DungeonGenerator();
        dungeonRooms = generator.GenerateDungeon(dungeonType);

        currentRoom = dungeonRooms[Vector2Int.zero];
        currentRoom.Load(tilePrefab, wallPrefab, roomParent, tileSize);

        SpawnPlayerInRoom(currentRoom);

        MinimapController.Instance.InitializeMinimap(new DungeonMap(dungeonRooms.Keys.ToList(), GenerateRoomDoors(dungeonRooms)), currentRoom.Position);
    }

    void Update()
    {
        
    }

    void SpawnPlayerInRoom(Room currentRoom)
    {
        if (currentRoom == null) return;

        // Calculate room world bounds
        // Assume currentRoom.Size is in units of (rooms), and OneByOneDimensions is (tiles)
        Vector2Int roomTiles = Room.BaseRoomDimensions * currentRoom.Size;
        float roomWidth = roomTiles.x * tileSize;
        float roomHeight = roomTiles.y * tileSize;

        Vector2 roomOrigin = currentRoom.RoomObject.transform.position;
        // Since room is centered, find bottom-left
        Vector2 bottomLeft = roomOrigin - new Vector2(roomWidth, roomHeight) / 2f + new Vector2(tileSize / 2f, tileSize / 2f);

        Rect roomRect = new Rect(bottomLeft, new Vector2(roomWidth, roomHeight));

        // Spawn player at the center of the room
        Vector3 spawnPos = currentRoom.RoomObject.transform.position;

        // Set bounds for the player
        var controller = playerInstance.GetComponent<PlayerController>();
        controller.roomBounds = roomRect;
    }

    public void MovePlayerToRoom(Room targetRoom, Direction entryDir)
    {
        if (targetRoom == null)
        {
            Debug.LogError("MovePlayerToRoom called with null targetRoom!");
            return;
        }
        if (playerInstance == null)
        {
            Debug.LogError("MovePlayerToRoom: playerInstance is null!");
            return;
        }

        if (currentRoom != null)
        {
            currentRoom.Unload();
        }

        if (targetRoom.RoomObject == null)
        {
            targetRoom.Load(tilePrefab, wallPrefab, roomParent, tileSize);
        }

        currentRoom = targetRoom;

        // Place player just inside the door in the new room
        Vector3 spawnPos = targetRoom.RoomObject.transform.position;
        Vector2Int roomTiles = Room.BaseRoomDimensions * targetRoom.Size;
        float roomWidth = roomTiles.x * tileSize;
        float roomHeight = roomTiles.y * tileSize;

        Vector3 offset = Vector3.zero;
        float edgeOffset = 1f * tileSize;

        switch (entryDir)
        {
            case Direction.Up:
                offset = new Vector3((roomTiles.x / 2f - 0.5f) * tileSize, -tileSize / 2f + 2 * tileSize, 0);
                break;
            case Direction.Down:
                offset = new Vector3((roomTiles.x / 2f - 0.5f) * tileSize, (roomTiles.y - 1) * tileSize - tileSize);
                break;
            case Direction.Left:
                offset = new Vector3((roomTiles.x - 1) * tileSize, (roomTiles.y / 2f - 0.5f) * tileSize, 0);
                break;
            case Direction.Right:
                offset = new Vector3(-tileSize / 2f + tileSize, (roomTiles.y / 2f - 0.5f) * tileSize, 0);
                break;
        }

        playerInstance.transform.position = spawnPos + offset;

        // Update player bounds
        var controller = playerInstance.GetComponent<PlayerController>();
        Vector2 roomOrigin = targetRoom.RoomObject.transform.position;
        Vector2 bottomLeft = roomOrigin - new Vector2(roomWidth, roomHeight) / 2f + new Vector2(tileSize / 2f, tileSize / 2f);
        Rect roomRect = new Rect(bottomLeft, new Vector2(roomWidth, roomHeight));
        controller.roomBounds = roomRect;

        MinimapController.Instance.UpdateCurrentRoom(currentRoom.Position);
    }

    public static Dictionary<Vector2Int, HashSet<Vector2Int>> GenerateRoomDoors(Dictionary<Vector2Int, Room> dungeonRooms)
    {
        var doors = new Dictionary<Vector2Int, HashSet<Vector2Int>>();

        foreach (var kvp in dungeonRooms)
        {
            Vector2Int roomPos = kvp.Key;
            Room room = kvp.Value;

            if (!doors.ContainsKey(roomPos))
                doors[roomPos] = new HashSet<Vector2Int>();

            // Ensure Room has a property or field for neighbors, e.g., List<Vector2Int> Neighbors
            foreach (var neighbor in room.Neighbors)
            {
                var neighborPos = roomPos + neighbor.Direction.AsVector();
                doors[roomPos].Add(neighborPos);

                // Also add the reverse connection for completeness (undirected)
                if (!doors.ContainsKey(neighborPos))
                    doors[neighborPos] = new HashSet<Vector2Int>();
                doors[neighborPos].Add(roomPos);
            }
        }

        return doors;
    }
}
