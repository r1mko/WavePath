using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset & Dead Zone")]
    public float xOffset;
    public float deadZoneHalfWidth;

    private float targetOffset;
    private float smoothSpeed = 1f;

    void Start()
    {
        targetOffset = xOffset;
    }

    void LateUpdate()
    {
        if (target == null) return;

        xOffset = Mathf.Lerp(xOffset, targetOffset, Time.deltaTime * smoothSpeed);

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

    public void SetDirection(int direction)
    {
        targetOffset = Mathf.Abs(xOffset) * direction;

        Debug.Log($"[Camera] Смена направления. Цель оффсета: {targetOffset}");
    }
}