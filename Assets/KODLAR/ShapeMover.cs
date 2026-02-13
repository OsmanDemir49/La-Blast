using UnityEngine;

public enum BlockType { Normal, RowColBomb, AreaBomb3x3 }

public class ShapeMover : MonoBehaviour
{
    [Header("Özel Ayarlar")]
    public BlockType currentBlockType = BlockType.Normal;
    [SerializeField] private Color[] _possibleColors;

    private Vector3 _startPosition;
    private bool _isDragging = false;
    private Vector3 _offset;
    private Vector3 _smallScale = Vector3.one * 0.6f;
    private Vector3 _normalScale = Vector3.one;

    void Start()
    {
        _startPosition = transform.position;
        AssignRandomColor();
        foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.sortingOrder = 10;
        transform.localScale = _smallScale;
    }

    void AssignRandomColor()
    {
        if (currentBlockType != BlockType.Normal) return;
        if (_possibleColors == null || _possibleColors.Length == 0) return;
        Color c = _possibleColors[Random.Range(0, _possibleColors.Length)];
        foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.color = c;
    }

    void OnMouseDown()
    {
        _isDragging = true;
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        _offset = transform.position - new Vector3(mousePos.x, mousePos.y, 0);
        transform.localScale = _normalScale;
        foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.sortingOrder = 100;
    }

    void OnMouseDrag()
    {
        if (_isDragging)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            transform.position = new Vector3(mousePos.x, mousePos.y, 0) + _offset;
        }
    }

    void OnMouseUp()
    {
        _isDragging = false;
        if (currentBlockType == BlockType.Normal) HandleNormalPlacement();
        else HandleBombPlacement(currentBlockType == BlockType.AreaBomb3x3);
    }

    void HandleNormalPlacement()
    {
       
        if (CanPlaceShape())
        {
            PlaceShape();
        }
        else
        {
            Debug.Log("Sýðmadý, geri dönüyor.");
            ResetPosition();
        }
    }

    void HandleBombPlacement(bool is3x3)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int targetX = Mathf.RoundToInt(mousePos.x);
        int targetY = Mathf.RoundToInt(mousePos.y);
        Tile t = GridManager.Instance.GetTileAt(targetX, targetY);

        if (t != null)
        {
            if (is3x3) GridManager.Instance.ExplodeArea3x3(targetX, targetY);
            else GridManager.Instance.ExplodeAt(targetX, targetY);
            FinishMove();
        }
        else ResetPosition();
    }

    void ResetPosition()
    {
        transform.position = _startPosition;
        transform.localScale = _smallScale;
        foreach (var r in GetComponentsInChildren<SpriteRenderer>()) r.sortingOrder = 10;
    }

    bool CanPlaceShape()
    {
        int anchorX = Mathf.RoundToInt(transform.position.x);
        int anchorY = Mathf.RoundToInt(transform.position.y);

        foreach (Transform child in transform)
        {
            int localX = Mathf.RoundToInt(child.localPosition.x);
            int localY = Mathf.RoundToInt(child.localPosition.y);
            int targetX = anchorX + localX;
            int targetY = anchorY + localY;

            Tile t = GridManager.Instance.GetTileAt(targetX, targetY);
            if (t == null || t.isOccupied) return false;
        }
        return true;
    }

    void PlaceShape()
    {
        Debug.Log("--- YERLEÞTÝRME BAÞLADI ---");
        if (AudioManager.Instance) AudioManager.Instance.PlayPlaceSound();

       
        Transform[] children = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++) children[i] = transform.GetChild(i);

        int anchorX = Mathf.RoundToInt(transform.position.x);
        int anchorY = Mathf.RoundToInt(transform.position.y);

        int placedCount = 0;

        foreach (Transform child in children)
        {
            int localX = Mathf.RoundToInt(child.localPosition.x);
            int localY = Mathf.RoundToInt(child.localPosition.y);
            int targetX = anchorX + localX;
            int targetY = anchorY + localY;

            Tile t = GridManager.Instance.GetTileAt(targetX, targetY);

            if (t != null)
            {
                
                child.SetParent(t.transform);
                child.localPosition = Vector3.zero;
                child.GetComponent<SpriteRenderer>().sortingOrder = 5;

                t.isOccupied = true; 
                Debug.Log($"Blok parçasý Tile ({targetX},{targetY}) içine koyuldu. Yeni Ebeveyn: {t.name}");
                placedCount++;
            }
            else
            {
                Debug.LogError($"HATA! ({targetX},{targetY}) konumunda Tile bulunamadý! Blok boþa düþtü!");
            }
        }

        Debug.Log($"Toplam {placedCount}/{children.Length} parça yerleþtirildi.");

        if (ScoreManager.Instance) ScoreManager.Instance.AddScore(10);

        
        GridManager.Instance.CheckForMatches();

        FinishMove();
    }

    void FinishMove()
    {
        
        if (ShapeSpawner.Instance != null)
        {
            ShapeSpawner.Instance.DecreaseCount();
            GridManager.Instance.CheckGameOver(ShapeSpawner.Instance.spawnPoints);
        }
        Destroy(gameObject);
    }
}