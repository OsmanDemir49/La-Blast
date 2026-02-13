using UnityEngine;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public bool isOccupied;

    public void Init(int x, int y)
    {
        this.x = x;
        this.y = y;
        isOccupied = false;
    }
}