using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using TMPro;
using System.Collections.Generic; 

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("Paneller & Oyun Objeleri")]
    public GameObject startMenuPanel;
    public GameObject gameOverPanel;

    public List<GameObject> gamePlayElements;

    private Button playButton;
    private Button restartButton;

    [Header("Animasyon Ayarları")]
    [SerializeField] private float panelFadeDuration = 0.5f;


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

        FindAndSetupUI();

        SetGameElementsActive(false);

        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        CheckRestartStatus();
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
        Time.timeScale = 1f; 
        FindAndSetupUI();    
        CheckRestartStatus();
    }

    
    void SetGameElementsActive(bool isActive)
    {
        if (gamePlayElements != null)
        {
            foreach (var obj in gamePlayElements)
            {
                if (obj != null) obj.SetActive(isActive);
            }
        }
    }

    void FindAndSetupUI()
    {
        GameObject canvas = GameObject.Find("Canvas");

        if (canvas != null)
        {
            Transform startTr = canvas.transform.Find("MainMenuPanel");
            Transform gameoverTr = canvas.transform.Find("GameOverPanel");

    
            if (gamePlayElements == null || gamePlayElements.Count == 0)
            {
                gamePlayElements = new List<GameObject>();

    
                Transform uiGamePanel = canvas.transform.Find("GamePanel");
                if (uiGamePanel != null) gamePlayElements.Add(uiGamePanel.gameObject);

    
                GameObject gridMgr = GameObject.Find("GridManager");
                if (gridMgr != null) gamePlayElements.Add(gridMgr);

    
                GameObject spawner = GameObject.Find("ShapeSpawner");
                if (spawner != null) gamePlayElements.Add(spawner);
            }

            if (startTr != null) startMenuPanel = startTr.gameObject;
            if (gameoverTr != null) gameOverPanel = gameoverTr.gameObject;
        }
        else
        {
            Debug.LogError("HATA: Sahnede 'Canvas' isminde bir obje bulunamadı!");
        }

    
        if (startMenuPanel != null)
        {
            playButton = startMenuPanel.GetComponentInChildren<Button>();
            if (playButton != null)
            {
                playButton.onClick.RemoveAllListeners();
                playButton.onClick.AddListener(StartGame);
                
    
                if (playButton.GetComponent<ButtonAnimator>() == null)
                {
                    playButton.gameObject.AddComponent<ButtonAnimator>();
                }
            }
        }

        if (gameOverPanel != null)
        {
            restartButton = gameOverPanel.GetComponentInChildren<Button>();
            if (restartButton != null)
            {
                restartButton.onClick.RemoveAllListeners();
                restartButton.onClick.AddListener(RestartGame);
                
    
                if (restartButton.GetComponent<ButtonAnimator>() == null)
                {
                    restartButton.gameObject.AddComponent<ButtonAnimator>();
                }
            }
        }
    }



    void CheckRestartStatus()
    {
        if (PlayerPrefs.GetInt("RestartDurumu", 0) == 1)
        {
           
            if (startMenuPanel != null) startMenuPanel.SetActive(false);
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

           
            SetGameElementsActive(true);

            PlayerPrefs.SetInt("RestartDurumu", 0);
            PlayerPrefs.Save();
        }
        else
        {
           
            SetGameElementsActive(false); 
            if (gameOverPanel != null) gameOverPanel.SetActive(false);

            if (startMenuPanel != null)
            {
                startMenuPanel.SetActive(false);
                DOVirtual.DelayedCall(0.1f, () => {
                    ShowMenuPanel(startMenuPanel);
                });
            }
        }
    }

    void ShowMenuPanel(GameObject panel)
    {
        panel.SetActive(true);

        MainMenuController menuController = panel.GetComponent<MainMenuController>();
        if (menuController != null)
        {
            menuController.PlayMenuAnimation();
            return;
        }

        AnimateMenuPanelDirectly(panel);
    }

    void AnimateMenuPanelDirectly(GameObject panel)
    {
        Image bgImage = panel.GetComponent<Image>();
        if (bgImage != null)
        {
            Color bgColor = bgImage.color;
            float originalAlpha = bgColor.a;
            bgColor.a = 0f;
            bgImage.color = bgColor;
            bgImage.DOFade(originalAlpha, 0.8f).SetEase(Ease.OutQuad).SetLink(panel);
        }

        TextMeshProUGUI[] texts = panel.GetComponentsInChildren<TextMeshProUGUI>(true);
        Button[] buttons = panel.GetComponentsInChildren<Button>(true);

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].gameObject == panel) continue;
            texts[i].alpha = 0f;
            texts[i].transform.localScale = Vector3.zero;
            float delay = i * 0.15f;
            texts[i].DOFade(1f, 0.5f).SetDelay(delay).SetEase(Ease.OutQuad).SetLink(texts[i].gameObject);
            texts[i].transform.DOScale(Vector3.one, 0.5f).SetDelay(delay).SetEase(Ease.OutBack).SetLink(texts[i].gameObject);
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            if (buttons[i].gameObject == panel) continue;
            buttons[i].gameObject.transform.localScale = Vector3.zero;
            CanvasGroup btnCG = buttons[i].GetComponent<CanvasGroup>();
            if (btnCG == null) btnCG = buttons[i].gameObject.AddComponent<CanvasGroup>();
            btnCG.alpha = 0f;

            float delay = (texts.Length * 0.15f) + (i * 0.2f);
            buttons[i].transform.DOScale(Vector3.one, 0.5f).SetDelay(delay).SetEase(Ease.OutBack).SetLink(buttons[i].gameObject);
            btnCG.DOFade(1f, 0.5f).SetDelay(delay).SetLink(buttons[i].gameObject);

            
            if (buttons[i].GetComponent<ButtonAnimator>() == null)
            {
                buttons[i].gameObject.AddComponent<ButtonAnimator>();
            }
        }
    }

    void HideMenuPanel(GameObject panel)
    {
        if (panel == null) return;
        CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
        if (canvasGroup == null) canvasGroup = panel.AddComponent<CanvasGroup>();

        canvasGroup.DOFade(0f, panelFadeDuration).SetEase(Ease.InQuad).SetLink(panel)
            .OnComplete(() => {
                panel.SetActive(false);
            });
        panel.transform.DOScale(Vector3.one * 0.8f, panelFadeDuration).SetEase(Ease.InBack).SetLink(panel);
    }

    
    public void StartGame()
    {
        if (playButton != null)
        {
            playButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f).SetLink(playButton.gameObject);
        }

        if (startMenuPanel != null)
        {
            MainMenuController menuController = startMenuPanel.GetComponent<MainMenuController>();
            if (menuController != null)
            {
    
                menuController.AnimateMenuExit(() => {
                    startMenuPanel.SetActive(false);

    
                    SetGameElementsActive(true);
                });
            }
            else
            {
                HideMenuPanel(startMenuPanel);
                SetGameElementsActive(true);
            }
        }
        else
        {
            SetGameElementsActive(true);
        }
    }

    public void RestartGame()
    {
        if (restartButton != null)
        {
            restartButton.transform.DOPunchScale(Vector3.one * 0.2f, 0.3f, 5, 0.5f).SetLink(restartButton.gameObject);
        }

        PlayerPrefs.SetInt("RestartDurumu", 1);
        PlayerPrefs.Save();

        DOVirtual.DelayedCall(0.3f, () => {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        });
    }

    public void ShowGameOver()
    {
        Debug.Log("UIManager.ShowGameOver() çağrıldı!");
        
        if (gameOverPanel != null)
        {
            
            gameOverPanel.SetActive(true);
            
            
            GameOverController controller = gameOverPanel.GetComponent<GameOverController>();
            if (controller != null && ScoreManager.Instance != null && DatabaseManager.Instance != null)
            {
                int finalScore = ScoreManager.Instance.GetCurrentScore();
                int highScore = DatabaseManager.Instance.GetHighScoreFromSQL();
                bool isNewRecord = finalScore > highScore;
                
                Debug.Log($"Game Over gösteriliyor - Skor: {finalScore}, En Yüksek: {highScore}, Yeni Rekor: {isNewRecord}");
                
            
                controller.ShowGameOverPanel(finalScore, highScore, isNewRecord);
            }
            else
            {
                Debug.LogWarning("GameOverController, ScoreManager veya DatabaseManager bulunamadı! Fallback kullanılıyor.");
            
                ShowMenuPanel(gameOverPanel);
            }
        }
        else
        {
            Debug.LogError("Game Over paneli bulunamadı!");
        }
    }

}