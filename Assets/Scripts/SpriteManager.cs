using System.Collections.Generic;
using UnityEngine;

// Should add a way to cull all the excess tiles when shrinking.

public class SpriteManager : MonoBehaviour
{
    private float LastSize = 0;
    private Vector3Int LastPos;
    private Stack<GameObject> Recycle = new Stack<GameObject>();
    private HashSet<GameObject> Seen = new HashSet<GameObject>();

    private GameObject Container;

    private void Awake()
    {
        LastPos = Vector3Int.RoundToInt(Camera.main.transform.position);
        Container = new GameObject("Sprite Container");
    }

    private void CreateSprite(int x, int y)
    {
        GameObject go;
        if (Recycle.Count > 0)
        {
            go = Recycle.Pop();
            Seen.Remove(go);
        }
        else
        {
            go = new GameObject(null, typeof(TileBehaviour));
        }
        go.transform.SetParent(Container.transform, true);
        go.transform.position = new Vector3(x, y, 0);
    }

    RaycastHit2D[] hit = new RaycastHit2D[1];
	private void Update()
	{
        Vector3Int newPos = Vector3Int.RoundToInt(Camera.main.transform.position);
        if (Camera.main.orthographicSize != LastSize || newPos != LastPos)
        {
            LastSize = Camera.main.orthographicSize;
            LastPos = newPos;

            int left = (int)(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect) - 1;
            int right = (int)(Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect) + 2;
            int bottom = (int)(Camera.main.transform.position.y - Camera.main.orthographicSize) - 1;
            int top = (int)(Camera.main.transform.position.y + Camera.main.orthographicSize) + 2;
            
            var area = Physics2D.OverlapAreaAll(new Vector2(left - 25, top + 25), new Vector2(right + 25, bottom - 25));
            HashSet <Collider2D> toDestroy = new HashSet<Collider2D>(area);
            toDestroy.ExceptWith(Physics2D.OverlapAreaAll(new Vector2(left, top), new Vector2(right, bottom)));

            foreach (var tile in toDestroy)
            {
                if (!Seen.Contains(tile.gameObject))
                {
                    Recycle.Push(tile.gameObject);
                    Seen.Add(tile.gameObject);
                }
            }

            for (int x = left; x <= right; x++)
                for (int y = bottom; y <= top; y++)
                    if (Physics2D.LinecastNonAlloc(new Vector2(x, y), new Vector2(x, y), hit) == 0)
                        CreateSprite(x, y);
        }
    }
}
