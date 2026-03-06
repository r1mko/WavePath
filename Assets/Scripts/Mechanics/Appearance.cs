using UnityEngine;
using System.Collections;

public class Appearance : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int currentActionStep;
    [SerializeField] private bool hideAtStart;
    [SerializeField] private AnimationCurve scaleCurve;

    private float animationDuration = 0.15f;
    private bool stepIsPlayed;
    private Vector3 startScale;
    private Vector3 targetScale;

    private void OnEnable()
    {
        startScale = hideAtStart ? Vector3.zero : Vector3.one;
        targetScale = Vector3.one;

        transform.localScale = startScale;
    }

    private void Start()
    {
        ActionBus.OnStepCompleted += StartAppearance;
    }

    private void OnDestroy()
    {
        ActionBus.OnStepCompleted -= StartAppearance;
    }

    private void StartAppearance(int step)
    {
        if (step == currentActionStep && !stepIsPlayed)
        {
            stepIsPlayed = true;
            gameObject.SetActive(true); 

            StartCoroutine(ScaleAnimation());
        }
    }

    private IEnumerator ScaleAnimation()
    {
        Vector3 currentStartScale = transform.localScale;
        if (hideAtStart)
        {
            yield return PlayScaleSequence(Vector3.zero, targetScale);
        }
        else
        {
            yield return PlayScaleSequence(currentStartScale, Vector3.zero);
            yield return PlayScaleSequence(Vector3.zero, targetScale);
        }

        transform.localScale = targetScale;
    }

    private IEnumerator PlayScaleSequence(Vector3 from, Vector3 to)
    {
        float elapsed = 0f;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);
            float curveValue = scaleCurve.Evaluate(t);
            Vector3 newScale = Vector3.Lerp(from, to, curveValue);
            transform.localScale = newScale;

            yield return null;
        }

        transform.localScale = to;
    }
}