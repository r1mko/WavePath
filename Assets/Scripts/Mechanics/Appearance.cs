using UnityEngine;
using System.Collections;

public class Appearance : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int currentActionStep;
    [SerializeField] private float actionDelay;
    [SerializeField] private AnimationCurve scaleCurve;

    private float animationDuration = 0.15f;
    private bool stepIsPlayed;
    private Vector3 startScale;
    private Vector3 targetScale;

    private void OnEnable()
    {
        startScale = Vector3.zero;
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

            StartCoroutine(ScaleUpAnimation(actionDelay));
        }
    }

    private IEnumerator ScaleUpAnimation(float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        float targetY = targetScale.y;

        while (elapsed < animationDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / animationDuration);

            float curveValue = scaleCurve.Evaluate(t);

            transform.localScale = new Vector3(targetScale.x, targetY * curveValue, targetScale.z);

            yield return null;
        }

        transform.localScale = targetScale;
    }
}