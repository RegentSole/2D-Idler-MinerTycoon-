using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WinScreen : MonoBehaviour
{
    [Header("Buttons")]
    public Button restartButton;
    public Button quitButton;

    void Start()
    {
        // Назначаем обработчики
        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(QuitGame);
        }
    }

    public void RestartGame()
    {
        // Восстанавливаем нормальное время
        Time.timeScale = 1f;

        // Перезагружаем текущую сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void QuitGame()
    {
        // Закрываем приложение (работает в билде)
        Application.Quit();

        // Для тестирования в редакторе
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}