using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MoveSpeed = 5;
    public float ZoomSpeed = 100;

    private WorldManager wm;

    private void Awake()
    {
        wm = FindObjectOfType<WorldManager>();
    }

    private void Update()
	{
        int x = (Input.GetKey(KeyCode.A) ? -1 : 0) + (Input.GetKey(KeyCode.D) ? 1 : 0);
        int y = (Input.GetKey(KeyCode.S) ? -1 : 0) + (Input.GetKey(KeyCode.W) ? 1 : 0);
        Camera.main.transform.position += new Vector3(x, y, 0) * MoveSpeed * Time.deltaTime;

        float size = Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(size, 0.5f, 16);

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GameObject tile = Physics2D.Linecast(new Vector2(pos.x, pos.y), new Vector2(pos.x, pos.y)).transform.gameObject;
            wm.SetTileType(tile, TileType.Air);
            tile.GetComponent<SpriteRenderer>().sprite = wm.TileTable[TileType.Air].Sprite;
            tile.name = TileType.Air.ToString();
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
	}
}
