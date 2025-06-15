using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2Int Position { get; private set; }
    public Vector2Int Size { get; set; } = Vector2Int.one;
    public bool IsEndRoom { get; set; } = false;
    public Room MasterRoom { get; set; } = null;
    public List<RoomNeighbor> Neighbors { get; private set; } = new List<RoomNeighbor>();
    public DungeonType DungeonType { get; set; }
    public static readonly Vector2Int BaseRoomDimensions = new Vector2Int(13, 7);
    public GameObject RoomObject { get; set; }
    public List<GameObject> EntityInstances = new List<GameObject>();

    public Room(int x, int y)
    {
        Position = new Vector2Int(x, y);
    }

    public void AddNeighbor(Room neighbor, Direction direction)
    {
        Neighbors.Add(new RoomNeighbor(neighbor, direction));
    }

    public Room GetMaster()
    {
        return MasterRoom == null ? this : MasterRoom.GetMaster();
    }

    public Vector3 GetSpawnPosition()
    {
        if (RoomObject != null)
            return RoomObject.transform.position;
        return new Vector3(Position.x, 0, Position.y);
    }

    public void Unload()
    {
        EntityInstances.RemoveAll(instance => instance == null);
        foreach (var instance in EntityInstances)
        {
            instance.SetActive(false);
        }

        if (RoomObject != null)
            GameObject.Destroy(RoomObject);
    }

    public void Load(GameObject tilePrefab, GameObject wallPrefab, Transform parent = null, float tileSize = 1f)
    {
        if (RoomObject != null)
            Unload();

        RoomObject = new GameObject($"Room_{Position.x}_{Position.y}");
        if (parent != null)
            RoomObject.transform.parent = parent;

        Vector2Int dim = new Vector2Int(
            BaseRoomDimensions.x * Size.x,
            BaseRoomDimensions.y * Size.y
        );

        int roomSeed = Position.x * 73856093 ^ Position.y * 19349663;
        System.Random roomRandom = new System.Random(roomSeed);

        float roomWidth = dim.x * tileSize;
        float roomHeight = dim.y * tileSize;

        RoomObject.transform.position = new Vector3(-roomWidth / 2f + tileSize / 2f, -roomHeight / 2f + tileSize / 2f, 0);

        for (int x = 0; x < dim.x; x++)
        {
            for (int y = 0; y < dim.y; y++)
            {
                GameObject tile = GameObject.Instantiate(tilePrefab, RoomObject.transform);
                tile.transform.localPosition = new Vector3(x * tileSize, y * tileSize, 0);

                var renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = DungeonType.floorColor;
                }
                else
                {
                    var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                        spriteRenderer.color = DungeonType.floorColor;
                }

                int tileSeed = roomSeed ^ (x * 83492791) ^ (y * 29348917);
                System.Random tileRandom = new System.Random(tileSeed);

                if (tileRandom.NextDouble() < 0.3 && DungeonType.decorSprites != null && DungeonType.decorSprites.Length > 0)
                {
                    int spriteIndex = tileRandom.Next(DungeonType.decorSprites.Length);
                    Sprite decorSprite = DungeonType.decorSprites[spriteIndex];

                    GameObject decorGO = new GameObject($"Decor_{x}_{y}");
                    decorGO.transform.parent = tile.transform;
                    decorGO.transform.localScale = new Vector3(0.5f, 0.5f, 1);
                    decorGO.transform.localPosition = Vector3.zero;

                    var sr = decorGO.AddComponent<SpriteRenderer>();
                    sr.sprite = decorSprite;

                    float decorOffset = 0.4f * tileSize;
                    float dx = ((float)tileRandom.NextDouble() - 0.5f) * decorOffset;
                    float dy = ((float)tileRandom.NextDouble() - 0.5f) * decorOffset;
                    decorGO.transform.localPosition += new Vector3(dx, dy, 0);

                    sr.sortingOrder = 1;
                }
            }
        }

        HashSet<Vector2Int> doorTilePositions = new HashSet<Vector2Int>();

        // Add doors to neighbors
        foreach (var neighbor in Neighbors)
        {
            Vector2Int roomDim = new Vector2Int(
                BaseRoomDimensions.x * Size.x,
                BaseRoomDimensions.y * Size.y
            );
            Vector3 doorLocalPos = Vector3.zero;
            Vector2Int doorTileCoord = Vector2Int.zero;
            switch (neighbor.Direction)
            {
                case Direction.Up:
                    doorTileCoord = new Vector2Int(roomDim.x / 2, roomDim.y);
                    doorLocalPos = new Vector3((roomDim.x / 2f - 0.5f) * tileSize, (roomDim.y) * tileSize, 0);
                    break;
                case Direction.Down:
                    doorTileCoord = new Vector2Int(roomDim.x / 2, -1);
                    doorLocalPos = new Vector3((roomDim.x / 2f - 0.5f) * tileSize, -tileSize, 0);
                    break;
                case Direction.Left:
                    doorTileCoord = new Vector2Int(-1, roomDim.y / 2);
                    doorLocalPos = new Vector3(-tileSize, (roomDim.y / 2f - 0.5f) * tileSize, 0);
                    break;
                case Direction.Right:
                    doorTileCoord = new Vector2Int(roomDim.x, roomDim.y / 2);
                    doorLocalPos = new Vector3((roomDim.x) * tileSize, (roomDim.y / 2f - 0.5f) * tileSize, 0);
                    break;
            }
            doorTilePositions.Add(doorTileCoord);

            GameObject doorObj = GameObject.Instantiate(DungeonType.doorPrefab, RoomObject.transform);
            doorObj.transform.localPosition = doorLocalPos;
            Door doorScript = doorObj.GetComponent<Door>();

            doorScript.ConnectedRoom = neighbor.Neighbor;
            doorScript.Direction = neighbor.Direction;

            var renderer = doorObj.GetComponent<Renderer>();
            if (renderer != null)
            {
                renderer.material.color = DungeonType.floorColor * 0.9f;
            }
        }

        int borderPadding = 1;
        for (int x = -borderPadding; x < dim.x + borderPadding; x++)
        {
            for (int y = -borderPadding; y < dim.y + borderPadding; y++)
            {
                if (x >= 0 && x < dim.x && y >= 0 && y < dim.y || doorTilePositions.Contains(new Vector2Int(x, y)))
                    continue;

                GameObject tile = GameObject.Instantiate(wallPrefab, RoomObject.transform);
                tile.transform.localPosition = new Vector3(x * tileSize, y * tileSize, 0);

                var renderer = tile.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.color = DungeonType.floorColor;
                }
                else
                {
                    var spriteRenderer = tile.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null)
                        spriteRenderer.color = DungeonType.floorColor;
                }

                int tileSeed = roomSeed ^ (x * 83492791) ^ (y * 29348917);
                System.Random tileRandom = new System.Random(tileSeed);

                if (DungeonType.backgroundSprites != null && DungeonType.backgroundSprites.Length > 0)
                {
                    int spriteIndex = tileRandom.Next(DungeonType.backgroundSprites.Length);
                    Sprite decorSprite = DungeonType.backgroundSprites[spriteIndex];

                    GameObject decorGO = new GameObject($"OutsideDecor_{x}_{y}");
                    decorGO.transform.parent = RoomObject.transform;
                    decorGO.transform.localPosition = new Vector3(x * tileSize, y * tileSize, 9);

                    var sr = decorGO.AddComponent<SpriteRenderer>();
                    sr.sprite = decorSprite;

                    float decorOffset = 0.2f * tileSize;
                    float dx = ((float)tileRandom.NextDouble() - 0.5f) * decorOffset;
                    float dy = ((float)tileRandom.NextDouble() - 0.5f) * decorOffset;
                    decorGO.transform.localPosition += new Vector3(dx, dy, 0);

                    decorGO.transform.localScale = new Vector2(0.5f, 0.5f);

                    float rotation = (float)tileRandom.NextDouble() * 360f;
                    decorGO.transform.localRotation = Quaternion.Euler(0, 0, rotation);

                    sr.sortingOrder = 9 + (int)(tileRandom.NextDouble() * 3);
                }
            }
        }

        foreach (var entity in EntityInstances)
        {
            if (entity != null)
                entity.SetActive(true);
        }
    }
}

public class RoomNeighbor
{
    public Room Neighbor { get; private set; }
    public Direction Direction { get; private set; }

    public RoomNeighbor(Room neighbor, Direction direction)
    {
        Neighbor = neighbor;
        Direction = direction;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}

public static class DirectionExtensions
{
    public static Vector2Int AsVector(this Direction direction)
    {
        switch (direction)
        {
            case Direction.Up: return new Vector2Int(0, 1);
            case Direction.Down: return new Vector2Int(0, -1);
            case Direction.Left: return new Vector2Int(-1, 0);
            case Direction.Right: return new Vector2Int(1, 0);
            default: return Vector2Int.zero;
        }
    }
}