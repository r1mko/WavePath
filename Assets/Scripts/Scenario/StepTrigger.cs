using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    public int CurrentStep;
    public int CollisionToUse;

    private bool wasUsed;
    private int collisionCount;

    private void Start()
    {
        wasUsed = false;
        collisionCount = 0;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !wasUsed)
        {
            collisionCount++;
            if (CollisionToUse <= collisionCount)
            {
                ActionBus.AdvanceTo(CurrentStep);
                wasUsed = true;
                Debug.Log($"[StepTrigger]Старт сценария с шагом: {CurrentStep}");
            }
        }
    }
}
