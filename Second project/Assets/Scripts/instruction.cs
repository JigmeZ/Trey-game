using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement; // Add this for scene loading

public class DzongkhaAudioSync : MonoBehaviour
{
    [Header("Audio & Subtitles")]
    public AudioSource audioSource;
    public TextMeshProUGUI subtitleText;

    [Header("UI Elements")]
    public GameObject nextObject;

    [Header("Scene Management")]
    public string nextSceneName; // Set this in the Inspector to the next scene's name

    private string[] lines = {
        "ཕྲད་ ད་བ་ས་ལུ་ ཀྱིས་ཐོབ་ནི་དང་",
        "ཕྲད་ ག་ང་དང་མཐའ་མེད་ལུ་ གིས་ཐོབ་ཨིན།",
        "དེ་ལས་ ཕྲད་ ན་མ་ར་ལ་ལུ་ གྱིས་ཐོབ་ཨིན།"
    };
    private float[] timings = { 5.6f, 9.3f, 13.5f };

    private int state = 0; // 0: showing lines, 1: waiting, 2: show all
    private int currentLine = 0;
    private float waitStartTime;

    void Start()
    {
#if !UNITY_ANDROID && !UNITY_IOS
        enabled = false;
        if (nextObject != null) nextObject.SetActive(false);
        return;
#endif
        audioSource.Play();
        if (nextObject != null) nextObject.SetActive(false);
    }

    void Update()
    {
        if (state == 0)
        {
            if (currentLine < lines.Length && audioSource.time >= timings[currentLine])
            {
                subtitleText.text = lines[currentLine];
                currentLine++;
                if (currentLine == lines.Length)
                {
                    waitStartTime = Time.time;
                    state = 1;
                }
            }
        }
        else if (state == 1)
        {
            if (Time.time - waitStartTime >= 8f)
            {
                subtitleText.text = string.Join("\n", lines);
                state = 2;
            }
        }

        if (!audioSource.isPlaying && audioSource.time > 0)
        {
            // Load the next scene if specified, else fallback to old behavior
            if (!string.IsNullOrEmpty(nextSceneName))
            {
                SceneManager.LoadScene(nextSceneName);
            }
            else
            {
                if (nextObject != null) nextObject.SetActive(true);
                gameObject.SetActive(false);
            }
        }
    }
}
