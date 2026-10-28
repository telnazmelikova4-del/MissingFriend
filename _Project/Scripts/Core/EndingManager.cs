using UnityEngine;
using UnityEngine.SceneManagement; // Нужно для перезагрузки игры

public class EndingManager : MonoBehaviour
{
    public static EndingManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Метод вызывается, когда игрок нажимает кнопку "Отправить заявление"
    // suspectName - это имя того, на кого указал игрок ("Илья", "Марина", "Леший_616")
    public void AccuseSuspect(string suspectName)
    {
        Debug.Log($"<color=orange>Игрок обвиняет: {suspectName}</color>");

        // Проверяем, нашел ли игрок главную улику (троян в логах)
        // В нашем тестовом скрипте мы искали файл "Логи управления ноутом.txt"
        // Открывал ли игрок папку Сталкеры (флаг из GameStateManager)
        bool hasTroyanEvidence = GameStateManager.Instance.GetState("stalkers_folder_unlocked");

        // Дополнительно проверим, искал ли игрок в браузере слово "троян"
        bool searchedForTroyan = false;
        using (var reader = DatabaseManager.Instance.ExecuteReader("SELECT * FROM browser_history WHERE query LIKE '%троян%';"))
        {
            if (reader.Read()) searchedForTroyan = true; // Нашли запрос
        }

        // --- ЛОГИКА КОНЦОВОК ---добавить концовку с анонимом, сделать так чтобы при каждой отдельной концовке было свое завершение, загатовка для вставки будущих промо-роликов, также сделать привязку к правильно подобранным доказательсвтвам + выбранное имя, если нет этих двух состоявляющих, то выведи ошибку(выплывающую плашку) о том что "что- то не так, надо подумать еще"

        if (suspectName == "Илья")
        {
            if (hasTroyanEvidence && searchedForTroyan)
            {
                // ИСТИННАЯ КОНЦОВКА
                Debug.Log("ИСТИННАЯ КОНЦОВКА: Илья — создатель трояна. Запуск промо-ролика...");
                // В реальной игре тут был бы запуск видео (VideoPlayer) или загрузка сцены с титрами
                // SceneManager.LoadScene("TrueEndingScene"); 
            }
            else
            {
                Debug.Log("ОШИБКА: Вы обвинили Илью, но не нашли достаточно улик (логов трояна).");
                GameOver();
            }
        }
        else if (suspectName == "Марина")
        {
            // ЛОЖНАЯ КОНЦОВКА (Марина)
            Debug.Log("ЛОЖНАЯ КОНЦОВКА: Марина невиновна. Игра окончена.");
            GameOver();
        }
        else if (suspectName == "Леший_616")
        {
            // ЛОЖНАЯ КОНЦОВКА (Леший)
            Debug.Log("ЛОЖНАЯ КОНЦОВКА: Леший просто хейтер. Игра окончена.");
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("<color=red>GAME OVER. Ноутбук выключается...</color>");
        // В реальной игре здесь был бы эффект выключения экрана или появление клоуна
        // Invoke("RestartGame", 3f); 
    }

    private void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}