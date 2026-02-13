using UnityEngine;
using System.Collections.Generic;

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;

    [Header("Ayarlar")]
    public int width = 8;
    public int height = 8;
    [SerializeField] private GameObject _tilePrefab;

    [Header("Efektler")]
    public GameObject explosionEffectPrefab;

    private Tile[,] _tiles;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Time.timeScale = 1f; 
        GenerateGrid();
    }

    void GenerateGrid()
    {
        
        
        foreach (Transform child in this.transform)
        {
            Destroy(child.gameObject);
        }
        

        _tiles = new Tile[width, height];
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_tilePrefab == null) return;

                GameObject spawnedTile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);
                spawnedTile.name = $"Tile {x},{y}";
                spawnedTile.transform.parent = this.transform;

                var tileScript = spawnedTile.GetComponent<Tile>();
                tileScript.Init(x, y);
                _tiles[x, y] = tileScript;
            }
        }
    }

    public Tile GetTileAt(int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height) return null;
        return _tiles[x, y];
    }

    
    public void RefreshGridState()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (_tiles[x, y] != null)
                {
                    
                    bool hasVisualBlock = _tiles[x, y].transform.childCount > 0;
                    _tiles[x, y].isOccupied = hasVisualBlock;
                }
            }
        }
    }

    public void CheckForMatches()
    {
        
        RefreshGridState();

        List<int> rowsToClear = new List<int>();
        List<int> colsToClear = new List<int>();

        
        for (int y = 0; y < height; y++)
        {
            bool full = true;
            for (int x = 0; x < width; x++)
            {
                
                if (_tiles[x, y] != null && !_tiles[x, y].isOccupied)
                {
                    full = false;
                    break;
                }
            }
            if (full) rowsToClear.Add(y);
        }

        
        for (int x = 0; x < width; x++)
        {
            bool full = true;
            for (int y = 0; y < height; y++)
            {
                if (_tiles[x, y] != null && !_tiles[x, y].isOccupied)
                {
                    full = false;
                    break;
                }
            }
            if (full) colsToClear.Add(x);
        }

        foreach (int y in rowsToClear)
        {
            ClearRow(y);
            if (ScoreManager.Instance) ScoreManager.Instance.AddScore(100);
        }

        foreach (int x in colsToClear)
        {
            ClearColumn(x);
            if (ScoreManager.Instance) ScoreManager.Instance.AddScore(100);
        }

        if (rowsToClear.Count > 0 || colsToClear.Count > 0)
        {
            if (AudioManager.Instance) AudioManager.Instance.PlayExplosionSound();
        }

        
        RefreshGridState();
    }

    void ClearRow(int y) { for (int x = 0; x < width; x++) ClearTile(_tiles[x, y]); }
    void ClearColumn(int x) { for (int y = 0; y < height; y++) ClearTile(_tiles[x, y]); }

    void ClearTile(Tile t)
    {
        if (t == null) return;

        if (t.transform.childCount > 0)
        {
            GameObject blockObj = t.transform.GetChild(0).gameObject;

            if (explosionEffectPrefab != null)
            {
                GameObject explosion = Instantiate(explosionEffectPrefab, t.transform.position, Quaternion.identity);
                SpriteRenderer blockRenderer = blockObj.GetComponent<SpriteRenderer>();
                if (blockRenderer != null && explosion.TryGetComponent<ParticleSystem>(out ParticleSystem ps))
                {
                    var mainModule = ps.main;
                    mainModule.startColor = blockRenderer.color;
                }
                Destroy(explosion, 2f);
            }

            
            
            
            blockObj.transform.SetParent(null);
            Destroy(blockObj);
            
        }
        t.isOccupied = false;
    }

    public void ExplodeAt(int targetX, int targetY)
    {
        if (targetX < 0 || targetX >= width || targetY < 0 || targetY >= height) return;
        ClearRow(targetY);
        ClearColumn(targetX);
        if (AudioManager.Instance) AudioManager.Instance.PlayExplosionSound();
        RefreshGridState();
    }

    public void ExplodeArea3x3(int centerX, int centerY)
    {
        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                int targetX = centerX + xOffset;
                int targetY = centerY + yOffset;
                if (targetX >= 0 && targetX < width && targetY >= 0 && targetY < height)
                {
                    Tile t = _tiles[targetX, targetY];
                  
                    if (t.transform.childCount > 0) ClearTile(t);
                }
            }
        }
        if (AudioManager.Instance) AudioManager.Instance.PlayExplosionSound();
        RefreshGridState();
    }

    public bool CheckIfShapeCanFit(GameObject shapeObj)
    {
        if (shapeObj == null || shapeObj.transform.childCount == 0) return false;

        
        RefreshGridState();

        
        GameObject ghostShape = Instantiate(shapeObj, new Vector3(500, 500), Quaternion.identity);
        ghostShape.transform.SetParent(null); 
        ghostShape.transform.localScale = Vector3.one; 

        
        Transform[] children = new Transform[ghostShape.transform.childCount];
        for (int i = 0; i < ghostShape.transform.childCount; i++)
        {
            children[i] = ghostShape.transform.GetChild(i);
        
            children[i].localScale = Vector3.one;
        }
        

        bool canFit = false;

        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                if (CanGhostFitAt(children, x, y))
                {
                    canFit = true;
        
                    break;
                }
            }
            if (canFit) break;
        }

        Destroy(ghostShape);
        return canFit;
    }

    private bool CanGhostFitAt(Transform[] shapeParts, int gridX, int gridY)
    {
        foreach (Transform part in shapeParts)
        {
            int partX = gridX + Mathf.RoundToInt(part.localPosition.x);
            int partY = gridY + Mathf.RoundToInt(part.localPosition.y);

        
            if (partX < 0 || partX >= width || partY < 0 || partY >= height) return false;

        
        
            if (_tiles[partX, partY].transform.childCount > 0) return false;
        
        }
        return true;
    }

    public bool CheckGameOver(Transform[] spawnPointsList)
    {
        if (_tiles == null || spawnPointsList == null) return false;

        
        RefreshGridState();

        bool playableMoveExists = false;
        bool anyVisibleBlockExists = false;

        foreach (Transform point in spawnPointsList)
        {
            if (point == null) continue;
            
            if (point.childCount > 0)
            {
                GameObject shape = point.GetChild(0).gameObject;
                if (shape == null) continue;

        
                if (shape.activeSelf && shape.transform.localScale.x > 0.2f)
                {
                    anyVisibleBlockExists = true;

        
                    ShapeMover shapeMover = shape.GetComponent<ShapeMover>();
                    if (shapeMover != null && shapeMover.currentBlockType != BlockType.Normal)
                    {
        
                        playableMoveExists = true;
                        break;
                    }

        
                    if (CheckIfShapeCanFit(shape))
                    {
                        playableMoveExists = true;
                        break;
                    }
                }
            }
        }

        
        if (anyVisibleBlockExists && !playableMoveExists)
        {
            Debug.Log("!!! OYUN BİTTİ - Hiçbir blok sığmıyor !!!");

            if (AudioManager.Instance) AudioManager.Instance.PlayGameOverSound();

            if (ScoreManager.Instance != null && DatabaseManager.Instance != null)
            {
                int currentScore = ScoreManager.Instance.GetCurrentScore();
                DatabaseManager.Instance.SaveScoreToSQL(currentScore);

                int bestScore = DatabaseManager.Instance.GetHighScoreFromSQL();
                ScoreManager.Instance.UpdateHighScoreUI(bestScore);
                ScoreManager.Instance.UpdateFinalScoreUI();
            }

            if (UIManager.Instance != null) UIManager.Instance.ShowGameOver();
            return true;
        }
        return false;
    }
}
