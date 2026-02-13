using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Ses Kaynakları")]
    [SerializeField] private AudioSource _sfxSource; // Efektler için (Patlama, tıklama)
    [SerializeField] private AudioSource _musicSource; // Arka plan müziği için

    [Header("Ses Dosyaları (Klipler)")]
    public AudioClip placeSound;    // Taş koyma sesi
    public AudioClip explodeSound;  // Satır patlama sesi
    public AudioClip gameOverSound; // Oyun bitiş sesi
    public AudioClip clickSound;    // Buton tıklama sesi (Opsiyonel)

    void Awake()
    {
        // Singleton Yapısı (Sahneler arası geçişte yok olmasın istersen DontDestroyOnLoad ekleyebilirsin)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        // Eğer AudioSource atanmamışsa otomatik oluştur
        if (_sfxSource == null)
        {
            _sfxSource = gameObject.AddComponent<AudioSource>();
            _sfxSource.playOnAwake = false;
            _sfxSource.loop = false;
            Debug.Log("AudioManager: SFX AudioSource otomatik oluşturuldu.");
        }

        if (_musicSource == null)
        {
            _musicSource = gameObject.AddComponent<AudioSource>();
            _musicSource.playOnAwake = false;
            _musicSource.loop = true;
            Debug.Log("AudioManager: Music AudioSource otomatik oluşturuldu.");
        }
    }

    // Tek seferlik efekt çalmak için genel fonksiyon
    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("AudioManager: Ses dosyası (AudioClip) atanmamış!");
            return;
        }

        if (_sfxSource == null)
        {
            Debug.LogError("AudioManager: SFX AudioSource bulunamadı! Start() fonksiyonu çalıştı mı?");
            return;
        }

        _sfxSource.PlayOneShot(clip); // PlayOneShot: Üst üste binse de kesilmeden çalar
    }

    // Özel kısayol fonksiyonları (Kod yazarken kolaylık olsun diye)
    public void PlayPlaceSound() => PlaySFX(placeSound);
    public void PlayExplosionSound() => PlaySFX(explodeSound);
    public void PlayGameOverSound() => PlaySFX(gameOverSound);
    public void PlayClickSound() => PlaySFX(clickSound);
    
    // ✅ YENİ: Game Over için ek sesler
    [Header("Game Over Sesleri (Opsiyonel)")]
    public AudioClip newRecordSound; // Yeni rekor fanfare sesi
    
    public void PlayNewRecordSound() => PlaySFX(newRecordSound);
}
