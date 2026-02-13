using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class MainMenuController : MonoBehaviour
{
    [Header("UI Elemanları")]
    public RectTransform logoRect;
    public RectTransform playButtonRect;
    public RectTransform highScoreRect;
    public TextMeshProUGUI highScoreText;

    [Header("Animasyon Ayarları")]
    public float floatStrength = 15f;
    public float floatDuration = 2f;

    private float logoStartY;

    void Awake()
    {
        if (logoRect != null) logoStartY = logoRect.anchoredPosition.y;
    }

    void OnEnable()
    {
    
        UpdateHighScore();
    }

    
    void OnDisable()
    {
    
    

        if (logoRect != null) logoRect.DOKill();
        if (playButtonRect != null) playButtonRect.DOKill();
        if (highScoreRect != null) highScoreRect.DOKill();

    
        transform.DOKill();
    }
    

    void UpdateHighScore()
    {
        if (highScoreText != null && DatabaseManager.Instance != null)
        {
            int best = DatabaseManager.Instance.GetHighScoreFromSQL();
            highScoreText.text = $"EN YÜKSEK SKOR\n<size=120%>{best}</size>";
        }
    }

    void ResetPositions()
    {
        if (logoRect != null) logoRect.anchoredPosition = new Vector2(logoRect.anchoredPosition.x, 500);
        if (playButtonRect != null) playButtonRect.localScale = Vector3.zero;
        if (highScoreRect != null) highScoreRect.localScale = Vector3.zero;
    }

    public void PlayMenuAnimation()
    {
        
        if (!gameObject.activeInHierarchy) return;

        ResetPositions();

        
        if (logoRect != null)
        {
            logoRect.DOKill(); 
            logoRect.DOAnchorPosY(logoStartY, 1f)
                .SetEase(Ease.OutBounce)
                .SetLink(logoRect.gameObject) 
                .OnComplete(() => StartIdleAnimations());
        }

        
        if (playButtonRect != null)
        {
            playButtonRect.DOKill();
            playButtonRect.DOScale(Vector3.one, 0.8f)
                .SetDelay(0.5f)
                .SetEase(Ease.OutBack)
                .SetLink(playButtonRect.gameObject);
        }

        
        if (highScoreRect != null)
        {
            highScoreRect.DOKill();
            highScoreRect.DOScale(Vector3.one, 0.6f)
                .SetDelay(0.7f)
                .SetEase(Ease.OutBack)
                .SetLink(highScoreRect.gameObject);
        }
    }

    public void AnimateMenuExit(System.Action onComplete)
    {
           

        if (playButtonRect != null)
            playButtonRect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetLink(playButtonRect.gameObject);

        if (highScoreRect != null)
            highScoreRect.DOScale(Vector3.zero, 0.3f).SetEase(Ease.InBack).SetLink(highScoreRect.gameObject);

        if (logoRect != null)
        {
            logoRect.DOAnchorPosY(logoRect.anchoredPosition.y + 500, 0.5f)
                .SetDelay(0.1f)
                .SetEase(Ease.InBack)
                .SetLink(logoRect.gameObject)
                .OnComplete(() => {
                    onComplete?.Invoke();
                });
        }
        else
        {
            
            onComplete?.Invoke();
        }
    }

    void StartIdleAnimations()
    {
        
        if (this == null || !gameObject.activeInHierarchy) return;

        if (logoRect != null)
        {
            logoRect.DOKill();
            logoRect.DOAnchorPosY(logoStartY + floatStrength, floatDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(logoRect.gameObject);
        }

        if (playButtonRect != null)
        {
            playButtonRect.DOKill();
            playButtonRect.DOScale(Vector3.one * 1.1f, 0.8f)
                .SetEase(Ease.InOutQuad)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(playButtonRect.gameObject);
        }
    }

    public void OnPlayButtonPressed()
    {
        if (AudioManager.Instance) AudioManager.Instance.PlayClickSound();

        if (playButtonRect != null)
        {
            playButtonRect.DOKill(); 

 
            playButtonRect.DOPunchScale(Vector3.one * 0.2f, 0.2f, 10, 1f)
                .SetLink(playButtonRect.gameObject)
                .OnComplete(() => {
 
                    if (UIManager.Instance != null) UIManager.Instance.StartGame();
                });
        }
        else
        {
            if (UIManager.Instance != null) UIManager.Instance.StartGame();
        }
    }
}