using UnityEngine;
using System.Collections.Generic;

public class GameStateManager : MonoBehaviour
{
    public static GameStateManager Instance;

    private Dictionary<string, bool> gameStates = new Dictionary<string, bool>();

    void Awake()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        // Мы убрали LoadState() отсюда
    }

    void Start()
    {
        // Теперь загрузка происходит, когда база данных точно создана
        LoadState();
    }

    public void SetState(string key, bool value)
    {
        gameStates[key] = value;
        string strValue = value ? "1" : "0";

        string query = $"INSERT OR REPLACE INTO game_state (key, value) VALUES ('{key}', '{strValue}');";
        DatabaseManager.Instance.ExecuteQuery(query);

        Debug.Log($"Состояние обновлено: {key} = {value}");
    }

    public bool GetState(string key)
    {
        if (gameStates.ContainsKey(key)) return gameStates[key];
        return false;
    }

    private void LoadState()
    {
        var reader = DatabaseManager.Instance.ExecuteReader("SELECT key, value FROM game_state;");

        while (reader.Read())
        {
            string key = reader.GetString(0);
            string value = reader.GetString(1);
            gameStates[key] = (value == "1");
        }

        reader.Dispose();
        Debug.Log("Состояния игры загружены из базы.");
    }
}