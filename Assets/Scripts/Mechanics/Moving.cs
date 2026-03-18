using System;
using System.Collections;
using System.Collections.Generic;
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
    private Queue<MovingSettings> movementQueue = new Queue<MovingSettings>();
    private bool isMoving = false;
    private int currentLevelInstanceID = -1;

    private void Start()
    {
        currentLevelInstanceID = GetHashCode();
        ResetState();
        ActionBus.OnStepCompleted += StartMoving;
    }

    private void OnDestroy()
    {
        ActionBus.OnStepCompleted -= StartMoving;
        ResetState();
    }

    private void ResetState()
    {
        movementQueue.Clear();
        isMoving = false;
        StopAllCoroutines();

        if (movingSteps != null)
        {
            stepIsPlayed = new bool[movingSteps.Length];
            for (int i = 0; i < movingSteps.Length; i++)
            {
                stepIsPlayed[i] = false;
            }
        }
    }

    private void StartMoving(int step)
    {
        if (movingSteps == null || stepIsPlayed == null) return;

        bool foundValidStep = false;

        for (int i = 0; i < movingSteps.Length; i++)
        {
            if (step == movingSteps[i].currentActionStep && !stepIsPlayed[i])
            {
                var settings = movingSteps[i];

                if (settings.target == null)
                {
                    continue;
                }

                stepIsPlayed[i] = true;
                movementQueue.Enqueue(settings);
                foundValidStep = true;
            }
        }

        if (foundValidStep && !isMoving)
        {
            ProcessNextMovement();
        }
    }

    public void ForceReset()
    {
        ResetState();
    }

    private void ProcessNextMovement()
    {
        if (movementQueue.Count > 0)
        {
            isMoving = true;
            MovingSettings settings = movementQueue.Dequeue();
            StartCoroutine(MoveAlongCurve(settings));
        }
        else
        {
            isMoving = false;
        }
    }

    private IEnumerator MoveAlongCurve(MovingSettings settings)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = settings.target.position;

        if (startPos == endPos)
        {
            ProcessNextMovement();
            yield break;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, settings.duration);

        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayMoving();
        }

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            float curveValue = moveCurve.Evaluate(t);
            transform.position = Vector3.Lerp(startPos, endPos, curveValue);
            yield return null;
        }

        transform.position = endPos;
        ProcessNextMovement();
    }
}