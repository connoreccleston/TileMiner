using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Sprite DebugSprite;

    private float lastSize = 0;
    private Vector3 lastPos;

    private WorldManager wm;
    private GameObject container;

    private Stack<GameObject> extraSprites = new Stack<GameObject>();

    private void Awake()
    {
        wm = FindObjectOfType<WorldManager>();
        lastPos = Camera.main.transform.position;
        container = new GameObject("Sprite Container");
    }

    private void CreateSprite(int x, int y)
    {
        GameObject go;
        SpriteRenderer sr;
        if (extraSprites.Count > 0)
        {
            go = extraSprites.Pop();
            sr = go.GetComponent<SpriteRenderer>();
        }
        else
        {
            go = new GameObject();
            sr = go.AddComponent<SpriteRenderer>();
            BoxCollider2D bc = go.AddComponent<BoxCollider2D>();
            bc.size = new Vector2(1, 1);
            go.transform.SetParent(container.transform, true);
        }
        go.transform.position = new Vector3(x, y, 0);
        TileType type = wm.GetTileType(go);
        sr.sprite = wm.TileTable[type].Sprite;
        go.name = type.ToString();
    }

    RaycastHit2D[] hit = new RaycastHit2D[1];
	private void Update()
	{
        if (Camera.main.orthographicSize != lastSize || Camera.main.transform.position != lastPos) // threshold?
        {
            lastSize = Camera.main.orthographicSize;
            lastPos = Camera.main.transform.position;

            int left = (int)(Camera.main.transform.position.x - Camera.main.orthographicSize * Camera.main.aspect) - 1;
            int right = (int)(Camera.main.transform.position.x + Camera.main.orthographicSize * Camera.main.aspect) + 2;
            int bottom = (int)(Camera.main.transform.position.y - Camera.main.orthographicSize) - 1;
            int top = (int)(Camera.main.transform.position.y + Camera.main.orthographicSize) + 2;

            // what about two big box traces put into sets?
            for (int x = left; x <= right; x++)
            {
                for (int y = bottom; y <= top; y++)
                {
                    if (x == left || x == right || y == bottom || y == top)
                    {
                        if (Physics2D.LinecastNonAlloc(new Vector2(x, y), new Vector2(x, y), hit) == 1)
                        {
                            Destroy(hit[0].transform.gameObject);
                            // fragmentation if moving too fast, getting added multiple times?
                            //extraSprites.Push(hit[0].transform.gameObject);
                            //hit[0].transform.gameObject.GetComponent<SpriteRenderer>().sprite = DebugSprite;
                        }
                    }
                    else
                    {
                        if (Physics2D.LinecastNonAlloc(new Vector2(x, y), new Vector2(x, y), hit) == 0)
                        {
                            CreateSprite(x, y);
                        }
                    }
                }
            }
        }
    }
}
