using System;
using System.Collections;
using UnityEngine;

public class Moving : MonoBehaviour
{
    [Serializable]
    public struct MovingSettings
    {
        public Transform target;
        public int currentActionStep;
        public float duration;
    }

    [SerializeField] private MovingSettings[] movingSteps;
    private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private bool[] stepIsPlayed;

    private void Start()
    {
        if (movingSteps != null)
        {
            stepIsPlayed = new bool[movingSteps.Length];
            for (int i = 0; i < movingSteps.Length; i++)
            {
                stepIsPlayed[i] = false;
            }
        }

        ActionBus.OnStepCompleted += StartMoving;
    }

    private void OnDestroy()
    {
        ActionBus.OnStepCompleted -= StartMoving;
    }

    private void StartMoving(int step)
    {
        if (movingSteps == null) return;

        for (int i = 0; i < movingSteps.Length; i++)
        {
            if (step == movingSteps[i].currentActionStep && !stepIsPlayed[i])
            {
                stepIsPlayed[i] = true;

                var settings = movingSteps[i];

                if (settings.target == null)
                {
                    Debug.LogWarning($"[Moving] Target is null for step {step} on {gameObject.name}");
                    continue;
                }

                StartCoroutine(MoveAlongCurve(settings));
            }
        }
    }

    private IEnumerator MoveAlongCurve(MovingSettings settings)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = settings.target.position;

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, settings.duration);

        SoundManager.Instance.PlayMoving();

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(elapsed / duration);

            float curveValue = moveCurve.Evaluate(t);

            transform.position = Vector3.Lerp(startPos, endPos, curveValue);

            yield return null;
        }

        transform.position = endPos;
    }
}