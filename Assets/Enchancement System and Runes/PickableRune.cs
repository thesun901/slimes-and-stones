using UnityEngine;

[RequireComponent(typeof(Collider2D), typeof(SpriteRenderer))]
public class PickableRune : MonoBehaviour
{
    [Tooltip("The ScriptableObject that defines this rune")]
    public RuneSO rune;

    [Tooltip("Tag of the player object that can pick this up")]
    public string playerTag = "Player";

    private Collider2D col;

    void Reset()
    {
        // Ensure we have a trigger collider
        col = GetComponent<Collider2D>();
        col.isTrigger = true;

        // Automatically set a default sprite if not assigned
        var sr = GetComponent<SpriteRenderer>();
        if (rune != null && sr.sprite != rune.icon)
            sr.sprite = rune.icon;
    }

    void Awake()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only respond to the player
        if (!other.CompareTag(playerTag)) return;

        if (rune != null)
        {
            // Collect it via your RuneManager
            RuneManager.Instance.CollectRune(rune.id);
        }
        else
        {
            Debug.LogWarning($"PickableRune on '{gameObject.name}' has no RuneSO assigned.");
        }

        // Destroy the world object
        Destroy(gameObject);
    }
}
