using UnityEngine;
using System.Security.Cryptography;
using System.Text;

public class SecurityManager : MonoBehaviour
{
    public static SecurityManager Instance;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Превращает пароль в хеш (например, "1234" -> "03ac674216f3e15...")
    public string GetHash(string password)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes) builder.Append(b.ToString("x2"));
            return builder.ToString();
        }
    }

    // Проверяет, совпадает ли введенный пароль с сохраненным хешем
    public bool CheckPassword(string inputPassword, string storedHash)
    {
        string inputHash = GetHash(inputPassword);
        return inputHash == storedHash;
    }
}
