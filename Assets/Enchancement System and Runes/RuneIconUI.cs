using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static Unity.Collections.Unicode;

/// <summary>
/// Handles drag & drop of a rune icon in the UI.
/// </summary>
public class RuneIconUI : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public Image iconImage;
    private RuneSO runeData;
    private Transform originalParent;
    private Canvas rootCanvas;
    private CanvasGroup canvasGroup;

    /// <summary>
    /// Call this right after Instantiate to set up visuals.
    /// </summary>
    public void Setup(RuneSO rune)
    {
        runeData = rune;
        iconImage.sprite = rune.icon;
    }

    void Awake()
    {
        rootCanvas = GetComponentInParent<Canvas>();
        canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        originalParent = transform.parent;
        transform.SetParent(rootCanvas.transform);    // bring to top
        canvasGroup.blocksRaycasts = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.blocksRaycasts = true;
        RuneManager.Instance.RefreshUI();
        Destroy(gameObject);
    }

    /// <summary>
    /// Expose the RuneSO when dropping.
    /// </summary>
    public RuneSO GetRuneData() => runeData;
}
