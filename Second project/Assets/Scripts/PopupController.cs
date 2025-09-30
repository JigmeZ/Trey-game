using UnityEngine;
using System.Collections;

public class PopupController : MonoBehaviour
{
    [Header("Popup Settings")]
    public CanvasGroup popupGroup;         // Assign the CanvasGroup that contains your topic UI
    public float fadeDuration = 0.5f;      // Duration of the fade-in/out effect
    public AudioSource topicAudio;         // Assign the AudioSource with your topic audio

    [Header("Level Management")]
    public GameObject[] levelScenes;       // Assign your level GameObjects here
    private int currentLevel = 0;

    void Start()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        enabled = false;
        if (popupGroup != null) {
            popupGroup.alpha = 0f;
            popupGroup.blocksRaycasts = false;
            popupGroup.interactable = false;
        }
        return;
#endif

        // Start with popup hidden
        popupGroup.alpha = 0f;
        popupGroup.blocksRaycasts = false;
        popupGroup.interactable = false;

        // Show the first level
        HideAllLevels();
        ShowLevel(0);

        // Begin the intro sequence
        StartCoroutine(PlayTopicAndAdvance());
    }

    IEnumerator PlayTopicAndAdvance()
    {
        // Fade in the popup
        yield return StartCoroutine(FadeInPopup());

        // Play audio and wait until it finishes
        if (topicAudio != null && topicAudio.clip != null)
        {
            topicAudio.Play();
            yield return new WaitWhile(() => topicAudio.isPlaying);
        }

        // Fade out the popup
        yield return StartCoroutine(FadeOutPopup());

        // Move to the next level
        GoToNextLevel();
    }

    IEnumerator FadeInPopup()
    {
        float time = 0f;
        popupGroup.blocksRaycasts = true;
        popupGroup.interactable = true;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            popupGroup.alpha = Mathf.Lerp(0f, 1f, time / fadeDuration);
            yield return null;
        }

        popupGroup.alpha = 1f;
    }

    IEnumerator FadeOutPopup()
    {
        float time = 0f;
        popupGroup.blocksRaycasts = false;
        popupGroup.interactable = false;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            popupGroup.alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);
            yield return null;
        }

        popupGroup.alpha = 0f;
    }

    void GoToNextLevel()
    {
        int nextLevel = currentLevel + 1;
        if (nextLevel < levelScenes.Length)
        {
            ShowLevel(nextLevel);
        }
    }

    void ShowLevel(int index)
    {
        HideAllLevels();
        if (index >= 0 && index < levelScenes.Length)
        {
            levelScenes[index].SetActive(true);
            currentLevel = index;
        }
    }

    void HideAllLevels()
    {
        foreach (var level in levelScenes)
        {
            if (level != null) level.SetActive(false);
        }
    }
}
