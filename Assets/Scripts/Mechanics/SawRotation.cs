using UnityEngine;

public class SawRotation : MonoBehaviour
{
    [SerializeField] private float speed = 100f;

    void Update()
    {
        transform.Rotate(0, 0, -speed * Time.deltaTime);
    }
}