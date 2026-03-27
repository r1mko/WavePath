using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Button restartButton;
    [SerializeField] private Button menuButton;

    private void Start()
    {
        restartButton.onClick.AddListener(RestartLevel);
        menuButton.onClick.AddListener(LoadMenu);
    }

    private void OnDestroy()
    {
        restartButton.onClick.RemoveAllListeners();
        menuButton.onClick.RemoveAllListeners();
    }

    private void RestartLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadMenu()
    {
        SceneManager.LoadScene(0);
    }
}
