using UnityEngine;
using YG;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { get; private set; }

    [SerializeField] private float cooldownDuration = 61f;
    private float lastShowTime;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        lastShowTime = -1f;
    }

    public void TryShowAd()
    {
        if (!CheckCooldown())
        {
            Debug.Log("[AdManager] Реклама НЕ вызвана: кулдаун еще не истек.");
            return;
        }

        Debug.Log("[AdManager] Кулдаун истек (или первый запуск). Вызов рекламы...");
        ShowAdInternal();

        lastShowTime = Time.realtimeSinceStartup;
        Debug.Log("[AdManager] Таймер кулдауна запущен.");
    }

    private bool CheckCooldown()
    {
        if (lastShowTime < 0f)
        {
            Debug.Log("[AdManager] Первый запуск в сессии: кулдаун пропускается.");
            return true;
        }

        float currentTime = Time.realtimeSinceStartup;
        float timePassed = currentTime - lastShowTime;

        if (timePassed < cooldownDuration)
        {
            float timeLeft = cooldownDuration - timePassed;
            Debug.Log($"[AdManager] Проверка: прошло {timePassed:F1} сек, нужно {cooldownDuration} сек. Осталось ждать: {timeLeft:F1} сек.");
            return false;
        }

        Debug.Log($"[AdManager] Проверка: прошло {timePassed:F1} сек. Кулдаун завершен.");
        return true;
    }

    private void ShowAdInternal()
    {
        YG2.InterstitialAdvShow();
    }
}