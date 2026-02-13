using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverController : MonoBehaviour
{
    [Header("UI Referansları")]
    public GameObject backgroundOverlay;
    public RectTransform contentPanel;
    public TextMeshProUGUI gameOverTitle;
    public TextMeshProUGUI highScoreText; 
    public RectTransform restartButton;

    [Header("Transparan Ayarları")]
    [SerializeField] private float overlayAlpha = 0.6f; 

    private bool isShowing = false;

    void OnEnable()
    {
        
        StartCoroutine(KeyboardInputHandler());
    }

    void OnDisable()
    {
        
        DOTween.Kill(this);
        if (contentPanel != null) contentPanel.DOKill();
        if (restartButton != null) restartButton.DOKill();
        if (gameOverTitle != null) gameOverTitle.DOKill();
    }

    System.Collections.IEnumerator KeyboardInputHandler()
    {
        while (gameObject.activeInHierarchy)
        {
            
            if (Input.GetKeyDown(KeyCode.R))
            {
                OnRestartPressed();
            }
            
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnRestartPressed();
            }

            yield return null;
        }
    }

    public void ShowGameOverPanel(int finalScore, int highScore, bool isNewRecord)
    {
        if (isShowing) return;
        isShowing = true;

        
        if (isNewRecord && DatabaseManager.Instance != null)
        {
            DatabaseManager.Instance.SaveScoreToSQL(finalScore);
            highScore = finalScore; 
        }

        
        if (highScoreText != null)
        {
            highScoreText.text = $"EN YÜKSEK SKOR\n<size=120%>{highScore}</size>";
        }

        
        ResetPositions();

        
        PlayGameOverSequence();
    }

    void ResetPositions()
    {
        
        if (backgroundOverlay != null)
        {
            CanvasGroup bg = backgroundOverlay.GetComponent<CanvasGroup>();
            if (bg == null) bg = backgroundOverlay.AddComponent<CanvasGroup>();
            bg.alpha = 0f;
        }

        
        if (contentPanel != null)
        {
            contentPanel.localScale = Vector3.zero;
        }

        
        if (gameOverTitle != null)
        {
            gameOverTitle.alpha = 0f;
        }

        
        if (highScoreText != null)
        {
            highScoreText.alpha = 0f;
        }

        
        if (restartButton != null)
        {
            restartButton.localScale = Vector3.zero;
        }
    }

    void PlayGameOverSequence()
    {
        Sequence seq = DOTween.Sequence();
        seq.SetLink(gameObject);

        
        if (backgroundOverlay != null)
        {
            CanvasGroup bg = backgroundOverlay.GetComponent<CanvasGroup>();
            seq.Append(bg.DOFade(overlayAlpha, 0.4f).SetEase(Ease.OutQuad));
        }

        
        if (contentPanel != null)
        {
            seq.Append(contentPanel.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack));
        }

        
        if (gameOverTitle != null)
        {
            seq.AppendCallback(() => {
                gameOverTitle.DOFade(1f, 0.4f).SetLink(gameOverTitle.gameObject);
                if (AudioManager.Instance) AudioManager.Instance.PlayGameOverSound();
            });
            seq.AppendInterval(0.3f);
        }

        
        if (highScoreText != null)
        {
            seq.AppendCallback(() => {
                highScoreText.DOFade(1f, 0.4f).SetLink(highScoreText.gameObject);
            });
            seq.AppendInterval(0.3f);
        }

        
        if (restartButton != null)
        {
            seq.AppendCallback(() => {
                restartButton.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack).SetLink(restartButton.gameObject);
                
                
                ButtonAnimator btnAnimator = restartButton.GetComponent<ButtonAnimator>();
                if (btnAnimator == null)
                {
                    btnAnimator = restartButton.gameObject.AddComponent<ButtonAnimator>();
                }
                btnAnimator.StartIdlePulse();
            });
        }

        seq.Play();
    }

    public void OnRestartPressed()
    {
        
        ButtonAnimator btnAnimator = restartButton != null ? restartButton.GetComponent<ButtonAnimator>() : null;
        if (btnAnimator != null)
        {
            btnAnimator.PlayPunchAnimation();
        }

        if (AudioManager.Instance) AudioManager.Instance.PlayClickSound();

        
        DOVirtual.DelayedCall(0.3f, () => {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.RestartGame();
            }
        }).SetLink(gameObject);
    }
}
