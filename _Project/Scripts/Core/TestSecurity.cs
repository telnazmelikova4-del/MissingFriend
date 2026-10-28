using UnityEngine;
using System.Collections.Generic;

public class TestSecurity : MonoBehaviour
{
    void Start()
    {
        Debug.Log("========== ТЕСТ 1: СИСТЕМА БЕЗОПАСНОСТИ ==========");

        // Получаем хеш правильного пароля
        string correctHash = SecurityManager.Instance.GetHash("stalker2026");

        // 1. Проверяем изначальное состояние папки
        bool isUnlocked = GameStateManager.Instance.GetState("stalkers_folder_unlocked");
        Debug.Log($"Папка Сталкеры открыта (до ввода)? {isUnlocked}");

        // 2. Симулируем неправильный ввод
        string playerInputWrong = "stalker123";
        if (SecurityManager.Instance.CheckPassword(playerInputWrong, correctHash))
        {
            Debug.LogError("ОШИБКА: Система пропустила неверный пароль!");
        }
        else
        {
            Debug.Log("УСПЕХ: Система отклонила неверный пароль.");
        }

        // 3. Симулируем правильный ввод
        string playerInputCorrect = "stalker2026";
        if (SecurityManager.Instance.CheckPassword(playerInputCorrect, correctHash))
        {
            Debug.Log("УСПЕХ: Система разрешила доступ!");
            // Сохраняем состояние, что папка открыта
            GameStateManager.Instance.SetState("stalkers_folder_unlocked", true);
        }
        else
        {
            Debug.LogError("ОШИБКА: Система отклонила правильный пароль!");
        }

        Debug.Log("========== ТЕСТ 2: ДВИЖОК ПОИСКА ==========");

        // Ищем слово "троян"
        List<SearchResult> troyanResults = SearchEngine.Instance.Search("троян");
        PrintResults("троян", troyanResults);

        // Ищем слово "Марина"
        List<SearchResult> marinaResults = SearchEngine.Instance.Search("Марина");
        PrintResults("Марина", marinaResults);

        // Проверим поиск слова, которого точно нет в базе
        List<SearchResult> emptyResults = SearchEngine.Instance.Search("крокодил");
        PrintResults("крокодил", emptyResults);

        Debug.Log("========== ВСЕ ТЕСТЫ ЗАВЕРШЕНЫ ==========");
    }

    // Вспомогательный метод для красивого вывода результатов в консоль
    private void PrintResults(string keyword, List<SearchResult> results)
    {
        Debug.Log($"<color=yellow>Результаты по слову '{keyword}':</color>");

        if (results.Count == 0)
        {
            Debug.Log("Ничего не найдено.");
            return;
        }

        foreach (var res in results)
        {
            Debug.Log($"[{res.type}] {res.title}: {res.content}");
        }
    }
}