using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    // Start is called before the first frame update

    private Image panelImage;
    void Start()
    {
        panelImage = GetComponent<Image>();
        FadeOutCoroutine();
    }

    public void FadeOutCoroutine()
    {
        StartCoroutine(FadeOut());
    }
    IEnumerator FadeOut()
    {

        float duration = 1f; // Duration of the fade
        float elapsedTime = 0f;
        Color color = panelImage.color;
        float startAlpha = color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            startAlpha = Mathf.Lerp(1, 0f, elapsedTime / duration);
            color.a = startAlpha;
            panelImage.color = color;
            yield return null;
        }

        startAlpha = 0f;
        color.a = startAlpha;
        panelImage.color = color;
    }


    public void FadeInPanel()
    {
        StartCoroutine(FadeInCoroutine());
    }

    IEnumerator FadeInCoroutine()
    {
        float duration = 1f;
        float elapsedTime = 0f;
        Color color = panelImage.color;
        float startAlpha = color.a;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            startAlpha = Mathf.Lerp(0, 1f, elapsedTime / duration);
            color.a = startAlpha;
            panelImage.color = color;
            yield return null;
        }

        startAlpha = 1f;
        color.a = startAlpha;
        panelImage.color = color;

        FadeOutCoroutine();
    }

    
}
