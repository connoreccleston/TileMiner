using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float MoveSpeed = 5;
    public float ZoomSpeed = 100;
    public float MineSpeed = 1;

    private RaycastHit2D[] hit = new RaycastHit2D[1];
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
            int hits = Physics2D.LinecastNonAlloc(new Vector2(pos.x, pos.y), new Vector2(pos.x, pos.y), hit);
            if (hits != 0)
                hit[0].transform.GetComponent<TileBehaviour>().Mine(MineSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
	}
}
