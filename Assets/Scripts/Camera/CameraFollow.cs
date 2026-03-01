using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset & Dead Zone")]
    [SerializeField] private float xOffset;
    [SerializeField] private float deadZoneHalfWidth;

    void LateUpdate()
    {
        if (target == null) return;

        float targetX = target.position.x + xOffset;
        float currentX = transform.position.x;

        float delta = targetX - currentX;

        float desiredX = currentX;
        if (Mathf.Abs(delta) > deadZoneHalfWidth)
        {
            desiredX = targetX - Mathf.Sign(delta) * deadZoneHalfWidth;
        }

        transform.position = new Vector3(desiredX, transform.position.y, transform.position.z);
    }
}