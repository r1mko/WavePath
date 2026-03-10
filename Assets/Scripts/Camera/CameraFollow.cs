using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    public float xOffset;
    private float targetOffset;

    [Header("Settings")]
    [SerializeField] private float catchUpSpeed;
    [SerializeField] private float lagThreshold;
    [SerializeField] private float snapPrecision;

    [Header("Bounds")]
    [SerializeField] private bool useBounds;
    [SerializeField] private float minX;
    [SerializeField] private float maxX;

    private bool isCatchingUp = false;

    void Start()
    {
        targetOffset = xOffset;
        transform.position = new Vector3(target.position.x + xOffset, transform.position.y, transform.position.z);
    }

    void LateUpdate()
    {
        if (target == null) return;

        xOffset = Mathf.Lerp(xOffset, targetOffset, Time.deltaTime * 5f);

        float targetX = target.position.x + xOffset;

        if (useBounds)
        {
            targetX = Mathf.Clamp(targetX, minX, maxX);
        }

        float distance = Mathf.Abs(targetX - transform.position.x);

        if (distance > lagThreshold)
        {
            isCatchingUp = true;
            float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * catchUpSpeed);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        }
        else if (isCatchingUp)
        {
            if (distance > snapPrecision)
            {
                float newX = Mathf.Lerp(transform.position.x, targetX, Time.deltaTime * 15f);
                transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            }
            else
            {
                isCatchingUp = false;
                transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
            }
        }
        else
        {
            transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
        }
    }

    public void SetDirection(int direction)
    {
        targetOffset = Mathf.Abs(xOffset) * direction;
    }
}