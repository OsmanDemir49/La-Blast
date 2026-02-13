using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

/// <summary>
/// Butonlara otomatik animasyon ekleyen component.
/// Inspector'dan herhangi bir butona eklenebilir.
/// </summary>
[RequireComponent(typeof(Button))]
public class ButtonAnimator : MonoBehaviour
{
    [Header("Animasyon Ayarları")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float animationDuration = 0.2f;
    
    [Header("Idle Pulse Ayarları")]
    [SerializeField] private bool startWithIdlePulse = false;
    [SerializeField] private float pulseScale = 1.05f;
    [SerializeField] private float pulseDuration = 1f;
    
    [Header("Ses Ayarları")]
    [SerializeField] private bool playHoverSound = true;
    [SerializeField] private bool playClickSound = true;

    private Button button;
    private Vector3 originalScale;
    private Tween idlePulseTween;
    private bool isHovering = false;

    void Awake()
    {
        button = GetComponent<Button>();
        originalScale = transform.localScale;
    }

    void Start()
    {
        SetupHoverAnimation();
        
        if (startWithIdlePulse)
        {
            StartIdlePulse();
        }
    }

    void OnDisable()
    {
        // Component devre dışı kalırsa tüm animasyonları durdur
        transform.DOKill();
        idlePulseTween?.Kill();
    }

    /// <summary>
    /// Hover ve click animasyonlarını otomatik kurar
    /// </summary>
    public void SetupHoverAnimation()
    {
        var trigger = gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<EventTrigger>();
        }

        // Önceki trigger'ları temizle (tekrar kurulum durumunda)
        trigger.triggers.Clear();

        // Pointer Enter (Mouse üzerine gelince)
        var pointerEnter = new EventTrigger.Entry();
        pointerEnter.eventID = EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnPointerEnter());
        trigger.triggers.Add(pointerEnter);

        // Pointer Exit (Mouse ayrılınca)
        var pointerExit = new EventTrigger.Entry();
        pointerExit.eventID = EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => OnPointerExit());
        trigger.triggers.Add(pointerExit);

        // Pointer Down (Tıklanınca)
        var pointerDown = new EventTrigger.Entry();
        pointerDown.eventID = EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => OnPointerDown());
        trigger.triggers.Add(pointerDown);

        // Pointer Up (Tıklama bırakılınca)
        var pointerUp = new EventTrigger.Entry();
        pointerUp.eventID = EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => OnPointerUp());
        trigger.triggers.Add(pointerUp);
    }

    void OnPointerEnter()
    {
        if (!button.interactable) return;
        
        isHovering = true;
        StopIdlePulse(); // Idle pulse varsa durdur

        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, animationDuration)
            .SetEase(Ease.OutBack)
            .SetLink(gameObject);

        if (playHoverSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
    }

    void OnPointerExit()
    {
        if (!button.interactable) return;
        
        isHovering = false;

        transform.DOKill();
        transform.DOScale(originalScale, animationDuration)
            .SetEase(Ease.InBack)
            .SetLink(gameObject)
            .OnComplete(() => {
                // Hover bitince idle pulse varsa tekrar başlat
                if (startWithIdlePulse && !isHovering)
                {
                    StartIdlePulse();
                }
            });
    }

    void OnPointerDown()
    {
        if (!button.interactable) return;

        transform.DOKill();
        transform.DOScale(originalScale * clickScale, animationDuration * 0.5f)
            .SetEase(Ease.InQuad)
            .SetLink(gameObject);

        if (playClickSound && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayClickSound();
        }
    }

    void OnPointerUp()
    {
        if (!button.interactable) return;

        transform.DOKill();
        transform.DOScale(originalScale * hoverScale, animationDuration * 0.5f)
            .SetEase(Ease.OutQuad)
            .SetLink(gameObject);
    }

    /// <summary>
    /// Sürekli pulse animasyonu başlatır (idle durumda)
    /// </summary>
    public void StartIdlePulse()
    {
        if (idlePulseTween != null && idlePulseTween.IsActive())
        {
            return; // Zaten çalışıyor
        }

        transform.DOKill();
        transform.localScale = originalScale;

        idlePulseTween = transform.DOScale(originalScale * pulseScale, pulseDuration)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
    }

    /// <summary>
    /// Idle pulse animasyonunu durdurur
    /// </summary>
    public void StopIdlePulse()
    {
        if (idlePulseTween != null)
        {
            idlePulseTween.Kill();
            idlePulseTween = null;
        }
    }

    /// <summary>
    /// Punch animasyonu yapar (tıklama feedback)
    /// </summary>
    public void PlayPunchAnimation(float strength = 0.2f, float duration = 0.3f)
    {
        StopIdlePulse();
        transform.DOKill();
        transform.DOPunchScale(Vector3.one * strength, duration, 5, 0.5f)
            .SetLink(gameObject)
            .OnComplete(() => {
                if (startWithIdlePulse && !isHovering)
                {
                    StartIdlePulse();
                }
            });
    }
}
