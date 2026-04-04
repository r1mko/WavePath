using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;
    [SerializeField] private NextLevelButton nextLevelButton;

    private void Awake()
    {
        Instance = this;

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (menuButton != null)
            menuButton.onClick.AddListener(LoadMenu);
    }

    private void OnDestroy()
    {
        if (restartButton != null)
            restartButton.onClick.RemoveAllListeners();

        if (menuButton != null)
            menuButton.onClick.RemoveAllListeners();

        if (Instance == this)
            Instance = null;
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }

    public NextLevelButton GetNextLevelButton()
    {
        return nextLevelButton;
    }
}