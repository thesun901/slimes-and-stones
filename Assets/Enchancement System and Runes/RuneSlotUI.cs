using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Drop target for active/inventory slots.
/// slotIndex = -1 means “inventory” drop area (deactivate).
/// slotIndex 0..2 means activeRune slot.
/// </summary>
public class RuneSlotUI : MonoBehaviour, IDropHandler
{
    [Tooltip("-1 = inventory drop, 0..2 = active slot")]
    public int slotIndex = -1;

    public void OnDrop(PointerEventData eventData)
    {
        var icon = eventData.pointerDrag?.GetComponent<RuneIconUI>();
        if (icon == null) return;

        var rune = icon.GetRuneData();
        if (slotIndex >= 0)
        {
            RuneManager.Instance.SetActiveRune(rune, slotIndex);
        }
        else
        {
            RuneManager.Instance.DeactivateRune(rune);
        }

        
    }
}
