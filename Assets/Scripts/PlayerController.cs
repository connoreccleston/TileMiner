using UnityEngine;

// Find way to prevent input when tiles in view are flipping.

public class PlayerController : MonoBehaviour
{
    public float CamSpeed = 10;
    public float ZoomSpeed = 250;
    public float MoveSpeed = 25;
    public float MineSpeed = 1;
    public float MineRadius = 5;

    //public static Direction Facing { get; private set; }
    private static Vector3 TargetPos; //{ get; private set; }

    private void Start()
    {
        //Facing = Direction.North;
        //WorldPos = new Vector3(World.Size / 2, World.Size / 2, World.Size / 2);
        transform.position = new Vector3(WorldNew.Origin.z, WorldNew.Origin.z); //(Vector2)WorldPos;
        Camera.main.transform.parent.transform.position = transform.position;
        TargetPos = transform.position;
        //World.SetTileType(WorldPos, TileType.Air);
    }

    private void Update()
    {
        MoveUpdate();

        MouseUpdate();

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

        FollowUpdate();
    }

    private void MoveUpdate()
    {
        bool dirtyWorld = false;

        // Movement
        Vector3 offset = Vector3.zero;
        if (Input.GetKeyDown(KeyCode.W))
            offset = Vector3.up;
        if (Input.GetKeyDown(KeyCode.S))
            offset = Vector3.down;
        if (Input.GetKeyDown(KeyCode.A))
            offset = Vector3.left;
        if (Input.GetKeyDown(KeyCode.D))
            offset = Vector3.right;
        if (Input.GetKeyDown(KeyCode.Q))
            dirtyWorld = WorldNew.Move(Vector2Int.RoundToInt(transform.position), false);
        if (Input.GetKeyDown(KeyCode.E))
            dirtyWorld = WorldNew.Move(Vector2Int.RoundToInt(transform.position), true);

        int depth;
        WorldNew.GetTileType(TargetPos + offset, out depth);
        if (depth > 0)
            TargetPos += offset;
        else
            dirtyWorld = false;

        //Util.Load(ref TileData, "Tiles");
        //int depth;
        //bool freeSpace = TileData[WorldNew.GetTileType(tempPos, out depth)].Sprite == null || depth > 0;
        //if (freeSpace && tempPos != TargetPos)
        //    TargetPos = tempPos;
        //else
        //    dirtyWorld = false;

        ////Turning
        //if (Input.GetKeyDown(KeyCode.LeftArrow))
        //{
        //    //Facing = (Direction)(((int)Facing - 1 + 4) % 4);
        //    WorldNew.Turn(Vector2Int.RoundToInt(transform.position), true);
        //    dirtyWorld = true;
        //}
        //if (Input.GetKeyDown(KeyCode.RightArrow))
        //{
        //    //Facing = (Direction)(((int)Facing + 1) % 4);
        //    WorldNew.Turn(Vector2Int.RoundToInt(transform.position), false);
        //    dirtyWorld = true;
        //}

        if (dirtyWorld)
            TileBehaviour.ResetAll();
    }
    //TileData TileData;

    bool keepMoving;
    private void FollowUpdate()
    {
        // Follow
        // LogicalPosition will have to change with direction
        Vector3 vel = new Vector3();
        transform.position = Vector3.SmoothDamp(transform.position, TargetPos, ref vel, 1 / MoveSpeed);

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
            Vector2 start = Vector2Int.RoundToInt(transform.position);
            Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            int numHits = Physics2D.LinecastNonAlloc(start, end, hit, LayerMask.GetMask("Tiles"));
            if (numHits == 0)
                numHits = Physics2D.LinecastNonAlloc(end, end, hit);
            if (numHits != 0)
                if (Vector2.Distance(start, hit[0].transform.position) < MineRadius)
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
        Vector2 end = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int numHits = Physics2D.LinecastNonAlloc(end, end, hit);
        if (numHits != 0)
        {
            Vector2 start = Vector2Int.RoundToInt(transform.position);
            TileBehaviour tb = hit[0].transform.GetComponent<TileBehaviour>();
            bool inRange = Vector2.Distance(start, hit[0].transform.position) < MineRadius;
            bool visible = Physics2D.LinecastNonAlloc(start, end, hit, LayerMask.GetMask("Tiles")) == 0;
            if (inRange && visible && tb.Depth > 0 && Vector2Int.RoundToInt(end) != start)
            {
                WorldNew.SetTileType(end, type);
                //World.SetTileType(new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), Mathf.Round(WorldPos.z)), type);
                tb.ResetTile(false);
            }
        }
    }
}
