using System;
using System.Collections;
using UnityEngine;

public class Scaling : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int currentActionStep;
    [SerializeField] private Vector3 targetScale;
    [SerializeField] private float animationDuration;
    [SerializeField] private AnimationCurve scaleCurve;
    private bool stepIsPlayed;


    private void Start()
    {
        ActionBus.OnStepCompleted += StartScaling;
    }

    private void OnDestroy()
    {
        ActionBus.OnStepCompleted -= StartScaling;
    }

    private void StartScaling(int step)
    {
        if (step == currentActionStep && !stepIsPlayed)
        {
            stepIsPlayed = true;
            StartCoroutine(ScaleAnimation(transform.localScale, targetScale));
        }
    }

    private IEnumerator ScaleAnimation(Vector3 from, Vector3 to)
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
