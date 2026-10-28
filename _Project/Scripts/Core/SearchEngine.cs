using UnityEngine;
using System.Collections.Generic;

// Класс для хранения одного результата поиска (чтобы удобно передавать его в UI)
[System.Serializable]
public class SearchResult
{
    public string type;     // "Сообщение", "Файл", "История"
    public string title;    // Например, "Чат с Мариной" или "Логи управления"
    public string content;  // Сам текст улики
}

public class SearchEngine : MonoBehaviour
{
    public static SearchEngine Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Главный метод поиска
    public List<SearchResult> Search(string keyword)
    {
        List<SearchResult> results = new List<SearchResult>();

        if (string.IsNullOrEmpty(keyword)) return results;

        // Защита от поломки SQL-запроса, если игрок введет кавычку
        string safeKeyword = keyword.Replace("'", "''");

        // 1. Ищем в переписках (JOIN соединяет таблицу сообщений и таблицу контактов)
        string msgQuery = $@"
            SELECT c.contact_name, m.text 
            FROM messages m 
            JOIN chats c ON m.chat_id = c.id 
            WHERE m.text LIKE '%{safeKeyword}%';";

        using (var reader = DatabaseManager.Instance.ExecuteReader(msgQuery))
        {
            while (reader.Read())
            {
                results.Add(new SearchResult
                {
                    type = "Переписка",
                    title = $"Чат с {reader.GetString(0)}",
                    content = reader.GetString(1)
                });
            }
        }

        // 2. Ищем в файлах
        string fileQuery = $@"
            SELECT parent_folder, file_name, file_content 
            FROM files 
            WHERE file_name LIKE '%{safeKeyword}%' OR file_content LIKE '%{safeKeyword}%';";

        using (var reader = DatabaseManager.Instance.ExecuteReader(fileQuery))
        {
            while (reader.Read())
            {
                results.Add(new SearchResult
                {
                    type = "Файл",
                    title = $"{reader.GetString(1)} ({reader.GetString(0)})",
                    content = reader.GetString(2)
                });
            }
        }

        // 3. Ищем в истории браузера
        string historyQuery = $@"
            SELECT query, timestamp 
            FROM browser_history 
            WHERE query LIKE '%{safeKeyword}%';";

        using (var reader = DatabaseManager.Instance.ExecuteReader(historyQuery))
        {
            while (reader.Read())
            {
                results.Add(new SearchResult
                {
                    type = "История браузера",
                    title = reader.GetString(1), // Дата
                    content = reader.GetString(0) // Запрос
                });
            }
        }

        Debug.Log($"Поиск по слову '{keyword}' завершен. Найдено результатов: {results.Count}");
        return results;
    }
}
