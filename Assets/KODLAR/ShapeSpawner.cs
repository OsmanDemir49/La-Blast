using UnityEngine;

public class ShapeSpawner : MonoBehaviour
{
    public static ShapeSpawner Instance;

    public GameObject[] shapePrefabs;
    public Transform[] spawnPoints;

    private int _activeShapeCount = 0;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SpawnNewBatch();
    }

    public void SpawnNewBatch()
    {
        
        _activeShapeCount = 0;

        foreach (Transform point in spawnPoints)
        {
            if (point.childCount > 0) Destroy(point.GetChild(0).gameObject);

            int randomIndex = Random.Range(0, shapePrefabs.Length);
            GameObject newShape = Instantiate(shapePrefabs[randomIndex], point.position, Quaternion.identity);

            newShape.transform.SetParent(point); 

            newShape.transform.localScale = Vector3.one * 0.6f;
            _activeShapeCount++;
        }

        
    }

    public void DecreaseCount()
    {
        _activeShapeCount--;
        if (_activeShapeCount <= 0)
        {
            SpawnNewBatch();
        }
    }
}