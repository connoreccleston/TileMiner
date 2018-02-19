using UnityEngine;

// Find way to prevent input when tiles in view are flipping.

public class PlayerController : MonoBehaviour
{
    public float CamSpeed = 10;
    public float ZoomSpeed = 250;
    public float MoveSpeed = 25;
    public float MineSpeed = 1;
    public float MineRadius = 5;

    public static Direction Facing { get; private set; }
    public static Vector3 WorldPos { get; private set; }

    private void Start()
    {
        Facing = Direction.North;
        WorldPos = new Vector3(World.Size / 2, World.Size / 2, World.Size / 2);
        transform.position = (Vector2)WorldPos;
        Camera.main.transform.parent.transform.position = transform.position;
        World.SetTileType(WorldPos, TileType.Air);
    }

    private void Update()
    {
        MoveUpdate();

        MouseUpdate();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        FollowUpdate();
    }

    private static void MoveUpdate()
    {
        Vector3 tempPos = WorldPos;
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
        if (freeSpace && tempPos != WorldPos)
            WorldPos = tempPos;
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
    }

    bool keepMoving;
    private void FollowUpdate()
    {
        // Follow
        // LogicalPosition will have to change with direction
        Vector3 vel = new Vector3();
        transform.position = Vector3.SmoothDamp(transform.position, (Vector2)WorldPos, ref vel, 1 / MoveSpeed);

        Transform camTrans = Camera.main.transform.parent.transform;
        bool x = Mathf.Abs(transform.position.x - camTrans.position.x) > Camera.main.orthographicSize * Camera.main.aspect - 2;
        bool y = Mathf.Abs(transform.position.y - camTrans.position.y) > Camera.main.orthographicSize - 2;
        if (x || y || keepMoving || Input.GetKeyDown(KeyCode.C))
        {
            keepMoving = true;
            camTrans.position = Vector3.MoveTowards(camTrans.position, (Vector2)transform.position, CamSpeed * Time.deltaTime);
        }
        if (Vector3.Distance(transform.position, camTrans.position) == 0)
            keepMoving = false;
    }

    RaycastHit2D[] hit = new RaycastHit2D[1];
    private void MouseUpdate()
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
                World.SetTileType(new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(WorldPos.z)), type);
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
