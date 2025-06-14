using UnityEngine;

public class Door : MonoBehaviour
{
    public Room ConnectedRoom;
    public Direction Direction;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (ConnectedRoom == null)
        {
            Debug.LogWarning($"Door at {transform.parent?.name} with direction {Direction} has no connected room!");
            return;
        }
        var player = other.GetComponent<PlayerController>();
        if (player != null)
        {
            DungeonManager.Instance.MovePlayerToRoom(ConnectedRoom, Direction);
        }
    }
}