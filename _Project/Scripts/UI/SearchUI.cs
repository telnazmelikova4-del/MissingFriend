using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class SearchUI : MonoBehaviour
{
    [Header("Ссылки на UI")]
    public TMP_InputField inputField;   // Сюда перетащим поле ввода
    public TextMeshProUGUI resultText;  // Сюда перетащим поле вывода

    // Этот метод мы привяжем к кнопке "Найти"
    public void OnSearchButtonClicked()
    {
        string keyword = inputField.text;

        if (string.IsNullOrEmpty(keyword)) return;

        // Очищаем старый текст
        resultText.text = "Идет поиск...\n";

        // Обращаемся к нашему движку поиска
        List<SearchResult> results = SearchEngine.Instance.Search(keyword);

        if (results.Count == 0)
        {
            resultText.text = $"По запросу '{keyword}' ничего не найдено.";
            return;
        }

        // Формируем красивый текст для вывода
        string finalText = $"<b>Найдено результатов: {results.Count}</b>\n\n";

        foreach (var res in results)
        {
            // <color=yellow> делает заголовок желтым, <i> делает текст курсивом
            finalText += $"<color=#FFD700>[{res.type}]</color> {res.title}\n";
            finalText += $"<i>{res.content}</i>\n\n";
        }

        resultText.text = finalText;
    }
}