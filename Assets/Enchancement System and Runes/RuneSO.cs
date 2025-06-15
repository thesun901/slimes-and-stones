using UnityEngine;

[CreateAssetMenu(fileName = "NewRune", menuName = "Runes/Rune")]
public class RuneSO : ScriptableObject
{
    [Tooltip("Unique identifier for this rune")]
    public string id;

    [Tooltip("Display name of the rune")]
    public string displayName;

    [TextArea, Tooltip("Description shown in UI")]
    public string description;

    [Tooltip("Icon sprite to show in the UI")]
    public Sprite icon;
}
