using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;



public class UIHelper : MonoBehaviour
{
    [Header("Buton Stilleri")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color hoverColor = new Color(0.9f, 0.9f, 1f);
    [SerializeField] private Color pressedColor = new Color(0.8f, 0.8f, 1f);

    [Header("Animasyon Ayarları")]
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float clickScale = 0.95f;
    [SerializeField] private float animationDuration = 0.2f;

    private Button button;
    private Image buttonImage;
    private Vector3 originalScale;
    private Color originalColor;

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        if (button != null)
        {
            originalScale = transform.localScale;
            if (buttonImage != null)
            {
                originalColor = buttonImage.color;
            }

            SetupButtonAnimations();
        }
    }

    void SetupButtonAnimations()
    {
        
        var trigger = gameObject.GetComponent<UnityEngine.EventSystems.EventTrigger>();
        if (trigger == null)
        {
            trigger = gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
        }

        
        var pointerEnter = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerEnter.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
        pointerEnter.callback.AddListener((data) => OnPointerEnter());
        trigger.triggers.Add(pointerEnter);

        
        var pointerExit = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
        pointerExit.callback.AddListener((data) => OnPointerExit());
        trigger.triggers.Add(pointerExit);

        
        var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
        pointerDown.callback.AddListener((data) => OnPointerDown());
        trigger.triggers.Add(pointerDown);

        
        var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
        pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
        pointerUp.callback.AddListener((data) => OnPointerUp());
        trigger.triggers.Add(pointerUp);
    }

    void OnPointerEnter()
    {
        transform.DOScale(originalScale * hoverScale, animationDuration).SetEase(Ease.OutBack);
        if (buttonImage != null)
        {
            buttonImage.DOColor(hoverColor, animationDuration);
        }
    }

    void OnPointerExit()
    {
        transform.DOScale(originalScale, animationDuration).SetEase(Ease.InBack);
        if (buttonImage != null)
        {
            buttonImage.DOColor(originalColor, animationDuration);
        }
    }

    void OnPointerDown()
    {
        transform.DOScale(originalScale * clickScale, animationDuration * 0.5f).SetEase(Ease.InQuad);
        if (buttonImage != null)
        {
            buttonImage.DOColor(pressedColor, animationDuration * 0.5f);
        }
    }

    void OnPointerUp()
    {
        transform.DOScale(originalScale * hoverScale, animationDuration * 0.5f).SetEase(Ease.OutQuad);
        if (buttonImage != null)
        {
            buttonImage.DOColor(hoverColor, animationDuration * 0.5f);
        }
    }

    
    
    public static void AddPulseAnimation(TextMeshProUGUI text, float scale = 1.1f, float duration = 1f)
    {
        if (text != null)
        {
            text.transform.DOScale(scale, duration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo);
        }
    }

    
    
    
    public static void FadeInText(TextMeshProUGUI text, float duration = 0.5f)
    {
        if (text != null)
        {
            text.alpha = 0f;
            text.DOFade(1f, duration).SetEase(Ease.OutQuad);
        }
    }

    
    
    
    public static void FadeOutText(TextMeshProUGUI text, float duration = 0.5f)
    {
        if (text != null)
        {
            text.DOFade(0f, duration).SetEase(Ease.InQuad);
        }
    }
}
