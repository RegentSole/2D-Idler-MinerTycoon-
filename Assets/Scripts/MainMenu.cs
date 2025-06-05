using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [Header("UI Settings")]
    public Button playButton;
    public string gameSceneName = "GameScene";

    [Header("Transition Settings")]
    public float transitionDelay = 0.5f;
    public Animator fadeAnimator;

    void Start()
    {
        // Проверяем и настраиваем кнопку
        if (playButton != null)
        {
            playButton.onClick.AddListener(StartGame);
        }
        else
        {
            Debug.LogError("Play button is not assigned in MainMenu script!");
        }

        // Убедимся что имя сцены указано
        if (string.IsNullOrEmpty(gameSceneName))
        {
            Debug.LogError("Game scene name is not set in MainMenu script!");
        }
    }

    public void StartGame()
    {
        // Проигрываем анимацию затемнения если есть
        if (fadeAnimator != null)
        {
            fadeAnimator.SetTrigger("FadeOut");
        }

        // Запускаем загрузку сцены с задержкой
        Invoke("LoadGameScene", transitionDelay);

        // Отключаем кнопку для предотвращения повторных нажатий
        if (playButton != null)
        {
            playButton.interactable = false;
        }
    }

    private void LoadGameScene()
    {
        // Проверяем существует ли сцена
        if (Application.CanStreamedLevelBeLoaded(gameSceneName))
        {
            SceneManager.LoadScene(gameSceneName);
        }
        else
        {
            Debug.LogError($"Scene '{gameSceneName}' not found! Check build settings.");

            // Возвращаем кнопке активность
            if (playButton != null)
            {
                playButton.interactable = true;
            }
        }
    }
}