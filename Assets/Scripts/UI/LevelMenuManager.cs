using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelMenuManager : MonoBehaviour
{
    [Serializable]
    public struct LevelButtonData
    {
        public int levelIndex;
        public Button button;
    }

    [Header("Настройки уровней")]
    [SerializeField] private LevelButtonData[] levelButtons;

    [SerializeField] private Button[] levelContainers;

    private void Start()
    {
        WireEvents();
    }

    private void WireEvents()
    {
        if (levelButtons == null) return;

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (levelButtons[i].button == null) continue;

            Button btn = levelButtons[i].button;
            int sceneIndex = levelButtons[i].levelIndex;

            btn.onClick.RemoveAllListeners();
            btn.onClick.AddListener(() => LoadLevel(sceneIndex));
        }
    }

    private void LoadLevel(int index)
    {
        if (index >= 0 && index < SceneManager.sceneCountInBuildSettings)
        {
            SceneManager.LoadScene(index);
        }
        else
        {
            Debug.LogError($"Ошибка: Сцена с индексом {index} не найдена в Build Settings!");
        }
    }

    [ContextMenu("Initialize Data from Containers")]
    public void MapReferences()
    {
        if (levelContainers == null || levelContainers.Length == 0)
        {
            Debug.LogError("Массив levelContainers пуст!");
            return;
        }

        levelButtons = new LevelButtonData[levelContainers.Length];

        for (int i = 0; i < levelContainers.Length; i++)
        {
            Button btn = levelContainers[i];
            if (btn == null) continue;

            levelButtons[i].levelIndex = i + 1;
            levelButtons[i].button = btn;
        }

        Debug.Log($"Данные инициализированы. Кнопок: {levelButtons.Length}");
    }
}