using UnityEngine;
using UnityEngine.UI;

public class ScrollingBackground : MonoBehaviour
{
    [Header("Ayarlar")]
    public float scrollSpeedX = 0.5f;
    public float scrollSpeedY = 0.5f;

    private RawImage _rawImage;
    private Rect _uvRect;

    void Start()
    {
       
       
        _rawImage = GetComponent<RawImage>();
        if (_rawImage != null) _uvRect = _rawImage.uvRect;
    }

    void Update()
    {
        if (_rawImage == null) return;

       
        _uvRect.x += scrollSpeedX * Time.deltaTime;
        _uvRect.y += scrollSpeedY * Time.deltaTime;

        _rawImage.uvRect = _uvRect;
    }
}