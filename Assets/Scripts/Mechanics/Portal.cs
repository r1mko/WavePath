using System.Collections;
using UnityEngine;

public class Portal : MonoBehaviour
{
    [Header("References")]
    public Transform Destination;
    public Portal DestinationPortal;

    [Header("Safety")]
    [Tooltip("Distance to push player outward from the center of the destination portal")]
    public float exitBuffer = 0.35f;

    private bool canTeleport = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!canTeleport || !collision.CompareTag("Player")) return;
        TeleportPlayer(collision.gameObject);
    }

    private void TeleportPlayer(GameObject player)
    {
        ToggleTeleport(false);

        Vector3 exitDirection = Destination.right;
        Vector3 exitPoint = Destination.position + exitDirection * exitBuffer;

        if (!IsPositionSafe(exitPoint))
        {
            exitPoint = Destination.position - exitDirection * exitBuffer;
            if (!IsPositionSafe(exitPoint))
            {
                Debug.LogWarning("Portal exit blocked in both directions. Aborting teleport.");
                ToggleTeleport(true);
                return;
            }
        }

        player.transform.position = exitPoint;
        StartCoroutine(ReenableAfterDelay(0.1f));
    }

    private IEnumerator ReenableAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ToggleTeleport(true);
    }

    private void ToggleTeleport(bool state)
    {
        canTeleport = state;
        DestinationPortal.SetCanTeleport(state);
    }

    public void SetCanTeleport(bool state) => canTeleport = state;

    private bool IsPositionSafe(Vector3 position)
    {
        float radius = 0.25f;
        var portalLayer = LayerMask.NameToLayer("Portal");
        var playerLayer = LayerMask.NameToLayer("Player");
        var ignoreMask = (1 << portalLayer) | (1 << playerLayer);
        var obstacleMask = ~ignoreMask;

        Collider2D[] hits = Physics2D.OverlapCircleAll(position, radius, obstacleMask);
        foreach (var hit in hits)
        {
            if (!hit.isTrigger) return false;
        }
        return true;
    }
}
