using UnityEngine;
using System.Collections;

public class FinishZone : MonoBehaviour
{
    [SerializeField] private Animator animController;
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveDuration = 1f;
    [SerializeField] private float scaleDuration = 0.5f;
    [SerializeField] private Transform targetEndPos;

    private void Awake()
    {
        if (animController == null) animController = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(FinishSequence(collision.gameObject));
        }
    }

    private IEnumerator FinishSequence(GameObject player)
    {
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PauseMovement = true;
        }

        Vector3 startPos = player.transform.position;
        Vector3 endPos = targetEndPos.position;
        Vector3 startScale = player.transform.localScale;
        Vector3 endScale = Vector3.zero;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            float curveValue = moveCurve.Evaluate(t);

            player.transform.position = Vector3.Lerp(startPos, endPos, curveValue);

            yield return null;
        }

        player.transform.position = endPos;

        elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scaleDuration);
            float curveValue = moveCurve.Evaluate(t);

            player.transform.localScale = Vector3.Lerp(startScale, endScale, curveValue);

            yield return null;
        }

        player.transform.localScale = endScale;

        if (playerController != null)
        {
            playerController.HidePlayerView();
        }

        SoundManager.Instance.PlayFinish();
        animController.Play("FinishAnimation");

        // SceneManager.LoadScene("Menu");
    }
}