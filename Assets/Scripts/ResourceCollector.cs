using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceCollector : MonoBehaviour
{
    public Button collectResource1Button;   // Первая кнопка для первого ресурса
    public Button collectResource2Button;   // Вторая кнопка для второго ресурса
    
    public TMP_Text resource1CountText;     // Текст для отображения первого ресурса
    public TMP_Text resource2CountText;     // Текст для отображения второго ресурса

    public int resource1PerClick = 1;       // Количество первого ресурса за один клик
    public int resource2PerClick = 1;       // Количество второго ресурса за один клик

    private int currentResource1 = 0;       // Текущий счетчик первого ресурса
    private int currentResource2 = 0;       // Текущий счетчик второго ресурса

    public int stoneCollectlevel = 1;
    public Button upgradeButton;

    void Start()
    {
        // Подписываемся на событие OnClick первой кнопки
        collectResource1Button.onClick.AddListener(OnCollectResource1ButtonClicked);
        
        // Подписываемся на событие OnClick второй кнопки
        collectResource2Button.onClick.AddListener(OnCollectResource2ButtonClicked);

        // Подписываемся на событие OnClick кнопки апгрейда
        upgradeButton.onClick.AddListener(OnButtonClick);
    }

    void OnCollectResource1ButtonClicked()
    {
        // Увеличиваем первый ресурс при нажатии на первую кнопку
           currentResource1 += resource1PerClick;
        resource1CountText.text = "Камня: " + currentResource1.ToString(); 

        // Проверяем уровень камня
        if (stoneCollectlevel == 2)
        {
            currentResource1 += resource1PerClick + 1;
            resource1CountText.text = "Камня: " + currentResource1.ToString();
        }

        else if (stoneCollectlevel == 3)
        {
            currentResource1 += resource1PerClick + 1;
            resource1CountText.text = "Камня: " + currentResource1.ToString();
        }
    }

    void OnCollectResource2ButtonClicked()
    {
        // Увеличиваем второй ресурс при нажатии на вторую кнопку
        currentResource2 += resource2PerClick;
        resource2CountText.text = "Угля: " + currentResource2.ToString();
    }

    public void OnButtonClick()
    {
        stoneCollectlevel++;
        upgradeButton.interactable = false;
    }
}