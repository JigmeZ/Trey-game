using UnityEngine;
using System.Collections;

public class Pop : MonoBehaviour
{
    [System.Serializable]
    public class PopElement
    {
        public GameObject element;   // The object to pop
        public bool enable = true;   // Toggle in Inspector
        public float popTime = 0f;   // Second to pop
        public AudioClip audioClip;  // Optional audio for this element
    }

    public PopElement[] elements;    
    public float scaleDuration = 0.3f;
    public float normalScaleMultiplier = 1f;

    [Header("Next Step")]
    public GameObject nextObject;        // The next scene/level container
    public GameObject currentSceneRoot;  // The current scene/level container to deactivate
    public float extraWaitAfterAudio = 1.5f;

    private AudioSource audioSource;

    private void Awake()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        enabled = false;
        if (currentSceneRoot != null) currentSceneRoot.SetActive(false);
        if (nextObject != null) nextObject.SetActive(false);
        return;
#endif
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    private void Start()
    {
        // Hide all at start
        foreach (var e in elements)
        {
            if (e.element != null)
            {
                e.element.SetActive(false);
                e.element.transform.localScale = Vector3.zero;
            }
        }

        StartCoroutine(PopSequence());
    }

    private IEnumerator PopSequence()
    {
        AudioClip lastClip = null;

        foreach (var e in elements)
        {
            if (!e.enable || e.element == null) 
                continue;

            yield return new WaitForSeconds(e.popTime);

            e.element.SetActive(true);
            StartCoroutine(ScaleIn(e.element));

            if (e.audioClip != null)
            {
                audioSource.PlayOneShot(e.audioClip);
                lastClip = e.audioClip;
            }
        }

        // Wait for last audio to finish
        if (lastClip != null)
            yield return new WaitForSeconds(lastClip.length);

        // Extra wait
        yield return new WaitForSeconds(extraWaitAfterAudio);

        // Activate next scene object
        if (nextObject != null)
            nextObject.SetActive(true);

        // Deactivate current scene root
        if (currentSceneRoot != null)
            currentSceneRoot.SetActive(false);
    }

    private IEnumerator ScaleIn(GameObject obj)
    {
        float t = 0f;
        Vector3 start = Vector3.zero;
        Vector3 target = Vector3.one * normalScaleMultiplier;

        while (t < scaleDuration)
        {
            t += Time.deltaTime;
            float k = Mathf.Clamp01(t / scaleDuration);
            obj.transform.localScale = Vector3.Lerp(start, target, k);
            yield return null;
        }

        obj.transform.localScale = target;
    }
}
