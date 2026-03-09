using UnityEngine;
using System.Collections;

public class FinishZone : MonoBehaviour
{
    [SerializeField] private Animator animController;

    private void Awake()
    {
        if (animController == null) animController = GetComponent<Animator>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(AnimatateAndLoadNextLevel(collision.gameObject));
        }
    }

    private IEnumerator AnimatateAndLoadNextLevel(GameObject player)
    {
        var playerController = player.GetComponent<PlayerController>();
        if (playerController != null)
        {
            playerController.PauseMovement = true;
            playerController.HidePlayerView();
        }

        yield return null;

        animController.Play("FinishAnimation");
        //SceneManager.LoadScene("Menu");
    }
}