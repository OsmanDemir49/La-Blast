using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro
using DG.Tweening;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance;

    [Header("Otomatik Bulunacak (TMP)")]
    public TextMeshProUGUI scoreText;         // Oyun İçi Puan
    public TextMeshProUGUI highScoreText;     // Ana Menü/Oyun İçi Rekor
    public TextMeshProUGUI finalScoreText;    // Game Over: Puan
    public TextMeshProUGUI finalHighScoreText;// Game Over: Rekor (YENİ EKLENDİ)

    [Header("Animasyon Ayarları")]
    [SerializeField] private float scoreAnimationDuration = 0.5f;
    [SerializeField] private float textPunchScale = 1.2f;

    private int score = 0;
    private int displayedScore = 0; // Animasyonlu gösterilecek skor
    private Tween scoreTween;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindTextObjects();
        ResetScore();

        // Sahne açılınca rekoru çekip her yere yazalım
        if (DatabaseManager.Instance != null)
        {
            int bestScore = DatabaseManager.Instance.GetHighScoreFromSQL();
            UpdateHighScoreUI(bestScore);
        }
    }

    void FindTextObjects()
    {
        // 1. Oyun İçi Puan
        GameObject sObj = GameObject.Find("ScoreText");
        if (sObj != null) scoreText = sObj.GetComponent<TextMeshProUGUI>();

        // 2. Oyun İçi/Menü Rekor
        GameObject hObj = GameObject.Find("HighScoreText");
        if (hObj != null) highScoreText = hObj.GetComponent<TextMeshProUGUI>();

        // 3. Game Over Panelindekiler
        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            // Puan Yazısı
            Transform fsTr = canvas.transform.Find("GameOverPanel/FinalScoreText");
            if (fsTr != null) finalScoreText = fsTr.GetComponent<TextMeshProUGUI>();

            // --- YENİ KISIM: Game Over Rekor Yazısı ---
            Transform fhsTr = canvas.transform.Find("GameOverPanel/FinalHighScoreText");
            if (fhsTr != null)
            {
                finalHighScoreText = fhsTr.GetComponent<TextMeshProUGUI>();
                Debug.Log("✅ Game Over Rekor Kutusu Bulundu!");
            }
        }
    }

    public void AddScore(int amount)
    {
        int oldScore = score;
        score += amount;
        
        // Animasyonlu skor güncelleme
        AnimateScoreChange(oldScore, score);
    }

    public void ResetScore()
    {
        score = 0;
        displayedScore = 0;
        UpdateScoreUI();
    }

    public int GetCurrentScore()
    {
        return score;
    }

    void AnimateScoreChange(int fromScore, int toScore)
    {
        // Önceki animasyonu durdur
        if (scoreTween != null && scoreTween.IsActive())
        {
            scoreTween.Kill();
        }

        // Skor text'ini büyüt-küçült animasyonu
        if (scoreText != null)
        {
            scoreText.transform.DOPunchScale(Vector3.one * (textPunchScale - 1f), 0.3f, 5, 0.5f);
        }

        // Sayı sayma animasyonu
        scoreTween = DOTween.To(() => displayedScore, x => {
            displayedScore = x;
            UpdateScoreUI();
        }, toScore, scoreAnimationDuration)
        .SetEase(Ease.OutQuad);
    }

    void UpdateScoreUI()
    {
        if (scoreText == null) FindTextObjects();
        if (scoreText != null)
        {
            scoreText.text = displayedScore.ToString();
        }
    }

    // --- REKOR GÜNCELLEME (HER YERİ GÜNCELLER) ---
    public void UpdateHighScoreUI(int bestScore)
    {
        // 1. Ana Menüdeki Rekoru Güncelle
        if (highScoreText == null) FindTextObjects();
        if (highScoreText != null)
        {
            highScoreText.text = "Rekor: " + bestScore.ToString();
            
            // Rekor güncellendiğinde animasyon
            highScoreText.transform.DOPunchScale(Vector3.one * 0.1f, 0.3f, 3, 0.3f);
        }

        // 2. Game Over Ekranındaki Rekoru da Güncelle!
        if (finalHighScoreText != null)
        {
            finalHighScoreText.text = "Yüksek Skor: " + bestScore.ToString();
        }
    }

    public void UpdateFinalScoreUI()
    {
        if (finalScoreText == null) FindTextObjects();
        if (finalScoreText != null)
        {
            // Final skor animasyonu
            finalScoreText.text = "Skorun: 0";
            finalScoreText.transform.localScale = Vector3.zero;
            
            // Fade in ve scale animasyonu
            finalScoreText.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
            
            // Sayı sayma animasyonu
            int finalScore = score;
            DOTween.To(() => 0, x => {
                finalScoreText.text = "Skorun: " + x.ToString();
            }, finalScore, 1f).SetEase(Ease.OutQuad);
        }
    }
}
