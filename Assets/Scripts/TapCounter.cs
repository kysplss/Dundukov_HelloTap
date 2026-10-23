using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Счётчик нажатий: по кнопке увеличивает число и показывает его на экране.
/// Вешается на Canvas; кнопка вызывает Increment() через OnClick.
/// </summary>
public class TapCounter : MonoBehaviour
{
    // Текст, в который выводится счёт (назначается в Inspector)
    [SerializeField] private Text scoreText;

    // Текущее значение счётчика
    private int count = 0;

    // Start вызывается один раз перед первым кадром
    private void Start()
    {
        UpdateUI();
    }

    /// <summary>Увеличить счёт на 1 (вызывается кнопкой).</summary>
    public void Increment()
    {
        count++;
        UpdateUI();
        Debug.Log("Tap! count = " + count); // вывод смотрим в окне Console
    }

    /// <summary>Обнулить счёт.</summary>
    public void Reset()
    {
        count = 0;
        UpdateUI();
    }

    // Обновить текст на экране
    private void UpdateUI()
    {
        scoreText.text = "Счёт: " + count;
    }
}
