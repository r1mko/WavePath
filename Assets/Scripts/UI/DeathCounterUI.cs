using System;
using UnityEngine;
using UnityEngine.UI;

public class DeathCounterUI : MonoBehaviour
{
    [Serializable]
    public struct DigitSprite
    {
        public int digit;
        public Sprite sprite;
    }

    [Header("Настройки отображения")]
    [SerializeField] private DigitSprite[] digitSprites;
    [SerializeField] private Image[] digitImages;

    private const string DEATH_KEY = "DEATH_COUNT";
    private const int MAX_DEATHS = 99999;

    private void Start()
    {
        UpdateDisplay();
    }

    public void UpdateDisplay()
    {
        if (digitImages == null || digitImages.Length != 5)
        {
            Debug.LogError("[DeathCounterUI] Ошибка: нужно ровно 5 Image для цифр!");
            return;
        }

        if (digitSprites == null || digitSprites.Length == 0)
        {
            Debug.LogError("[DeathCounterUI] Ошибка: массив спрайтов цифр пуст!");
            return;
        }

        int deaths = PlayerPrefs.GetInt(DEATH_KEY, 0);

        if (deaths > MAX_DEATHS)
        {
            deaths = MAX_DEATHS;
        }

        string deathString = deaths.ToString();
        int length = deathString.Length;

        for (int i = 0; i < 5; i++)
        {
            if (digitImages[i] == null) continue;

            if (i < length)
            {
                char charDigit = deathString[i];
                int digit = int.Parse(charDigit.ToString());

                Sprite targetSprite = GetSpriteForDigit(digit);

                if (targetSprite != null)
                {
                    digitImages[i].sprite = targetSprite;
                    digitImages[i].enabled = true;
                    digitImages[i].color = Color.white;
                }
                else
                {
                    Debug.LogError($"[DeathCounterUI] Не найден спрайт для цифры {digit}!");
                    digitImages[i].enabled = false;
                }
            }
            else
            {
                digitImages[i].enabled = false;
            }
        }
    }

    private Sprite GetSpriteForDigit(int digit)
    {
        for (int i = 0; i < digitSprites.Length; i++)
        {
            if (digitSprites[i].digit == digit)
            {
                if (digitSprites[i].sprite == null)
                {
                    Debug.LogWarning($"[DeathCounterUI] Спрайт для цифры {digit} равен null в массиве настроек!");
                }
                return digitSprites[i].sprite;
            }
        }
        Debug.LogWarning($"[DeathCounterUI] Цифра {digit} не найдена в массиве digitSprites!");
        return null;
    }

    [ContextMenu("Test: Add 100 Deaths")]
    public void TestAdd100()
    {
        int current = PlayerPrefs.GetInt(DEATH_KEY, 0);
        int newValue = current + 100;
        if (newValue > MAX_DEATHS) newValue = MAX_DEATHS;
        PlayerPrefs.SetInt(DEATH_KEY, newValue);
        PlayerPrefs.Save();
        UpdateDisplay();
        Debug.Log($"[Test] Добавлено 100. Всего: {newValue}");
    }

    [ContextMenu("Test: Add 1000 Deaths")]
    public void TestAdd1000()
    {
        int current = PlayerPrefs.GetInt(DEATH_KEY, 0);
        int newValue = current + 1000;
        if (newValue > MAX_DEATHS) newValue = MAX_DEATHS;
        PlayerPrefs.SetInt(DEATH_KEY, newValue);
        PlayerPrefs.Save();
        UpdateDisplay();
        Debug.Log($"[Test] Добавлено 1000. Всего: {newValue}");
    }

    [ContextMenu("Test: Add 10000 Deaths")]
    public void TestAdd10000()
    {
        int current = PlayerPrefs.GetInt(DEATH_KEY, 0);
        int newValue = current + 10000;
        if (newValue > MAX_DEATHS) newValue = MAX_DEATHS;
        PlayerPrefs.SetInt(DEATH_KEY, newValue);
        PlayerPrefs.Save();
        UpdateDisplay();
        Debug.Log($"[Test] Добавлено 10000. Всего: {newValue}");
    }

    [ContextMenu("Test: Reset to 0")]
    public void TestReset()
    {
        PlayerPrefs.SetInt(DEATH_KEY, 0);
        PlayerPrefs.Save();
        UpdateDisplay();
        Debug.Log("[Test] Сброшено до 0");
    }
}