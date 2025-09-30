using UnityEngine;
using System.Collections;

[RequireComponent(typeof(RectTransform))]
[DisallowMultipleComponent]
public class DropSlot : MonoBehaviour
{
    [Header("Drag Items")]
    public GameObject correctItem;
    public GameObject wrongItem1;
    public GameObject wrongItem2; // optional (can be left empty)

    [Header("Audio")]
    public AudioClip rightAnswerAudio;
    public AudioClip wrongAnswerAudio;

    [Header("Next Level Object")]
    public GameObject nextLevelObject; // assign the next level/root object to show

    [Header("Snap")]
    public Vector2 snapOffset = Vector2.zero; // fine-tune placement relative to the slot

    private RectTransform slotRect;
    private AudioSource audioSource;
    private bool isFilled = false;

    void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        enabled = false;
        return;
#endif
        slotRect = GetComponent<RectTransform>();
        audioSource = GetComponent<AudioSource>();
        if (!audioSource) audioSource = gameObject.AddComponent<AudioSource>();
    }

    // Returns true if this slot handled the drop (so DragItem won't reset)
    public bool HandleDrop(GameObject dropped)
    {
        if (isFilled || dropped == null) return false;

        // Decide right/wrong
        bool isCorrect = (dropped == correctItem);
        bool isWrong = (dropped == wrongItem1) || (wrongItem2 != null && dropped == wrongItem2);

        var drag = dropped.GetComponent<DragItem>();
        var itemRect = dropped.GetComponent<RectTransform>();

        if (isCorrect)
        {
            // Snap to slot center (keep original parent)
            var itemParent = itemRect.parent as RectTransform;
            Vector2 localPoint;
            if (itemParent != null && RectTransformUtility.ScreenPointToLocalPointInRectangle(
                    itemParent, slotRect.position, null, out localPoint))
            {
                itemRect.localPosition = localPoint + snapOffset;
            }
            else
            {
                itemRect.position = slotRect.position + (Vector3)snapOffset;
            }
            // Lock the item so it can’t be dragged again
            if (drag) drag.LockInPlace();
            PlayOne(rightAnswerAudio);
            isFilled = true;
            StartCoroutine(GoNextAfter(rightAnswerAudio ? rightAnswerAudio.length : 0f));
            return true;
        }
        else if (isWrong)
        {
            PlayOne(wrongAnswerAudio);
            if (drag) drag.ResetPosition();
            return true;
        }

        // Item not recognized by this slot
        return false;
    }

    private void PlayOne(AudioClip clip)
    {
        if (!clip) return;
        audioSource.Stop();
        audioSource.PlayOneShot(clip);
    }

    private IEnumerator GoNextAfter(float delay)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);

        if (nextLevelObject) nextLevelObject.SetActive(true);

        // Hide current level root if that’s your flow
        var parentRoot = transform.parent ? transform.parent.gameObject : null;
        if (parentRoot) parentRoot.SetActive(false);
    }
}


