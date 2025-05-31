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

    private Room testRoom;

    void Start()
    {
        // Create a 1x1 room at grid position (0,0)
        testRoom = new Room(0, 0);
        testRoom.DungeonType = dungeonType;
        testRoom.Size = Vector2Int.one; // Explicit, but 1x1 by default

        // Generate and display the room
        testRoom.Load(tilePrefab, wallPrefab, roomParent, tileSize);

        SpawnPlayerInRoom(testRoom);
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
        playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        // Set bounds for the player
        var controller = playerInstance.GetComponent<PlayerController>();
        controller.roomBounds = roomRect;
    }
}
