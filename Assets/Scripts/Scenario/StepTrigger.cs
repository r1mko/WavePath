using UnityEngine;

public class StepTrigger : MonoBehaviour
{
    public int CurrentStep;
    private bool wasUsed;

    private void Start()
    {
        wasUsed = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !wasUsed)
        {
            ActionBus.AdvanceTo(CurrentStep);
            wasUsed = true;
            Debug.Log($"[StepTrigger]Старт сценария с шагом: {CurrentStep}");
        }
    }
}
