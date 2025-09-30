using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems; 
using System.Collections;

public class Hint : MonoBehaviour
{
    public GameObject popupImage; // Assign your popup image GameObject in the Inspector

    private Button imageButton;
    public float animationDuration = 0.75f; // Slower pop animation
    private Coroutine popupCoroutine;
    private Vector3 originalScale = Vector3.one;

    void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        // Disable this script if not on phone
        enabled = false;
        if (popupImage != null) popupImage.SetActive(false);
        return;
#endif
        imageButton = GetComponent<Button>();
    }

    void Start()
    {
        if (popupImage != null)
        {
            originalScale = popupImage.transform.localScale; // Store the intended scale
            popupImage.SetActive(false); // Hide popup at start
            popupImage.transform.localScale = originalScale;
        }

        if (imageButton != null)
            imageButton.onClick.AddListener(ShowHint);
    }

    void ShowHint()
    {
        if (popupImage != null)
        {
            if (popupCoroutine != null)
                StopCoroutine(popupCoroutine);

            popupImage.SetActive(true);
            popupImage.transform.localScale = Vector3.zero;
            popupCoroutine = StartCoroutine(PopAnimationIn());
        }
    }

    void Update()
    {
        if (popupImage != null && popupImage.activeSelf)
        {
            // Handle both mouse and touch for dismiss
#if UNITY_EDITOR || UNITY_STANDALONE
            if (Input.GetMouseButtonDown(0))
            {
                if (!IsPointerOverUIObject(popupImage) && !IsPointerOverUIObject(gameObject))
                {
                    HideHint();
                }
            }
#else
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                // For touch, check if touch is over UI
                PointerEventData eventData = new PointerEventData(EventSystem.current);
                eventData.position = Input.GetTouch(0).position;
                var results = new System.Collections.Generic.List<RaycastResult>();
                EventSystem.current.RaycastAll(eventData, results);
                bool overPopup = false;
                foreach (var r in results)
                {
                    if (r.gameObject == popupImage || r.gameObject.transform.IsChildOf(popupImage.transform) ||
                        r.gameObject == gameObject || r.gameObject.transform.IsChildOf(transform))
                    {
                        overPopup = true;
                        break;
                    }
                }
                if (!overPopup)
                {
                    HideHint();
                }
            }
#endif
        }
    }

    public void HideHint()
    {
        if (popupImage != null && popupImage.activeSelf)
        {
            if (popupCoroutine != null)
                StopCoroutine(popupCoroutine);

            popupCoroutine = StartCoroutine(PopAnimationOut());
        }
    }

    IEnumerator PopAnimationIn()
    {
        float t = 0;
        while (t < animationDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / animationDuration;
            // Springy pop effect
            float scale = Mathf.SmoothStep(0, 1.1f, normalized) - Mathf.Sin(normalized * Mathf.PI) * 0.1f;
            popupImage.transform.localScale = originalScale * scale;
            yield return null;
        }
        popupImage.transform.localScale = originalScale;
    }

    IEnumerator PopAnimationOut()
    {
        float t = 0;
        Vector3 startScale = popupImage.transform.localScale;
        while (t < animationDuration)
        {
            t += Time.unscaledDeltaTime;
            float normalized = t / animationDuration;
            float scale = Mathf.Lerp(startScale.x / originalScale.x, 0, normalized);
            popupImage.transform.localScale = originalScale * scale;
            yield return null;
        }
        popupImage.SetActive(false);
        popupImage.transform.localScale = originalScale;
    }

    // Helper to check if pointer is over a specific UI object
    bool IsPointerOverUIObject(GameObject target)
    {
        PointerEventData eventData = new PointerEventData(EventSystem.current);
        eventData.position = Input.mousePosition;
        var results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, results);
        foreach (var r in results)
        {
            if (r.gameObject == target || r.gameObject.transform.IsChildOf(target.transform))
                return true;
        }
        return false;
    }
}