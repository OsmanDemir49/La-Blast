using UnityEngine;

/// <summary>
/// Tam ekranda bile oyunun dikey formatını (9:16) korur.
/// Yanlara siyah bantlar (pillarbox) ekler.
/// </summary>
public class FullscreenAspectRatioEnforcer : MonoBehaviour
{
    [Header("Mobil Dikey Ekran")]
    [Tooltip("9:16 aspect ratio")]
    public float targetAspect = 9f / 16f;

    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }

        if (cam == null)
        {
            Debug.LogError("FullscreenAspectRatioEnforcer: Camera bulunamadı!");
            return;
        }

        // Arka planı siyah yap
        cam.backgroundColor = Color.black;
        
        Debug.Log("✅ FullscreenAspectRatioEnforcer başlatıldı!");
    }

    void Start()
    {
        UpdateAspectRatio();
    }

    void Update()
    {
        // Her frame kontrol et (tam ekran geçişlerini yakala)
        UpdateAspectRatio();
    }

    void UpdateAspectRatio()
    {
        if (cam == null) return;

        float currentAspect = (float)Screen.width / Screen.height;
        float scaleHeight = currentAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Letterbox (üst/alt siyah)
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Pillarbox (yan siyah) - bizim istediğimiz
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}
