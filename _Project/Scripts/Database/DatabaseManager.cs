using UnityEngine;
using CsSqlite;
using System.IO;

public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private string dbPath;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        dbPath = Path.Combine(Application.persistentDataPath, "GameDatabase.db");

        CreateTables();
        SeedData();

        Debug.Log($"База данных готова! Путь: {dbPath}");
    }

    private void CreateTables()
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();

            string createTablesSql = @"
                CREATE TABLE IF NOT EXISTS chats (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    contact_name TEXT NOT NULL,
                    platform TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS messages (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    chat_id INTEGER,
                    sender TEXT,
                    text TEXT,
                    is_deleted INTEGER DEFAULT 0,
                    is_read INTEGER DEFAULT 0,
                    FOREIGN KEY(chat_id) REFERENCES chats(id)
                );

                CREATE TABLE IF NOT EXISTS files (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    parent_folder TEXT,
                    file_name TEXT,
                    file_content TEXT,
                    is_hidden INTEGER DEFAULT 0
                );

                CREATE TABLE IF NOT EXISTS browser_history (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    query TEXT,
                    timestamp TEXT
                );

                CREATE TABLE IF NOT EXISTS game_state (
                    key TEXT PRIMARY KEY,
                    value TEXT
                );";

            connection.ExecuteNonQuery(createTablesSql);
        }
    }

    public void ExecuteQuery(string query)
    {
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            connection.ExecuteNonQuery(query);
        }
    }

    //
    public CsSqlite.SqliteReader ExecuteReader(string query)
    {
        var connection = new SqliteConnection(dbPath);
        connection.Open();
        return connection.ExecuteReader(query);
    }

    private void SeedData()
    {
        bool isEmpty = true;
        using (var connection = new SqliteConnection(dbPath))
        {
            connection.Open();
            using (var reader = connection.ExecuteReader("SELECT COUNT(*) FROM chats;"))
            {
                if (reader.Read())
                {
                    long count = reader.GetInt64(0);
                    if (count > 0) isEmpty = false;
                }
            }
        }

        if (!isEmpty) return;
        ExecuteQuery("INSERT INTO game_state (key, value) VALUES ('stalkers_folder_unlocked', '0');");
        Debug.Log("Наполняем базу сюжетом...");

        ExecuteQuery("INSERT INTO chats (contact_name, platform) VALUES ('Марина', 'Аська');");
        ExecuteQuery("INSERT INTO chats (contact_name, platform) VALUES ('Илья', 'Аська');");
        ExecuteQuery("INSERT INTO chats (contact_name, platform) VALUES ('Аноним', 'ВКонтакте');");
        ExecuteQuery("INSERT INTO chats (contact_name, platform) VALUES ('Леший_616', 'ВКонтакте');");

        ExecuteQuery("INSERT INTO messages (chat_id, sender, text) VALUES (1, 'Марина', 'ты опять сохранила всё в сталкерах?');");
        ExecuteQuery("INSERT INTO messages (chat_id, sender, text) VALUES (1, 'Алена', 'ага');");
        ExecuteQuery("INSERT INTO messages (chat_id, sender, text) VALUES (1, 'Марина', 'пароль тот же?');");
        ExecuteQuery("INSERT INTO messages (chat_id, sender, text) VALUES (1, 'Алена', 'да');");
        ExecuteQuery("INSERT INTO messages (chat_id, sender, text, is_deleted) VALUES (1, 'Алена', 'я собираю на него компромат в скрытой папке...', 1);");

        ExecuteQuery("INSERT INTO files (parent_folder, file_name, file_content) VALUES ('C:/Сталкеры', 'Логи управления ноутом.txt', 'API след от трояна. Время подключения: 21 октября, 03:00.');");
        ExecuteQuery("INSERT INTO files (parent_folder, file_name, file_content) VALUES ('C:/Сталкеры', 'Список мест от Анонима.txt', 'Координаты заброшек: Завод, Больница, Чернобыль-2.');");

        ExecuteQuery("INSERT INTO browser_history (query, timestamp) VALUES ('как сделать скрытую папку', '2026-10-18');");
        ExecuteQuery("INSERT INTO browser_history (query, timestamp) VALUES ('троян признаки заражения', '2026-10-20');");
        ExecuteQuery("INSERT INTO browser_history (query, timestamp) VALUES ('как посмотреть логи управления компом', '2026-10-21');");

        Debug.Log("База данных успешно наполнена!");
    }
}