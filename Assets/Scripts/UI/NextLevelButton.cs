using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using YG;

public class NextLevelButton : MonoBehaviour
{
    [SerializeField] private Button button;
    [SerializeField] private AnimationCurve appearCurve;
    [SerializeField] private float appearDuration = 0.5f;

    private void Awake()
    {
        if (button == null) button = GetComponent<Button>();
        gameObject.SetActive(false);
    }

    public void SetupListener()
    {
        gameObject.SetActive(true);

        if (button == null) button = GetComponent<Button>();

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(LoadNextScene);
        StartCoroutine(AnimateAppear());
    }

    private IEnumerator AnimateAppear()
    {
        gameObject.SetActive(true);

        float elapsed = 0f;

        while (true)
        {
            elapsed += Time.deltaTime;
            float t = (elapsed / appearDuration) % 1f;
            float curveValue = appearCurve.Evaluate(t);
            transform.localScale = Vector3.one * (1f + curveValue * 0.2f);

            yield return null;
        }
    }

    private void LoadNextScene()
    {
        YG2.InterstitialAdvShow();

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        int maxIndex = SceneManager.sceneCountInBuildSettings - 1;

        if (currentIndex >= maxIndex)
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            SceneManager.LoadScene(currentIndex + 1);
        }
    }
}