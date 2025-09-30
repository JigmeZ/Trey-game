using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector3 startPosition;
    private bool locked; // set true when dropped correctly

    void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        enabled = false;
        return;
#endif
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        if (!canvasGroup) canvasGroup = gameObject.AddComponent<CanvasGroup>();
    }

    void OnEnable()
    {
        startPosition = rectTransform.position;
        locked = false;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (locked) return;
        canvasGroup.blocksRaycasts = false; // let the slot receive the raycast
        rectTransform.SetAsLastSibling();   // draw on top while dragging
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (locked) return;
        Vector2 localPoint;
        var canvas = GetComponentInParent<Canvas>();
        if (canvas != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, eventData.position, canvas.worldCamera, out localPoint))
        {
            rectTransform.localPosition = localPoint;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (locked) { canvasGroup.blocksRaycasts = true; return; }

        // Raycast under pointer to find a DropSlotRaycast
        var results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);

        bool handled = false;
        foreach (var r in results)
        {
            var slot = r.gameObject.GetComponent<DropSlot>();
            if (slot != null)
            {
                // Let the slot decide correct/wrong + snap/reset/audio
                handled = slot.HandleDrop(gameObject);
                if (handled) break;
            }
        }

        if (!handled) ResetPosition(); // not over any slot

        canvasGroup.blocksRaycasts = true; // restore clicks
    }

    public void ResetPosition()
    {
        rectTransform.position = startPosition;
    }

    public void LockInPlace()
    {
        locked = true;
    }
}
