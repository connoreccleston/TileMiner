using UnityEngine;
using DG.Tweening;

// Find way to prevent input when tiles in view are flipping.

public class PlayerController : MonoBehaviour
{
    public float ZoomSpeed = 250;
    public float MineSpeed = 1;
    public float MineRadius = 5;

    public static Direction Facing { get; private set; }
    public static Vector3 Position { get; private set; }

    private void Start()
    {
        Facing = Direction.North;
        Position = transform.position + new Vector3(0, 0, 2);
        Camera.main.transform.parent.transform.position = Position;
        World.SetTileType(Position, TileType.Air);
    }

    private void Update()
    {
        Vector3 tempPos = Position;
        bool dirtyWorld = false;

        Vector3 aVector, dVector, qVector, eVector;
        aVector = dVector = qVector = eVector = new Vector3();

        switch (Facing)
        {
            case Direction.North:
                aVector = Vector3.left;
                dVector = Vector3.right;
                qVector = Vector3.back;
                eVector = Vector3.forward;
                break;
            case Direction.East:
                aVector = Vector3.forward;
                dVector = Vector3.back;
                qVector = Vector3.left;
                eVector = Vector3.right;
                break;
            case Direction.South:
                aVector = Vector3.right;
                dVector = Vector3.left;
                qVector = Vector3.forward;
                eVector = Vector3.back;
                break;
            case Direction.West:
                aVector = Vector3.back;
                dVector = Vector3.forward;
                qVector = Vector3.right;
                eVector = Vector3.left;
                break;
        }

        // Movement
        if (Input.GetKeyDown(KeyCode.W))
            tempPos += Vector3.up;
        if (Input.GetKeyDown(KeyCode.S))
            tempPos += Vector3.down;
        if (Input.GetKeyDown(KeyCode.A))
            tempPos += aVector;
        if (Input.GetKeyDown(KeyCode.D))
            tempPos += dVector;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            tempPos += qVector;
            dirtyWorld = true;
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            tempPos += eVector;
            dirtyWorld = true;
        }

        int depth;
        bool freeSpace = World.GetTileType(tempPos, out depth) == TileType.Air || depth > 0;
        if (freeSpace && tempPos != Position)
            Position = tempPos;
        else
            dirtyWorld = false;

        // Turning
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    Facing = (Direction)(((int)Facing - 1 + 4) % 4);
        //    dirtyWorld = true;
        //}
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    Facing = (Direction)(((int)Facing + 1) % 4);
        //    dirtyWorld = true;
        //}

        if (dirtyWorld)
            TileBehaviour.ResetAll();

        Mouse();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        // Follow
        // Try doing these as lerp(currentPos, desiredPos, constant) or smoothdamp or movetowards
        // Also change distance so it's closer to the edge
        transform.DOMove(new Vector3(Position.x, Position.y), 0.25f);
        if (Vector3.Distance(Position, Camera.main.transform.parent.transform.position) > Camera.main.orthographicSize - 2)
            Camera.main.transform.parent.DOMove(new Vector3(Position.x, Position.y), 0.75f);
    }

    RaycastHit2D[] hit = new RaycastHit2D[1];
    private void Mouse()
    {
        float size = Camera.main.orthographicSize - Input.GetAxis("Mouse ScrollWheel") * ZoomSpeed * Time.deltaTime;
        Camera.main.orthographicSize = Mathf.Clamp(size, 2, 16);

        if (Input.GetMouseButton(0))
        {
            Vector3 pos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int numHits = Physics2D.LinecastNonAlloc(transform.position, pos, hit, LayerMask.GetMask("Tiles"));
            if (numHits == 0)
                numHits = Physics2D.LinecastNonAlloc(pos, pos, hit);
            if (numHits != 0)
                if (Vector3.Distance(transform.position, hit[0].transform.position) < MineRadius)
                    hit[0].transform.GetComponent<TileBehaviour>().Mine(MineSpeed * Time.deltaTime);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            PlaceTile(TileType.Pylon);
        }
        else if (Input.GetMouseButtonDown(2))
        {
            PlaceTile(TileType.Generator);
        }
    }

    private void PlaceTile(TileType type)
    {
        Vector3 pos = (Vector2)Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int numHits = Physics2D.LinecastNonAlloc(pos, pos, hit);
        if (numHits != 0)
        {
            TileBehaviour tb = hit[0].transform.GetComponent<TileBehaviour>();
            bool inRange = Vector3.Distance(transform.position, hit[0].transform.position) < MineRadius;
            bool visible = Physics2D.LinecastNonAlloc(transform.position, pos, hit, LayerMask.GetMask("Tiles")) == 0;
            if (inRange && visible && tb.Depth > 0)
            {
                World.SetTileType(new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(Position.z)), type);
                tb.ResetTile(false);
            }
        }
    }
}

public enum Direction
{
    North = 0,
    East = 1,
    South = 2,
    West = 3
}
