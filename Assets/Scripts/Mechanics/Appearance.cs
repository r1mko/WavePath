using UnityEngine;
using System.Collections;

public class Appearance : MonoBehaviour
{
    public enum AppearanceType
    {
        Show,
        Hide
    }

    [Header("Settings")]
    [SerializeField] private int currentActionStep;
    [SerializeField] private AppearanceType appearanceType = AppearanceType.Show;

    private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    private float animationDuration = 0.15f;
    private bool stepIsPlayed;

    private readonly Vector3 visibleScale = Vector3.one;
    private readonly Vector3 hiddenScale = Vector3.zero;

    private void OnEnable()
    {
        if (appearanceType == AppearanceType.Show)
        {
            transform.localScale = hiddenScale;
        }
        else
        {
            transform.localScale = visibleScale;
        }

        stepIsPlayed = false;
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
        SoundManager.Instance.PlayAppearance();

        Vector3 from = transform.localScale;
        Vector3 to = (appearanceType == AppearanceType.Show) ? visibleScale : hiddenScale;

        yield return PlayScaleSequence(from, to);

        transform.localScale = to;

        if (appearanceType == AppearanceType.Hide)
        {
            gameObject.SetActive(false);
        }
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