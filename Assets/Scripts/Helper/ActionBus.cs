using System;

public static class ActionBus
{
    public static Action<int> OnStepCompleted;
    public static void AdvanceTo(int step) => OnStepCompleted?.Invoke(step);
}
