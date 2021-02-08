using System.Collections;
using System.Collections.Generic;
using UnityEngine;

struct V2
{
    public V2(int a, int b) { x = a; y = b; }
    public int x;
    public int y;
}

public class BackgroundManager : MonoBehaviour
{
    public Transform trigger;
    public float tileWidth;
    public float tileHeight;
    public GameObject tilePrefab;

    HashSet<V2> BGCheckPoints;
    V2 lastPos = new V2(0, 0);

    //update
    V2 pos;
    int signX;
    int signY;
    int diffX;
    int diffY;

    private void Awake()
    {
        BGCheckPoints = new HashSet<V2>();
        BGCheckPoints.Add(new V2(0, 0));
    }

    public void Update()
    {
        pos.x = (int)(trigger.position.x / tileWidth);
        pos.y = (int)(trigger.position.y / tileHeight);
        if (!BGCheckPoints.Contains(pos))
        {
            if (pos.x == lastPos.x)
            {
                diffY = pos.y - lastPos.y;
                Instantiate(tilePrefab, new Vector2(tileWidth * (pos.x - 1), tileHeight * (pos.y + diffY)), Quaternion.identity);
                Instantiate(tilePrefab, new Vector2(tileWidth * pos.x, tileHeight * (pos.y + diffY)), Quaternion.identity);
                Instantiate(tilePrefab, new Vector2(tileWidth * (pos.x + 1), tileHeight * (pos.y + diffY)), Quaternion.identity);
            }
            else
            {
                diffX = pos.x - lastPos.x;
                Instantiate(tilePrefab, new Vector2(tileWidth * (pos.x + diffX), tileHeight * (pos.y - 1)), Quaternion.identity);
                Instantiate(tilePrefab, new Vector2(tileWidth * (pos.x + diffX), tileHeight * pos.y), Quaternion.identity);
                Instantiate(tilePrefab, new Vector2(tileWidth * (pos.x + diffX), tileHeight * (pos.y + 1)), Quaternion.identity);
            }
            lastPos = pos;
            BGCheckPoints.Add(new V2(pos.x, pos.y));
        }
    }

}
