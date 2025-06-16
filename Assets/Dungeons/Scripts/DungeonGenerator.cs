using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using static Unity.Collections.Unicode;

public class DungeonGenerator
{
    public int roomCount = 10;
    public int minBranchLength = 3;
    public int maxBranchLength = 6;

    private Dictionary<Vector2Int, Room> rooms = new Dictionary<Vector2Int, Room>();

    public Dictionary<Vector2Int, Room> GenerateDungeon(DungeonType dungeonType)
    {
        rooms.Clear();
        Vector2Int start = Vector2Int.zero;
        Room startRoom = new Room(start.x, start.y);
        startRoom.DungeonType = dungeonType;
        rooms[start] = startRoom;

        List<Vector2Int> endpoints = new List<Vector2Int> { start };

        // Main path (linear with some branches)
        Vector2Int currentPos = start;
        for (int i = 1; i < roomCount; i++)
        {
            Direction dir = (Direction)Random.Range(0, 4);
            Vector2Int offset = DirToVec(dir);
            Vector2Int nextPos = currentPos + offset;

            // Prevent overlaps
            if (rooms.ContainsKey(nextPos))
            {
                // Try to find a free neighbor
                bool placed = false;
                foreach (Direction tryDir in System.Enum.GetValues(typeof(Direction)))
                {
                    Vector2Int tryOffset = DirToVec(tryDir);
                    Vector2Int testPos = currentPos + tryOffset;
                    if (!rooms.ContainsKey(testPos))
                    {
                        dir = tryDir;
                        nextPos = testPos;
                        placed = true;
                        break;
                    }
                }
                if (!placed) continue; // skip, can't place more
            }

            Room newRoom = new Room(nextPos.x, nextPos.y);
            newRoom.DungeonType = dungeonType;
            rooms[nextPos] = newRoom;

            // Connect rooms
            Room prevRoom = rooms[currentPos];
            prevRoom.AddNeighbor(newRoom, dir);
            newRoom.AddNeighbor(prevRoom, OppositeDir(dir));

            currentPos = nextPos;
            endpoints.Add(currentPos);
        }

        // Optionally: add branches
        for (int b = 0; b < 2; b++)
        {
            Vector2Int branchStart = endpoints[Random.Range(0, endpoints.Count)];
            Vector2Int branchPos = branchStart;
            int branchLen = Random.Range(minBranchLength, maxBranchLength + 1);
            for (int i = 0; i < branchLen; i++)
            {
                Direction dir = (Direction)Random.Range(0, 4);
                Vector2Int offset = DirToVec(dir);
                Vector2Int nextPos = branchPos + offset;
                if (rooms.ContainsKey(nextPos)) continue;

                Room branchRoom = new Room(nextPos.x, nextPos.y);
                branchRoom.DungeonType = dungeonType;
                rooms[nextPos] = branchRoom;

                Room prevRoom = rooms[branchPos];
                prevRoom.AddNeighbor(branchRoom, dir);
                branchRoom.AddNeighbor(prevRoom, OppositeDir(dir));

                branchPos = nextPos;
                endpoints.Add(branchPos);
            }
        }

        // Mark end room (furthest from start)
        Vector2Int endRoomPos = start;
        int maxDist = 0;
        foreach (var key in rooms.Keys)
        {
            int dist = Mathf.Abs(key.x - start.x) + Mathf.Abs(key.y - start.y);
            if (dist > maxDist)
            {
                maxDist = dist;
                endRoomPos = key;
            }
        }
        rooms[endRoomPos].IsEndRoom = true;

        string path = $"Room Layouts/{dungeonType.dungeonName}";
        RoomLayout[] layouts = Resources.LoadAll<RoomLayout>(path);

        foreach (var room in rooms)
        {
            Vector2Int dim = new Vector2Int(Room.BaseRoomDimensions.x * room.Value.Size.x, Room.BaseRoomDimensions.y * room.Value.Size.y);
            float tileSize = 1f;
            Vector3 offset = new Vector3(-dim.x * tileSize / 2f + tileSize / 2f, -dim.y * tileSize / 2f + tileSize / 2f, 0);

            if (room.Value.Position == new Vector2Int(0, 0))
                continue;

            var possibleLayouts = layouts.ToList()
                .Where(layout => layout.size == room.Value.Size)
                .ToArray();

            if (room.Value.IsEndRoom)
            {
                var prefab = dungeonType.PrefabLibrary.GetPrefab("rune_1");
                if (prefab != null)
                {
                    var instance = GameObject.Instantiate(
                        prefab,
                        new Vector3(6 * tileSize, 3 * tileSize, -1f) + offset,
                        Quaternion.identity
                    );
                    instance.SetActive(false);
                    room.Value.EntityInstances.Add(instance);
                }
                continue;
            }

            if (possibleLayouts.Length > 0)
            {
                int layout = Random.Range(0, possibleLayouts.Length);
                var chosenLayout = possibleLayouts[layout];

                foreach (var obj in chosenLayout.obstacles)
                {
                    var prefab = dungeonType.PrefabLibrary.GetPrefab(obj.objectType);
                    if (prefab != null)
                    {
                        var instance = GameObject.Instantiate(
                            prefab,
                            new Vector3(obj.position.x * tileSize, obj.position.y * tileSize, -1f) + offset,
                            Quaternion.identity
                        );
                        instance.SetActive(false);
                        room.Value.EntityInstances.Add(instance);
                    }
                }

                foreach (var monster in chosenLayout.monsters)
                {
                    var prefab = dungeonType.PrefabLibrary.GetPrefab(monster.objectType);
                    if (prefab != null)
                    {
                        var instance = GameObject.Instantiate(
                            prefab,
                            new Vector3(monster.position.x * tileSize, monster.position.y * tileSize, -1f) + offset,
                            Quaternion.identity
                        );
                        instance.SetActive(false);
                        room.Value.EntityInstances.Add(instance);
                    }
                }
            }
        }

        return rooms;
    }

    private Vector2Int DirToVec(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Vector2Int.up;
            case Direction.Down: return Vector2Int.down;
            case Direction.Left: return Vector2Int.left;
            case Direction.Right: return Vector2Int.right;
        }
        return Vector2Int.zero;
    }

    private Direction OppositeDir(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
        }
        return dir;
    }
}
