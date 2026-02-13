using UnityEngine;

/// PlayerPrefs 
public class DatabaseManager : MonoBehaviour
{
    public static DatabaseManager Instance;

    private const string HIGH_SCORE_KEY = "HighScore";

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        TestConnection();
    }

    public void TestConnection()
    {
        Debug.Log("✅ DatabaseManager Hazır! (PlayerPrefs kullanılıyor)");
        int currentHighScore = GetHighScoreFromSQL();
        Debug.Log($"📊 Mevcut En Yüksek Skor: {currentHighScore}");
    }

    public void SaveScoreToSQL(int score)
    {
        int currentHighScore = GetHighScoreFromSQL();
        
        if (score > currentHighScore)
        {
            PlayerPrefs.SetInt(HIGH_SCORE_KEY, score);
            PlayerPrefs.Save();
            Debug.Log($"✅ YENİ REKOR KAYDEDİLDİ: {score}");
        }
        else
        {
            Debug.Log($"Skor kaydedilmedi. Mevcut rekor: {currentHighScore}, Yeni skor: {score}");
        }
    }

    public int GetHighScoreFromSQL()
    {
        int highScore = PlayerPrefs.GetInt(HIGH_SCORE_KEY, 0);
        return highScore;
    }

    public void ResetAllScores()
    {
        PlayerPrefs.DeleteKey(HIGH_SCORE_KEY);
        PlayerPrefs.Save();
        Debug.Log("⚠️ Tüm skorlar sıfırlandı!");
    }
}