using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UrlButton : MonoBehaviour
{
    [SerializeField] private string url = "https://yandex.ru/games/app/501996";
    [SerializeField] private float duration = 1.5f;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private RectTransform rectTransform;

    private void Start()
    {
        if (scaleCurve != null && scaleCurve.keys.Length > 0)
        {
            StartCoroutine(AnimateScale());
        }

        Button btn = GetComponent<Button>();
        if (btn != null)
        {
            btn.onClick.AddListener(OpenUrl);
        }
    }

    private IEnumerator AnimateScale()
    {
        while (true)
        {
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / duration);

                float currentScale = scaleCurve.Evaluate(t);

                if (rectTransform != null)
                {
                    rectTransform.localScale = new Vector3(currentScale, currentScale, 1f);
                }

                yield return null;
            }
        }
    }

    private void OpenUrl()
    {
        if (!string.IsNullOrEmpty(url))
        {
            Application.OpenURL(url);
        }
    }
}