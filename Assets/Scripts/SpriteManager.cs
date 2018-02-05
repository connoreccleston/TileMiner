using System.Collections.Generic;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public Sprite DebugSprite;

    private float LastSize = 0;
    private Vector3 LastPos;

    private GameObject Container;

    private Stack<GameObject> ExtraSprites = new Stack<GameObject>();

    private void Awake()
    {
        LastPos = Camera.main.transform.position;
        Container = new GameObject("Sprite Container");
    }

    private void CreateSprite(int x, int y)
    {
        GameObject go;
        if (ExtraSprites.Count > 0)
        {
            go = ExtraSprites.Pop();
            go.GetComponent<TileBehaviour>().ResetTile(false);
        }
        else
        {
            go = new GameObject(null, typeof(TileBehaviour));
            go.transform.SetParent(Container.transform, true);
        }
        go.transform.position = new Vector3(x, y, 0);
    }

    RaycastHit2D[] hit = new RaycastHit2D[1];
	private void Update()
	{
        if (Camera.main.orthographicSize != LastSize || Camera.main.transform.position != LastPos) // threshold?
        {
            LastSize = Camera.main.orthographicSize;
            LastPos = Camera.main.transform.position;

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
                            // multiple new tiles will spawn if telling them to animate
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
